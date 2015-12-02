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
using FT_Driver.Classes;
using FT_Driver.Resources;
using Telerik.Windows.Controls.PhoneTextBox;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Phone.Notification;
using System.Text;

namespace FT_Driver.Pages
{
    public partial class Login : PhoneApplicationPage
    {
       // string pushChannelURI = null;
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        IsolatedStorageSettings tNetAppSetting = IsolatedStorageSettings.ApplicationSettings;


        public Login()
        {
            InitializeComponent();

            //Create Push notification Channel
           // CreatePushChannel();
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
                var mid = "";//pushChannelURI; //HttpUtility.UrlEncode(pushChannelURI); ;
                var mType = ConstantVariable.mTypeWIN;

                var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"mid\":\"{2}\",\"mType\":\"{3}\"}}", uid, pwmd5, mid, mType);
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverLoginAddress, input);
                if (output != null)
                {
                    try
                    {

                        var driverLogin = JsonConvert.DeserializeObject<DriverLogin>(output);
                        if (driverLogin != null)
                        {
                            if (driverLogin.content != null)
                            {
                                tNetAppSetting["isLogin"] = "WasLogined"; //Change login state to Logined

                                NavigationService.Navigate(new Uri("/Pages/DriverCarList.xaml", UriKind.Relative));
                                tNetUserLoginData["UserId"] = uid;
                                tNetUserLoginData["PasswordMd5"] = pwmd5;
                                tNetUserLoginData["UserLoginData"] = driverLogin;                                
                            }
                            else
                            {
                                MessageBox.Show(ConstantVariable.errLoginFailed);
                                grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable Process bar
                            }
                        }
                        else
                        {
                            MessageBox.Show(ConstantVariable.errLoginFailed);
                            grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable Process bar
                        }

                    }
                    catch (Exception)
                    {

                        //Login Failed | Đăng nhập không thành công
                        MessageBox.Show(ConstantVariable.errLoginFailed);
                        grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable Process bar
                    }
                }
                else
                {
                    MessageBox.Show(ConstantVariable.errConnectingError);
                    grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable Process bar
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
            NavigationService.Navigate(new Uri("/Pages/DriverRegister.xaml", UriKind.Relative));
        }

        private void tbl_Tap_LostPassword(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/DriverLostPassword.xaml", UriKind.Relative));
        }


        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            NavigationService.Navigate(new Uri("/Pages/DriverCarList.xaml", UriKind.Relative));
        }


    }
}