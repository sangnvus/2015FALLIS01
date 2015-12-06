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


        public Login()
        {
            InitializeComponent();

            //Create Push notification Channel            
        }


        private async void tbn_Tap_Login(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (txt_UserId.Text != "" && txt_Password.ActionButtonCommandParameter.ToString() != "")
            {

                grv_ProcessScreen.Visibility = Visibility.Visible; //Enable Process bar
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
                    var riderLogin = JsonConvert.DeserializeObject<RiderLogin>(output);
                    if (riderLogin != null)
                    {
                        if (riderLogin.status.Equals(ConstantVariable.responseCodeSuccess)) //0000 Code
                        {
                            tNetAppSetting["isLogin"] = "WasLogined";
                            NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
                            tNetUserLoginData["UserId"] = uid;
                            tNetUserLoginData["PasswordMd5"] = pwmd5;
                            tNetUserLoginData["UserLoginData"] = riderLogin;
                            tNetUserLoginData["RawPassword"] = txt_Password.ActionButtonCommandParameter.ToString();
                            tNetUserLoginData["UserLmd"] = riderLogin.content.olmd; //Cái này là để cập nhật lmd cho việc update thông tin
                        }
                        else
                        {
                            MessageBox.Show(ConstantVariable.errLoginFailed);
                            grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable Process bar
                            Debug.WriteLine("(Mã lỗi 1103) " + ConstantVariable.errLoginFailed);
                        }
                    }
                    else
                    {
                        MessageBox.Show(ConstantVariable.errLoginFailed);
                        grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable Process bar
                        Debug.WriteLine("(Mã lỗi 1102) " + ConstantVariable.errLoginFailed);
                    }

                }
                catch (Exception)
                {
                    MessageBox.Show(ConstantVariable.errConnectingError);
                    grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable Process bar
                    Debug.WriteLine("(Mã lỗi 1101) " + ConstantVariable.errConnectingError);
                }
            }
            else
            {
                MessageBox.Show(ConstantVariable.errNotEmpty);
                grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable Process bar
            }
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
    }
}