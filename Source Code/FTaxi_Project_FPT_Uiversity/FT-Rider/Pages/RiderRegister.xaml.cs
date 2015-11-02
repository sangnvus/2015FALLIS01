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
             else if (!Password.ToString().Equals(PasswordAgain.ToString()))
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
            else if (txt_Email.Text != "" && Password.ToString() != "" && txt_Name.Text != "" && txt_FirstAndMiddleName.Text != "" && txt_PhoneNumber.Text != "")
            {
                UserData ObjUserData = new UserData();
                ObjUserData.Email = txt_Email.Text;
                ObjUserData.Password = Password.ToString();
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

        private void txt_FirstAndMiddleName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txt_FirstAndMiddleName.Text == "Họ")
            {
                txt_FirstAndMiddleName.Text = "";
                SolidColorBrush Brush3 = new SolidColorBrush();
                Brush3.Color = Colors.Black;
                txt_FirstAndMiddleName.Foreground = Brush3;
            }
        }

        private void txt_FirstAndMiddleName_LostFocus(object sender, RoutedEventArgs e)
        {
              if (txt_FirstAndMiddleName.Text == String.Empty)
            {
                txt_FirstAndMiddleName.Text = "Họ";
                SolidColorBrush Brush4 = new SolidColorBrush();
                Brush4.Color = Colors.Gray;
                txt_FirstAndMiddleName.Foreground = Brush4;
            }
        }

        private void txt_Email_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txt_Email.Text == "Điền Email mới tại đây")
            {
                txt_Email.Text = "";
                SolidColorBrush Brush3 = new SolidColorBrush();
                Brush3.Color = Colors.Black;
                txt_Email.Foreground = Brush3;
            }
        }

        private void txt_Email_LostFocus(object sender, RoutedEventArgs e)
        {
             if (txt_Email.Text == String.Empty)
            {
                txt_Email.Text = "Điền Email mới tại đây";
                SolidColorBrush Brush4 = new SolidColorBrush();
                Brush4.Color = Colors.Gray;
                txt_Email.Foreground = Brush4;
            }
        }

        private void txt_PasswordAgain_GotFocus(object sender, RoutedEventArgs e)
        {
             if (txt_PasswordAgain.Text == "Điền lại mật khẩu mới tại đây")
            {
                txt_PasswordAgain.Text = "";
                SolidColorBrush Brush3 = new SolidColorBrush();
                Brush3.Color = Colors.Black;
                txt_PasswordAgain.Foreground = Brush3;
            }
        }

        
          private void txt_PasswordAgain_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt_PasswordAgain.Text == String.Empty)
            {
                txt_PasswordAgain.Text = "Điền lại mật khẩu mới tại đây";
                SolidColorBrush Brush4 = new SolidColorBrush();
                Brush4.Color = Colors.Gray;
                txt_PasswordAgain.Foreground = Brush4;
            }
        }

        private void txt_Name_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txt_Name.Text == "Tên")
            {
                txt_Name.Text = "";
                SolidColorBrush Brush3 = new SolidColorBrush();
                Brush3.Color = Colors.Black;
                txt_Name.Foreground = Brush3;
            }
        }

        private void txt_Name_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt_Name.Text == String.Empty)
            {
                txt_Name.Text = "Tên";
                SolidColorBrush Brush4 = new SolidColorBrush();
                Brush4.Color = Colors.Gray;
                txt_Name.Foreground = Brush4;
            }
        }


        private void txt_PhoneNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txt_PhoneNumber.Text == "Số điện thoại")
            {
                txt_PhoneNumber.Text = "";
                SolidColorBrush Brush3 = new SolidColorBrush();
                Brush3.Color = Colors.Black;
                txt_PhoneNumber.Foreground = Brush3;
            }
        }

        private void txt_PhoneNumber_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txt_PhoneNumber.Text == String.Empty)
            {
                txt_PhoneNumber.Text = "Số điện thoại";
                SolidColorBrush Brush4 = new SolidColorBrush();
                Brush4.Color = Colors.Gray;
                txt_PhoneNumber.Foreground = Brush4;
            }
        }

        private void PwLostFocus(object sender, RoutedEventArgs e)
        {
            CheckPasswordWatermark();
        }
        public void CheckPasswordWatermark()
        {
            var passwordEmpty = string.IsNullOrEmpty(Password.Password);
            txt_Password.Opacity = passwordEmpty ? 100 : 0;
            Password.Opacity = passwordEmpty ? 0 : 100;
        }
        private void PwGotFocus(object sender, RoutedEventArgs e)
        {
            txt_Password.Opacity = 0;
            Password.Opacity = 100;
        }



        private void PwAgainLostFocus(object sender, RoutedEventArgs e)
        {
            CheckPasswordAgainWatermark();
        }

        public void CheckPasswordAgainWatermark()
        {
            var passwordEmpty = string.IsNullOrEmpty(PasswordAgain.Password);
            txt_PasswordAgain.Opacity = passwordEmpty ? 100 : 0;
            PasswordAgain.Opacity = passwordEmpty ? 0 : 100;
        }
       
        private void PwAgainGotFocus(object sender, RoutedEventArgs e)
        {
            txt_PasswordAgain.Opacity = 0;
            PasswordAgain.Opacity = 100;
        }

        

   

        
      
    }
}