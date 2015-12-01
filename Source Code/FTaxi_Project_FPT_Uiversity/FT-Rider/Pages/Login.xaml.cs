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
            //CreatePushChannel();
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
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderLoginAddress, input);
                if (output != null)
                {
                    try
                    {
                        var riderLogin = JsonConvert.DeserializeObject<RiderLogin>(output);
                        if (riderLogin != null)
                        {
                            if (riderLogin.content != null)
                            {
                                tNetAppSetting["isLogin"] = "WasLogined";
                                NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
                                tNetUserLoginData["UserId"] = uid;
                                tNetUserLoginData["PasswordMd5"] = pwmd5;
                                tNetUserLoginData["UserLoginData"] = riderLogin;
                                tNetUserLoginData["RawPassword"] = txt_Password.ActionButtonCommandParameter.ToString();
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


        /*
        private void CreatePushChannel()
        {
            HttpNotificationChannel pushChannel;
            string channelName = "FtaxiChannel";
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
        */

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