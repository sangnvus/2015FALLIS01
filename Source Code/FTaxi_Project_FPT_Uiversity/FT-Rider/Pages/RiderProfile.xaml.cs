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
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using FT_Rider.Classes;
using Newtonsoft.Json;
using System.Diagnostics;
using Telerik.Windows.Controls.PhoneTextBox;



namespace FT_Rider.Pages
{
    public partial class RiderProfile : PhoneApplicationPage
    {
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        RiderLogin userData = null;
        string pwmd5 = string.Empty;
        long preOlmd;

        public RiderProfile()
        {
            InitializeComponent();


            //Load rider profile
            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userData = new RiderLogin();
                userData = (RiderLogin)tNetUserLoginData["UserLoginData"];
                pwmd5 = (string)tNetUserLoginData["PasswordMd5"];
                preOlmd = (long)tNetUserLoginData["UserLmd"];

            }

            LoadRiderProfile();
        }


        private void LoadRiderProfile()
        {
            txt_FirstName.Text = userData.content.fName;
            txt_LastName.Text = userData.content.lName;
            txt_Mobile.Text = "0" + userData.content.mobile;
            txt_Email.Text = userData.content.email;
            tbl_HomeAddress.Text = userData.content.hAdd;
            tbl_OfficeAddress.Text = userData.content.oAdd;
        }


        private void tbl_Tap_ChangePassword(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/RiderChangePassword.xaml", UriKind.Relative));
        }

        private void img_EditIcon_HomeAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/RiderAddHomePlace.xaml", UriKind.Relative));
        }

        private void img_EditIcon_OfficeAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/RiderAddWorkPlace.xaml", UriKind.Relative));
        }

        private void txt_LastName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowSaveButton();
        }

        private void txt_FirstName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowSaveButton();

        }

        private void txt_Email_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowSaveButton();

        }

        private void txt_Mobile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowSaveButton();

        }

        private void ShowSaveButton()
        {
            btn_Save.Visibility = Visibility.Visible;
        }

        private void HideSaveButton()
        {
            btn_Save.Visibility = Visibility.Collapsed;
        }

        private void txt_LastName_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox addressTextbox = (TextBox)sender;
            addressTextbox.Background = new SolidColorBrush(Colors.Transparent);
            addressTextbox.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txt_FirstName_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox addressTextbox = (TextBox)sender;
            addressTextbox.Background = new SolidColorBrush(Colors.Transparent);
            addressTextbox.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txt_Email_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox addressTextbox = (TextBox)sender;
            addressTextbox.Background = new SolidColorBrush(Colors.Transparent);
            addressTextbox.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txt_Mobile_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox addressTextbox = (TextBox)sender;
            addressTextbox.Background = new SolidColorBrush(Colors.Transparent);
            addressTextbox.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private async void btn_Save_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Show loading screen
            ShowLoadingSreen();

            var rid = userData.content.rid;
            var email = txt_Email.Text;
            var fName = txt_FirstName.Text;
            var lName = txt_LastName.Text;
            var mobile = txt_Mobile.Text;
            var uid = userData.content.uid;
            var olmd = preOlmd;; //Cái này là dùng lmd của Login
            var pw = pwmd5;
            var input = string.Format("{{\"rid\":\"{0}\",\"email\":\"{1}\",\"fName\":\"{2}\",\"lName\":\"{3}\",\"mobile\":\"{4}\",\"uid\":\"{5}\",\"pw\":\"{6}\",\"olmd\":\"{7}\"}}", rid, email, fName, lName, mobile, uid, pw, olmd);
            try
            {
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderUpdateProfile, input);
                var updateStatus = JsonConvert.DeserializeObject<BaseResponse>(output);
                if (updateStatus.status.Equals(ConstantVariable.responseCodeSuccess)) //Neu tra ve 0000
                {
                    ///1. Cập nhật lmd
                    ///2. show messenger box
                    ///tắt loading

                    //1
                    tNetUserLoginData["UserLmd"] = updateStatus.lmd;

                    //3
                    HideLoadingSreen();
                    HideSaveButton();

                    //2
                    MessageBox.Show(ConstantVariable.strRiderUpdateSuccess); //Cập nhật thành công
                }
            }
            catch (Exception)
            {
                HideLoadingSreen();
                MessageBox.Show("(Mã lỗi 2201) " + ConstantVariable.errServerErr); //Co loi may chu
                Debug.WriteLine("Có lỗi 7hsgt54 ở update profile");               
            }
        }


        private void ShowLoadingSreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Visible;
        }

        private void HideLoadingSreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Collapsed;
        }




        private bool ValidateEmail()
        {
            if (Regex.IsMatch(txt_Email.Text.Trim(), @"^([a-zA-Z_])([a-zA-Z0-9_\-\.]*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\])$"))
            {
                MessageBox.Show(ConstantVariable.validEmail);
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool ValidateName()
        {
            var NameEmpty = string.IsNullOrEmpty(txt_FirstName.Text);
            if (NameEmpty)
            {
                MessageBox.Show(ConstantVariable.validName);
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool ValidateLastName()
        {
            var FirstAndMiddleNameEmpty = string.IsNullOrEmpty(txt_LastName.Text);
            if (FirstAndMiddleNameEmpty)
            {
                MessageBox.Show(ConstantVariable.validName);
                return false;
            }
            else
            {
                return true;
            }
        }


        private bool ValidatePhoneNumber()
        {
            var PhoneNumberEmpty = string.IsNullOrEmpty(txt_Mobile.Text);
            if (PhoneNumberEmpty || txt_Mobile.Text.Length != 10 || txt_Mobile.Text.Length != 11)
            {
                MessageBox.Show(ConstantVariable.validMobile);
                return false;
            }
            else
            {
                return true;
            }
        }

        private void txt_LastName_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateLastName();
        }

        private void txt_FirstName_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateName();
        }

        private void txt_Email_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateEmail();
        }

        private void txt_Mobile_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidatePhoneNumber();
        }

    }
}