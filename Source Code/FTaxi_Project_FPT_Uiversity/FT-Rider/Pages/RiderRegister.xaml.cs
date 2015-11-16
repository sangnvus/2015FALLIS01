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


namespace FT_Rider.Pages
{
    public partial class RiderRegister : PhoneApplicationPage
    {
        IsolatedStorageFile iSOFile = IsolatedStorageFile.GetUserStoreForApplication();
        List<UserData> objUserDataList = new List<UserData>();

        public RiderRegister()
        {
            InitializeComponent();
            this.rad_Email.DataContext = new Data { Name = "Email" };
            this.rad_Password.DataContext = new Data { Name = "Mật khẩu" };
            this.rad_PasswordAgain.DataContext = new Data { Name = "Nhập lại mật khẩu" };
            this.rad_Name.DataContext = new Data { Name = "Tên" };
            this.rad_FirstAndMiddleName.DataContext = new Data { Name = "Họ" };
            this.rad_PhoneNumber.DataContext = new Data { Name = "Số điện thoại" };
            this.Loaded += RiderRegister_Loaded;
        }


        private void RiderRegister_Loaded(object sender, RoutedEventArgs e)
        {
            if (iSOFile.FileExists("RegistrationDetails"))//loaded previous items into list
            {
                using (IsolatedStorageFileStream fileStream = iSOFile.OpenFile("RegistrationDetails", FileMode.Open))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(List<UserData>));
                    objUserDataList = (List<UserData>)serializer.ReadObject(fileStream);

                }
            }
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
            if (Regex.IsMatch(rad_Email.Text.Trim(), @"^([a-zA-Z_])([a-zA-Z0-9_\-\.]*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\])$"))
            {
                rad_Email.ChangeValidationState(ValidationState.Valid, "");
                return true;
            }
            else
            {
                rad_Email.ChangeValidationState(ValidationState.Invalid, "Nhập lại Email");
                return false;
            }
        }


        private bool ValidatePassword()
        {
            var passwordEmpty = string.IsNullOrEmpty(rad_Password.Password);
            //tbPasswordWatermark.Opacity = passwordEmpty ? 100 : 0;
            //pbPassword.Opacity = passwordEmpty ? 0 : 100;
            if (passwordEmpty || rad_Password.Password.Length < 6)
            {

                rad_Password.ChangeValidationState(ValidationState.Invalid, "Vui lòng nhập mật khẩu");
                return false;
            }
            else
            {
                rad_Password.ChangeValidationState(ValidationState.Valid, "");
                return true;
            }
        }

        private bool ValidateVerifyPassword()
        {
            var passwordEmpty = string.IsNullOrEmpty(rad_PasswordAgain.Password);
            //tbVerifyPasswordWatermark.Opacity = passwordEmpty ? 100 : 0;
            //pbVerifyPassword.Opacity = passwordEmpty ? 0 : 100;
            if (passwordEmpty || rad_PasswordAgain.Password.Length < 6)
            {

                rad_PasswordAgain.ChangeValidationState(ValidationState.Invalid, "Mật khẩu ít nhất 6 kí tự !");
                return false;
            }
            else if (!rad_PasswordAgain.Password.Equals(rad_Password.Password))
            {
                rad_PasswordAgain.ChangeValidationState(ValidationState.Invalid, "Mật khẩu không trùng nhau !");
                return false;
            }

            else
            {

                rad_PasswordAgain.ChangeValidationState(ValidationState.Valid, "");
                return true;
            }
        }

        private bool ValidateName()
        {
            var NameEmpty = string.IsNullOrEmpty(rad_Name.Text);
            if (NameEmpty)
            {
                rad_Name.ChangeValidationState(ValidationState.Invalid, "Xin hãy nhập tên");
                return false;
            }
            else
            {
                rad_Name.ChangeValidationState(ValidationState.Valid, "");
                return true;
            }
        }

        private bool ValidateFirstAndMiddleName()
        {
            var FirstAndMiddleNameEmpty = string.IsNullOrEmpty(rad_FirstAndMiddleName.Text);
            if (FirstAndMiddleNameEmpty)
            {
                rad_FirstAndMiddleName.ChangeValidationState(ValidationState.Invalid, "Xin hãy nhập họ và tên đệm");
                return false;
            }
            else
            {
                rad_FirstAndMiddleName.ChangeValidationState(ValidationState.Valid, "");
                return true;
            }
        }


        private bool ValidatePhoneNumber()
        {
            var PhoneNumberEmpty = string.IsNullOrEmpty(rad_PhoneNumber.Text);
            if (PhoneNumberEmpty || rad_PhoneNumber.Text.Length != 10)
            {
                rad_PhoneNumber.ChangeValidationState(ValidationState.Invalid, "Số điện thoại gồm 10 số");
                return false;
            }
            else
            {
                rad_PhoneNumber.ChangeValidationState(ValidationState.Valid, "");
                return true;
            }
        }



        private void rad_Email_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateEmail();
        }

        private void rad_Password_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidatePassword();
        }

        private void rad_PasswordAgain_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateVerifyPassword();
        }

        private void rad_Name_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateName();
        }
        private void rad_FirstAndMiddleName_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateFirstAndMiddleName();
        }
        private void rad_PhoneNumber_LostFocus(object sender, RoutedEventArgs e)
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


        private void btn_Click_Register(object sender, RoutedEventArgs e)
        {
            if (ValidateEmail() && ValidatePassword() && ValidateVerifyPassword() && ValidateName() && ValidatePhoneNumber() && ValidateFirstAndMiddleName())
            {
                UserData ObjUserData = new UserData();
                ObjUserData.Email = rad_Email.Text;
                ObjUserData.Password = rad_Password.ToString();
                ObjUserData.FirstAndMiddleName = rad_FirstAndMiddleName.Text;
                ObjUserData.Name = rad_Name.Text;
                ObjUserData.PhoneNumber = rad_PhoneNumber.Text;
                objUserDataList.Add(ObjUserData);
                if (iSOFile.FileExists("RegistrationDetails"))
                {
                    iSOFile.DeleteFile("RegistrationDetails");
                }
                using (IsolatedStorageFileStream fileStream = iSOFile.OpenFile("RegistrationDetails", FileMode.Create))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(List<UserData>));

                    serializer.WriteObject(fileStream, objUserDataList);

                }
                MessageBox.Show("Đăng ký thành công");
                NavigationService.Navigate(new Uri("/Pages/Login.xaml", UriKind.Relative));
            }

        }
    }
}