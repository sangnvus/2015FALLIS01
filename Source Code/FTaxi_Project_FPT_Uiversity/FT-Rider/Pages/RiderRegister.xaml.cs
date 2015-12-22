using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO.IsolatedStorage;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using FT_Rider.Classes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls.PhoneTextBox;
using Newtonsoft.Json;


namespace FT_Rider.Pages
{
    public partial class RiderRegister : PhoneApplicationPage
    {

        public RiderRegister()
        {
            InitializeComponent();
            this.txt_UserId.DataContext = new Data { Name = "Email" };
            this.txt_Password.DataContext = new Data { Name = "Mật khẩu" };
            this.txt_PasswordAgain.DataContext = new Data { Name = "Nhập lại mật khẩu" };
            this.txt_FirstName.DataContext = new Data { Name = "Tên" };
            this.txt_LastName.DataContext = new Data { Name = "Họ" };
            this.txt_Mobile.DataContext = new Data { Name = "Số điện thoại" };
        }


        public static bool IsNumeric(string s)
        {
            return IsNumeric(s, false);
        }

        public static bool IsNumeric(string s, bool allowDecimal)
        {
            bool result = true;
            if (String.IsNullOrEmpty(s))
            {
                return false;
            }

            if (s.StartsWith("-"))
            {
                s = s.Substring(1);
            }

            char[] chars = s.ToCharArray();

            if (allowDecimal)
            {
                bool decimalFound = false;
                foreach (char c in chars)
                {
                    if (c == '.' && !decimalFound)
                    {
                        decimalFound = true;
                    }
                    else
                    {
                        result = result & (char.IsNumber(c));
                    }
                }
            }
            else
            {
                foreach (char c in chars)
                {
                    result = result & char.IsNumber(c);
                }
            }

            return result;
        }


        private bool ValidateEmail()
        {
            if (txt_UserId.Text.Trim().Length > 15 && txt_UserId.Text.Trim().Length < 50 && Regex.IsMatch(txt_UserId.Text.Trim(), @"^([a-zA-Z_])([a-zA-Z0-9_\-\.]*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\])$"))
            {
                txt_UserId.ChangeValidationState(ValidationState.Valid, "");
                return true;
            }

            {
                txt_UserId.ChangeValidationState(ValidationState.Invalid, "Nhập lại Email");
                return false;
            }
        }


        private bool ValidatePassword()
        {
            var passwordEmpty = string.IsNullOrEmpty(txt_Password.Password);
            if (txt_Password.Password.Length < 6)
            {

                txt_Password.ChangeValidationState(ValidationState.Invalid, "Mật khẩu ít nhất có 6 ký tự ");
                return false;
            }
            else if (txt_Password.Password.Length > 24)
            {
                txt_Password.ChangeValidationState(ValidationState.Invalid, "Mật khẩu lớn nhất 24 ký tự ");
                return false;
            }
            else if (passwordEmpty)
            {
                txt_Password.ChangeValidationState(ValidationState.Invalid, "Vui lòng nhập mật khẩu");
                return false;
            }
            else
            {
                txt_Password.ChangeValidationState(ValidationState.Valid, "");
                return true;
            }
        }

        private bool ValidateVerifyPassword()
        {
            var passwordEmpty = string.IsNullOrEmpty(txt_PasswordAgain.Password);
            if (txt_PasswordAgain.Password.Length < 6)
            {

                txt_PasswordAgain.ChangeValidationState(ValidationState.Invalid, "Mật khẩu ít nhất 6 kí tự !");
                return false;
            }
            else if (passwordEmpty)
            {
                txt_PasswordAgain.ChangeValidationState(ValidationState.Invalid, "Vui lòng nhập mật khẩu");
                return false;
            }
            else
                if (!txt_PasswordAgain.Password.Equals(txt_Password.Password))
                {
                    txt_PasswordAgain.ChangeValidationState(ValidationState.Invalid, "Mật khẩu không trùng nhau !");
                    return false;
                }

                else
                {

                    txt_PasswordAgain.ChangeValidationState(ValidationState.Valid, "");
                    return true;
                }
        }

