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
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        List<UserData> ObjUserDataList = new List<UserData>();
        string pushChannelURI = null;


        public Login()
        {
            InitializeComponent();
           

            this.txt_UserId.DataContext = new Data { Name = "Email" };
            this.txt_Password.DataContext = new Data { Name = "Passsword" };
            this.Loaded += Login_Loaded;
        }

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
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                pushChannel.Open();

                // Bind this new channel for toast events.
                pushChannel.BindToShellToast();

            }
            else
            {
                // The channel was already open, so just register for all the events.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                // Display the URI for testing purposes. Normally, the URI would be passed back to your web service at this point.
                System.Diagnostics.Debug.WriteLine(pushChannel.ChannelUri.ToString());

                pushChannelURI = pushChannel.ChannelUri.ToString();
                //MessageBox.Show(String.Format("Channel Uri is {0}", pushChannel.ChannelUri.ToString()));

            }
        }


        // Display the new URI for testing purposes.   Normally, the URI would be passed back to your web service at this point.
        void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {

            Dispatcher.BeginInvoke(() =>
            {
                System.Diagnostics.Debug.WriteLine(e.ChannelUri.ToString());
                pushChannelURI = e.ChannelUri.ToString();
                //MessageBox.Show(String.Format("Channel Uri is {0}",e.ChannelUri.ToString()));

                //>>>>>>>>>>>>>>>>>>>>>>>>> Chan URI HERE <<<<<<<<<<<<<<<<<<<<<<
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


        // Parse out the information that was part of the message.
        void PushChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            StringBuilder message = new StringBuilder();
            string relativeUri = string.Empty;

            message.AppendFormat("Received Toast {0}:\n", DateTime.Now.ToShortTimeString());

            // Parse out the information that was part of the message.
            foreach (string key in e.Collection.Keys)
            {
                message.AppendFormat("{0}: {1}\n", key, e.Collection[key]);

                if (string.Compare(
                    key,
                    "wp:Param",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.CompareOptions.IgnoreCase) == 0)
                {
                    relativeUri = e.Collection[key];
                }
            }

            // Display a dialog of all the fields in the toast.
            Dispatcher.BeginInvoke(() => MessageBox.Show(message.ToString()));

        }


        private bool Validate(string text)
        {
            //Your validation logic
            return false;
        }



        public class Data
        {
            public string Name { get; set; }
        }


        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
            CreatePushChannel();

            var Settings = IsolatedStorageSettings.ApplicationSettings;
            //Check if user already login,so we need to direclty navigate to details page instead of showing login page when user launch the app.  
            if (Settings.Contains("CheckLogin"))
            {
                NavigationService.Navigate(new Uri("DriverProfile.xaml", UriKind.Relative));
            }
            else
            {
                if (ISOFile.FileExists("RegistrationDetails"))//loaded previous items into list  
                {
                    using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("RegistrationDetails", FileMode.Open))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(List<UserData>));
                        ObjUserDataList = (List<UserData>)serializer.ReadObject(fileStream);

                    }
                }
            }
        }



        private async void tbn_Tap_Login(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (txt_UserId.Text != "" && txt_Password.ToString() != "") 
            {
                var uid = txt_UserId.Text;
                MD5.MD5 pw = new MD5.MD5();
                pw.Value = txt_Password.ActionButtonCommandParameter.ToString();
                var pwmd5 = pw.FingerPrint.ToLower();
                var mid = pushChannelURI; //HttpUtility.UrlEncode(pushChannelURI); ;
                var mType = ConstantVariable.mTypeWIN;
                var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"mid\":\"{2}\",\"mType\":\"{3}\"}}", uid, pwmd5, mid, mType);
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverLoginAddress, input);
                try
                {
                    
                    var driverLogin = JsonConvert.DeserializeObject<DriverLogin>(output);
                    if (driverLogin.content != null)
                    {
                        NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
                        PhoneApplicationService.Current.State["UserInfo"] = driverLogin;
                        PhoneApplicationService.Current.State["UserId"] = uid;
                        PhoneApplicationService.Current.State["PasswordMd5"] = pwmd5;
                    }
                    else
                    {
                        MessageBox.Show(ConstantVariable.errLoginFailed);
                    }

                }
                catch (Exception)
                {

                    //Login Failed | Đăng nhập không thành công
                    MessageBox.Show(ConstantVariable.errLoginFailed);
                }
            }
//             if (txt_UserId.Text != "" && txt_Password.ToString() != "")
//             {
//                 int Temp = 0;
//                
//                     if (txt_UserId.Text == "admin@gmail.com" && txt_Password.Password == "admin")
//                     {
//                         Temp = 1;
//                         NavigationService.Navigate(new Uri("/Pages/DriverProfile.xaml", UriKind.Relative));
//                     
//                 }
//                 if (Temp == 0)
//                 {
//                     txt_Password.ChangeValidationState(ValidationState.Invalid, "");
//                     txt_UserId.ChangeValidationState(ValidationState.Invalid, "");
//                 }
//             }
//             else
//             {
//                 txt_Password.ChangeValidationState(ValidationState.Invalid, "");
//                 txt_UserId.ChangeValidationState(ValidationState.Invalid, "");
//             }

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