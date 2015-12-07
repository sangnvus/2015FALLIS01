using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.IO.IsolatedStorage;
using FT_Rider.Classes;
using System.Diagnostics;
using Newtonsoft.Json;

namespace FT_Rider.Pages
{
    public partial class RiderChangePassword : PhoneApplicationPage
    {

        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        IsolatedStorageSettings tNetAppSetting = IsolatedStorageSettings.ApplicationSettings;

        string pwmd5 = string.Empty;
        string userId = string.Empty;
        string rawPassword = string.Empty;
        public RiderChangePassword()
        {
            InitializeComponent();
            

            //Get User data from login
            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userId = (string)tNetUserLoginData["UserId"];
                rawPassword = (string)tNetUserLoginData["RawPassword"];
            }
        }

        private void txt_OldPassword_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void txt_NewPassword_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void txt_NewPassWordAgain_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }


        //Clear all session data
        public void ClearAllUserData()
        {
            tNetAppSetting.Remove("isLogin");
            tNetUserLoginData.Remove("UserId");
            tNetUserLoginData.Remove("PasswordMd5");
            tNetUserLoginData.Remove("RawPassword");
            tNetUserLoginData.Remove("UserLmd");
            tNetUserLoginData.Remove("UserLoginData");
            tNetUserLoginData.Remove("cityNamesDB");
        }


        private void btn_Confirm_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (txt_OldPassword.Password.Equals(null))
            {
                MessageBox.Show("Lòng điền thông tin cần thiết!");
            }
            else
            {
                if (CheckOldPassword() && CheckNewPassword() && CheckPasswordAgain())
                {
                    ChangePassword();
                }
            }

        }

        private async void ChangePassword()
        {
            //Show
            ShowLoadingScreen();

            var opw = txt_OldPassword.Password;
            var uid = userId;
            var npw = txt_NewPassWordAgain.Password;
            var input = string.Format("{{\"opw\":\"{0}\",\"npw\":\"{1}\",\"uid\":\"{2}\"}}", opw, npw, uid);
            try
            {
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderChangePassword, input);
                var changePassStatus = JsonConvert.DeserializeObject<BaseResponse>(output);
                if (changePassStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS)) //0000
                {
                    ///1. update lmd
                    ///2. tat loading screen
                    ///3. show messs
                    ///4. Xóa toàn bộ thông tin session
                    ///5. Quay về màn login
                    
                    //1
                    tNetUserLoginData["UserLmd"] = changePassStatus.lmd;

                    //2.
                    HideLoadingScreen();

                    //3.
                    MessageBox.Show(ConstantVariable.strChangePassSuccess); //Đổi mk thành công

                    //4.
                    ClearAllUserData();
                    NavigationService.Navigate(new Uri("/Pages/Login.xaml", UriKind.Relative));

                }
            }
            catch (Exception)
            {

                HideLoadingScreen();
                MessageBox.Show(ConstantVariable.errServerErr);
            }
        }



        private bool CheckNewPassword()
        {
            var passwordEmpty = string.IsNullOrEmpty(txt_NewPassword.Password);
            //tbPasswordWatermark.Opacity = passwordEmpty ? 100 : 0;
            //pbPassword.Opacity = passwordEmpty ? 0 : 100;
            if (passwordEmpty || txt_NewPassword.Password.Length < 6)
            {
                MessageBox.Show(ConstantVariable.strPassNotValid); //Mk khong hop le
                txt_NewPassword.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }


        private void ShowLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Visible;
        }

        private void HideLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Collapsed;
        }


        private bool CheckOldPassword()
        {
            var mypw = txt_OldPassword.Password;
            if (mypw.Equals(rawPassword))
            {
                return true;
            }
            else
            {
                MessageBox.Show(ConstantVariable.strOldPassNotCorrect); //Mk khong chinh xac
                txt_OldPassword.Focus();
                Debug.WriteLine("Có lỗi 6986254fgg ở check old password");
                return false;
            }
        }

        private bool CheckPasswordAgain()
        {
            if (txt_NewPassword.Password.Equals(txt_NewPassWordAgain.Password))
            {
                return true;
            }
            else
            {
                MessageBox.Show(ConstantVariable.strPassNotLike); //Mk khong trung khop
                txt_NewPassWordAgain.Focus();
                return false;
            }
        }

        private void txt_NewPassword_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckNewPassword();
        }

        private void txt_NewPassWordAgain_LostFocus(object sender, RoutedEventArgs e)
        {
            CheckPasswordAgain();
        }

    }
}