        private bool ValidateName()
        {
            var NameEmpty = string.IsNullOrEmpty(txt_FirstName.Text.Trim());
            if (NameEmpty || txt_FirstName.Text.Trim().Length > 20 || txt_FirstName.Text.Trim().Length < 1)
            {
                txt_FirstName.ChangeValidationState(ValidationState.Invalid, "Xin hãy nhập tên");
                return false;
            }
            else
            {
                txt_FirstName.ChangeValidationState(ValidationState.Valid, "");
                return true;
            }
        }

        private bool ValidateFirstAndMiddleName()
        {
            var FirstAndMiddleNameEmpty = string.IsNullOrEmpty(txt_LastName.Text);
            if (FirstAndMiddleNameEmpty && txt_LastName.Text.Length < 30)
            {
                txt_LastName.ChangeValidationState(ValidationState.Invalid, "Xin hãy nhập họ và tên đệm");
                return false;
            }
            else
            {
                txt_LastName.ChangeValidationState(ValidationState.Valid, "");
                return true;
            }
        }


        private bool ValidatePhoneNumber()
        {
            var PhoneNumberEmpty = string.IsNullOrEmpty(txt_Mobile.Text);
            if (PhoneNumberEmpty || txt_Mobile.Text.Length < 10 || txt_Mobile.Text.Length > 11)
            {
                txt_Mobile.ChangeValidationState(ValidationState.Invalid, "Số điện thoại phải có 10 hoặc 11 chứ số.");
                return false;
            }
            else
            {
                txt_Mobile.ChangeValidationState(ValidationState.Valid, "");
                return true;
            }
        }



        private void txt_UserId_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateEmail();
        }

        private void txt_Password_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidatePassword();
        }

        private void txt_PasswordAgain_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateVerifyPassword();
        }

        private void txt_FirstName_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateName();

        }
        private void txt_LastName_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateFirstAndMiddleName();
        }
        private void txt_Mobile_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidatePhoneNumber();
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


        private async void btn_Click_Register(object sender, RoutedEventArgs e)
        {
            if (ValidateEmail() && ValidatePassword() && ValidateVerifyPassword() && ValidateName() && ValidatePhoneNumber() && ValidateFirstAndMiddleName())
            {
                ShowLoadingScreen();

                //Recheck PMT input
                var uid = txt_UserId.Text;
                var pw = txt_Password.ActionButtonCommandParameter.ToString();
                var cntry = "VN";
                var fName = txt_FirstName.Text;
                var lName = txt_LastName.Text;
                var lan = "vi";
                var mobile = txt_Mobile.Text;

                var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"fName\":\"{2}\",\"lName\":\"{3}\",\"mobile\":\"{4}\",\"lan\":\"{5}\",\"cntry\":\"{6}\",\"pmt\":[]}}", uid, pw, fName, lName, mobile, lan, cntry);
                try
                {
                    var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderRegisterAddress, input);
                    var result = JsonConvert.DeserializeObject<BaseResponse>(output);

                    switch (result.status)
                    {
                        case ConstantVariable.RESPONSECODE_SUCCESS:
                            MessageBox.Show(ConstantVariable.strLoginSuccessed);
                            NavigationService.Navigate(new Uri("/Pages/Login.xaml", UriKind.Relative));
                            HideLoadingScreen();
                            break;
                        case ConstantVariable.RESPONSECODE_DUPLICATED:
                            MessageBox.Show("(Mã lỗi 4503) " + ConstantVariable.DUPLICATED);
                            HideLoadingScreen();
                            break;
                        case ConstantVariable.RESPONSECODE_DUPPLICATED_PHONE_NUMBER:
                            MessageBox.Show("(Mã lỗi 4502) " + ConstantVariable.DUPPLICATED_PHONE_NUMBER);
                            HideLoadingScreen();
                            break;
                    }
                }
                catch (Exception)
                {
                    HideLoadingScreen();
                    MessageBox.Show("(Mã lỗi 4501) " + ConstantVariable.errServerErr);
                }

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
    }
}