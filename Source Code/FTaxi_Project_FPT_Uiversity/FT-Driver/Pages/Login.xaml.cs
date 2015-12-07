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


        private void HideLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable Process bar
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
                        if (driverLogin.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS))// neu ok 0000
                        {
                            tNetAppSetting["isLogin"] = "WasLogined"; //Change login state to Logined

                            NavigationService.Navigate(new Uri("/Pages/DriverCarList.xaml", UriKind.Relative));
                            tNetUserLoginData["UserId"] = uid;
                            tNetUserLoginData["PasswordMd5"] = pwmd5;
                            tNetUserLoginData["UserLoginData"] = driverLogin;
                            tNetUserLoginData["UserLmd"] = driverLogin.lmd;
                            tNetUserLoginData["RawPassword"] = txt_Password.ActionButtonCommandParameter.ToString();
                            tNetUserLoginData["PushChannelURI"] = pushChannelURI;
                        }
                        else
                        {
                            MessageBox.Show("(Mã lỗi 3101) " + ConstantVariable.errLoginFailed);
                            HideLoadingScreen();
                            Debug.WriteLine("Có lỗi 265fgt67 ở Driver Login");
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

                pushChannel.Open();

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


        //// Parse out the information that was part of the message.
        //void PushChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        //{
           
        //    StringBuilder message = new StringBuilder();
        //    //string relativeUri = string.Empty;

        //    //message.AppendFormat("Received Toast {0}:\n", DateTime.Now.ToShortTimeString());

        //    // Parse out the information that was part of the message.
        //    foreach (string key in e.Collection.Keys)
        //    {
        //        //message.AppendFormat("{0}: {1}\n", key, e.Collection[key]);

        //        if (string.Compare(
        //            key,
        //            "wp:Param",
        //            System.Globalization.CultureInfo.InvariantCulture,
        //            System.Globalization.CompareOptions.IgnoreCase) == 0)
        //        {
        //            //Lấy chuỗi thông báo từ Notification
        //            notificationReceivedString = e.Collection[key];
        //        }
        //    }

        //    // Display a dialog of all the fields in the toast.
        //    //Dispatcher.BeginInvoke(() => MessageBox.Show(message.ToString()));          
        //    Dispatcher.BeginInvoke(() =>
        //    {
        //        //notificationReceivedString = e.Collection.Keys.;
        //        //MessageBox.Show(notificationReceivedString);
        //        if (notificationReceivedString != string.Empty)
        //        {

        //            //Hàm này để lấy ra chuỗi Json trong một String gửi qua notification
        //            int a = notificationReceivedString.IndexOf("json=");
        //            int b = notificationReceivedString.IndexOf("}");
        //            int c = notificationReceivedString.IndexOf("notiType=");
        //            string tmpStirng = notificationReceivedString.Substring(a + 5, b - a - 4);
        //            //Cái này để lấy kiểu 
        //            notificationType = notificationReceivedString.Substring(c + 9, notificationReceivedString.Length - c - 9);
        //            notificationReceivedString = tmpStirng;

        //            //Sau đó chạy thông báo
        //            ShowNotification();
        //        }
        //    });

        //}


    }
}