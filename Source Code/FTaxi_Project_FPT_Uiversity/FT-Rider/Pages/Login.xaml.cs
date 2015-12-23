using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FT_Rider.Classes;
using FT_Rider.Resources;
using Telerik.Windows.Controls.PhoneTextBox;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Microsoft.Phone.Notification;
using System.Diagnostics;

namespace FT_Rider.Pages
{
    public partial class Login : PhoneApplicationPage
    {
        //string pushChannelURI = null;
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        IsolatedStorageSettings tNetAppSetting = IsolatedStorageSettings.ApplicationSettings;
        RiderLogin riderLogin = null;

        public Login()
        {
            InitializeComponent();

            //Create Push notification Channel            
        }


        /// <summary>
        /// CÁI NÀY ĐỂ VÔ HIỆU HÓA NÚT BACK Ở HOME
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            //base.OnBackKeyPress(e);
            //MessageBox.Show("You can not use Hardware back button");
            e.Cancel = true;
        }

        private async void tbn_Tap_Login(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (txt_UserId.Text != "" && txt_Password.ActionButtonCommandParameter.ToString() != "")
            {
                ShowLoadingScreen();
                var uid = txt_UserId.Text;
                MD5.MD5 pw = new MD5.MD5();
                pw.Value = txt_Password.ActionButtonCommandParameter.ToString();
                var pwmd5 = pw.FingerPrint.ToLower();
                var mid = "";//pushChannelURI;
                var mType = ConstantVariable.mTypeWIN;
                var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"mid\":\"{2}\",\"mType\":\"{3}\"}}", uid, pwmd5, mid, mType);
                try
                {
                    //Thử xem có lấy đc JSON về ko, nếu ko thì bắn ra Lối kết nối / lỗi server
                    var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderLoginAddress, input);
                    riderLogin = new RiderLogin();
                    riderLogin = JsonConvert.DeserializeObject<RiderLogin>(output);
                    if (riderLogin != null)
                    {
                        switch (riderLogin.status)
                        {
                            case ConstantVariable.RESPONSECODE_SUCCESS: //0000 OK
                                tNetAppSetting["isLogin"] = "WasLogined";
                                NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
                                tNetUserLoginData["UserId"] = uid;
                                tNetUserLoginData["PasswordMd5"] = pwmd5;
                                tNetUserLoginData["UserLoginData"] = riderLogin;
                                tNetUserLoginData["RawPassword"] = txt_Password.ActionButtonCommandParameter.ToString();
                                tNetUserLoginData["UserLmd"] = riderLogin.content.lmd; //Cái này là để cập nhật lmd cho việc update thông tin
                                HideLoadingScreen();
                                break;
                            case ConstantVariable.RESPONSECODE_USERNAME_NOT_CORRECT: //Tài khoản không đúng
                                ShowMessageUSERNAME_NOT_CORRECT();
                                HideLoadingScreen();
                                break;
                            case ConstantVariable.RESPONSECODE_PASSWORD_NOT_CORRECT:
                                ShowMessagePASSWORD_NOT_CORRECT();
                                HideLoadingScreen();
                                break;
                            case ConstantVariable.RESPONSECODE_ERR_SYSTEM:
                                ShowMessageERR_SYSTEM();
                                HideLoadingScreen();
                                break;
                            case ConstantVariable.RESPONSECODE_INVALID_PASSWORD:
                                ShowMessageINVALID_PASSWORD();
                                HideLoadingScreen();
                                break;
                            case ConstantVariable.RESPONSECODE_USERNAME_NOT_FOUND:
                                ShowMessageUSERNAME_NOT_FOUND();
                                HideLoadingScreen();
                                break;
                            case ConstantVariable.RESPONSECODE_INVALID_USER_GROUP:
                                ShowMessageINVALID_USER_GROUP();
                                HideLoadingScreen();
                                break;
                            default:
                                MessageBox.Show("(Mã lỗi 1105) " + ConstantVariable.errLoginFailed);
                                HideLoadingScreen();
                                Debug.WriteLine("Có lỗi 265fgt67 ở Rider Login");
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("(Mã lỗi 1102) " + ConstantVariable.errLoginFailed);
                        HideLoadingScreen();
                        Debug.WriteLine("Có lỗi 2356fgg ở Login");
                    }

                }
                catch (Exception)
                {
                    MessageBox.Show("(Mã lỗi 1101) " + ConstantVariable.errConnectingError);
                    HideLoadingScreen();
                    Debug.WriteLine("Có lỗi 25hg567 ở Login");
                }
            }
            else
            {
                MessageBox.Show("(Mã lỗi 1109) " + ConstantVariable.errNotEmpty);
                HideLoadingScreen();
            }
        }

       

        private void ShowLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Visible; //Enable Process bar
        }

        private void HideLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable Process bar
        }


        private void tbn_Tap_Register(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/RiderRegister.xaml", UriKind.Relative));
        }

        private void tbl_Tap_LostPassword(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/RiderLostPassword.xaml", UriKind.Relative));
        }


        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
        }


        private void txt_Password_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
        }


        private void ShowMessageUSERNAME_NOT_CORRECT()
        {
            MessageBox.Show("(Mã lỗi 1108) " + ConstantVariable.USERNAME_NOT_CORRECT);
            txt_UserId.Focus();
        }
        private void ShowMessagePASSWORD_NOT_CORRECT()
        {
            MessageBox.Show("(Mã lỗi 1107) " + ConstantVariable.PASSWORD_NOT_CORRECT);
            txt_Password.Focus();
        }
        private void ShowMessageERR_SYSTEM()
        {
            MessageBox.Show("(Mã lỗi 1106) " + ConstantVariable.ERR_SYSTEM);
            this.Focus();
        }
        private void ShowMessageINVALID_PASSWORD()
        {
            MessageBox.Show("(Mã lỗi 1100) " + ConstantVariable.INVALID_PASSWORD);
            txt_Password.Focus();
        }
        private void ShowMessageUSERNAME_NOT_FOUND()
        {
            MessageBox.Show("(Mã lỗi 1112) " + ConstantVariable.USERNAME_NOT_FOUND);
            txt_UserId.Focus();
        }
        private void ShowMessageINVALID_USER_GROUP()
        {
            MessageBox.Show("(Mã lỗi 1112) " + ConstantVariable.INVALID_USER_GROUP);
            txt_UserId.Focus();
        }
    }
}