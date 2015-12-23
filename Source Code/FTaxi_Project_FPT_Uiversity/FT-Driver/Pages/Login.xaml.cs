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
using System.Diagnostics;

namespace FT_Driver.Pages
{
    public partial class Login : PhoneApplicationPage
    {
        // string pushChannelURI = null;
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        IsolatedStorageSettings tNetAppSetting = IsolatedStorageSettings.ApplicationSettings;
        string pushChannelURI = string.Empty;


        public Login()
        {
            InitializeComponent();

            CreatePushChannel();
            //Create Push notification Channel
            // CreatePushChannel();
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

        private void HideLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable Process bar
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
                var mid = pushChannelURI; //HttpUtility.UrlEncode(pushChannelURI); ;
                var mType = ConstantVariable.mTypeWIN;
                var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"mid\":\"{2}\",\"mType\":\"{3}\"}}", uid, pwmd5, mid, mType);

                try
                {
                    //Thử xem có lấy đc dữ liệu ko
                    var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverLoginAddress, input);
                    var driverLogin = JsonConvert.DeserializeObject<DriverLogin>(output);
                    if (driverLogin != null)
                    {
                        //Kiểm tra các trường hợp trả về
                        switch (driverLogin.status)
                        {
                            case ConstantVariable.RESPONSECODE_SUCCESS: //0000 OK
                                tNetAppSetting["isLogin"] = "WasLogined"; //Change login state to Logined
                                NavigationService.Navigate(new Uri("/Pages/DriverCarList.xaml", UriKind.Relative));
                                tNetUserLoginData["UserId"] = uid;
                                tNetUserLoginData["PasswordMd5"] = pwmd5;
                                tNetUserLoginData["UserLoginData"] = driverLogin;
                                tNetUserLoginData["UserLmd"] = driverLogin.lmd;
                                tNetUserLoginData["RawPassword"] = txt_Password.ActionButtonCommandParameter.ToString();
                                tNetUserLoginData["PushChannelURI"] = pushChannelURI;
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
                                MessageBox.Show("(Mã lỗi 3101) " + ConstantVariable.errLoginFailed);
                                HideLoadingScreen();
                                Debug.WriteLine("Có lỗi 265fgt67 ở Driver Login");
                                break;

                        }
                    }
                    else
                    {
                        MessageBox.Show("(Mã lỗi 3102) " + ConstantVariable.errServerError);
                        HideLoadingScreen();
                        Debug.WriteLine("Có lỗi 693fgh10 ở Driver Login");
                    }
                }
                catch (Exception)
                {
                    //Nếu không thì
                    MessageBox.Show("(Mã lỗi 3103) " + ConstantVariable.errConnectingError);
                    HideLoadingScreen();
                    Debug.WriteLine("Có lỗi 78s558 ở Driver Login");
                }

            }
            else
            {
                MessageBox.Show("(Mã lỗi 3104) " + ConstantVariable.errNotEmpty);
                HideLoadingScreen();
                Debug.WriteLine("Có lỗi 68dfghr ở Driver Login");
            }

        }

        private void ShowLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Visible; //Enable Process bar
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



        /// <summary>
        /// NOTIFICATION CHANNEL
        /// </summary>
        private void CreatePushChannel()
        {
            HttpNotificationChannel pushChannel;
            string channelName = "FtaxiDriverChannel";
            pushChannel = HttpNotificationChannel.Find(channelName);

            if (pushChannel == null)
            {
                pushChannel = new HttpNotificationChannel(channelName);

                // Register for all the events before attempting to open the channel.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                //pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                //pushChannel.Open();

                // Bind this new channel for toast events.
                //pushChannel.BindToShellToast();

            }
            else
            {
                // The channel was already open, so just register for all the events.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                //pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                // Display the URI for testing purposes. Normally, the URI would be passed back to your web service at this point.
                System.Diagnostics.Debug.WriteLine(pushChannel.ChannelUri.ToString());

                pushChannelURI = pushChannel.ChannelUri.ToString();


            }
        }

        // Display the new URI for testing purposes.   Normally, the URI would be passed back to your web service at this point.
        void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {

            Dispatcher.BeginInvoke(() =>
            {
                System.Diagnostics.Debug.WriteLine(e.ChannelUri.ToString());
                pushChannelURI = e.ChannelUri.ToString();
            });
        }


        // Error handling logic for your particular application would be here.
        void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
                MessageBox.Show(String.Format("A push notification {0} error occurred.  {1} ({2}) {3}",
                    e.ErrorType, e.Message, e.ErrorCode, e.ErrorAdditionalData))
                    );
        }


        private void ShowMessageUSERNAME_NOT_CORRECT()
        {
            MessageBox.Show("(Mã lỗi 3116) " + ConstantVariable.USERNAME_NOT_CORRECT);
            txt_UserId.Focus();
        }
        private void ShowMessagePASSWORD_NOT_CORRECT()
        {
            MessageBox.Show("(Mã lỗi 3115) " + ConstantVariable.PASSWORD_NOT_CORRECT);
            txt_Password.Focus();
        }
        private void ShowMessageERR_SYSTEM()
        {
            MessageBox.Show("(Mã lỗi 3114) " + ConstantVariable.ERR_SYSTEM);
            this.Focus();
        }
        private void ShowMessageINVALID_PASSWORD()
        {
            MessageBox.Show("(Mã lỗi 3112) " + ConstantVariable.INVALID_PASSWORD);
            txt_Password.Focus();
        }
        private void ShowMessageUSERNAME_NOT_FOUND()
        {
            MessageBox.Show("(Mã lỗi 3111) " + ConstantVariable.USERNAME_NOT_FOUND);
            txt_UserId.Focus();
        }
        private void ShowMessageINVALID_USER_GROUP()
        {
            MessageBox.Show("(Mã lỗi 3110) " + ConstantVariable.INVALID_USER_GROUP);
            txt_UserId.Focus();
        }
    }
}