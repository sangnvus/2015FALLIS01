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


namespace FT_Rider.Pages
{
    public partial class RiderRegister : PhoneApplicationPage
    {
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        List<UserData> ObjUserDataList = new List<UserData>();
        public RiderRegister()
        {
            InitializeComponent();
            this.Loaded += RiderRegister_Loaded;
        }


        private void RiderRegister_Loaded(object sender, RoutedEventArgs e)
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

        private void txb_Tap_RegisterPayment(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/RiderRegisterpayment.xaml", UriKind.Relative));
        }

        private void btn_Tap_Cancel(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Login.xaml", UriKind.Relative));
        }


        //private void txt_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    TextBox tb = (TextBox)sender;
        //    tb.BorderBrush = new SolidColorBrush(Colors.LightGray);
        //}

        private void btn_Click_Register(object sender, RoutedEventArgs e)
        {          
            if (txt_Password.Text.Length < 6)
            {
                MessageBox.Show("Độ dài mật khẩu ít nhất 6 ký tự!");
            }
             else if (txt_PasswordAgain.Text.Length < 6)
             {
                 MessageBox.Show("Độ dài mật khẩu nhập lại ít nhất 6 ký tự!");
             }
             else if (!txt_Password.Text.ToString().Equals(txt_PasswordAgain.Text.ToString()))
             {
                 MessageBox.Show("Mật khẩu và mật khẩu nhập lại chưa chính xác");
             }
                  
            else if (txt_FirstAndMiddleName.Text == "")
            {
                MessageBox.Show("Vui lòng nhập họ và tên đệm!");
            }

            else if (txt_Name.Text == "")
            {
                MessageBox.Show("Vui lòng nhập tên!");
            }

            //Phone Number Length Validation
            else if (txt_PhoneNumber.Text.Length != 10)
            {
                MessageBox.Show("Số Đt không hợp lệ ");
            }

            //EmailID validation
            else if (!Regex.IsMatch(txt_Email.Text.Trim(), @"^([a-zA-Z_])([a-zA-Z0-9_\-\.]*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\])$"))
            {
                MessageBox.Show("Email không hợp lệ");
            }

           //After validation success ,store user detials in isolated storage
            else if (txt_Email.Text != "" && txt_Password.Text != "" && txt_Name.Text != "" && txt_FirstAndMiddleName.Text != "" && txt_PhoneNumber.Text != "")
            {
                UserData ObjUserData = new UserData();
                ObjUserData.Email = txt_Email.Text;
                ObjUserData.Password = txt_Password.Text;
                ObjUserData.FirstAndMiddleName = txt_FirstAndMiddleName.Text;
                ObjUserData.Name = txt_Name.Text;
                ObjUserData.PhoneNumber = txt_PhoneNumber.Text;
                int Temp = 0;
                foreach (var UserLogin in ObjUserDataList)
                {
                    if (ObjUserData.Email == UserLogin.Email)
                    {
                        Temp = 1;
                    }
                }
                //Checking existing user names in local DB
                if (Temp == 0)
                {
                    ObjUserDataList.Add(ObjUserData);
                    if (ISOFile.FileExists("RegistrationDetails"))
                    {
                        ISOFile.DeleteFile("RegistrationDetails");
                    }
                    using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("RegistrationDetails", FileMode.Create))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(List<UserData>));

                        serializer.WriteObject(fileStream, ObjUserDataList);

                    }
                    MessageBox.Show("Đăng ký thành công");
                    NavigationService.Navigate(new Uri("/Pages/Login.xaml", UriKind.Relative));
                }
                else
                {
                    MessageBox.Show("Xin lỗi, email bị trùng");
                }

            }
            else
            {
                MessageBox.Show("Vui lòng nhập thông tin");
            }
        }

        private void txt_Email_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            txt_Email.Text = String.Empty;
            txt_Email.Foreground = new SolidColorBrush(Colors.Black);
            txt_Email.BorderBrush.Opacity = 20;
        }

        private void txt_FirstAndMiddleName_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            txt_FirstAndMiddleName.Text = String.Empty;
            txt_FirstAndMiddleName.Foreground = new SolidColorBrush(Colors.Black);
            txt_FirstAndMiddleName.BorderBrush.Opacity = 20;
        }

        private void txt_Name_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            txt_Name.Text = String.Empty;
            txt_Name.Foreground = new SolidColorBrush(Colors.Black);
            txt_Name.BorderBrush.Opacity = 20;
        }

        private void txt_PhoneNumber_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            txt_PhoneNumber.Text = String.Empty;
            txt_PhoneNumber.Foreground = new SolidColorBrush(Colors.Black);
            txt_PhoneNumber.BorderBrush.Opacity = 20;
        }

        private void txt_Password_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            txt_Password.Text = String.Empty;
            txt_Password.Foreground = new SolidColorBrush(Colors.Black);
            txt_Password.BorderBrush.Opacity = 20;
        }

        private void txt_PasswordAgain_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            txt_PasswordAgain.Text = String.Empty;
            txt_PasswordAgain.Foreground = new SolidColorBrush(Colors.Black);
            txt_PasswordAgain.BorderBrush.Opacity = 20;
        }
        
    }
}