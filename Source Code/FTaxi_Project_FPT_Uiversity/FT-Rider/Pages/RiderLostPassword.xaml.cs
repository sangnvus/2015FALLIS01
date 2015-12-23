using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FT_Rider.Classes;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Windows.Media;

namespace FT_Rider.Pages
{
    public partial class RiderLostPassword : PhoneApplicationPage
    {
        public RiderLostPassword()
        {
            InitializeComponent();
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

        private void txt_Email_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void txt_Email_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateEmail();
        }

        private async void btn_ResetPassword_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            if (txt_Email.Text.Trim().Equals(String.Empty))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ email hợp lệ!");
                txt_Email.Focus();
            }
            else
            {
                //Show 
                ShowLoadingSreen();

                var email = txt_Email.Text;
                var input = string.Format("{{\"email\":\"{0}\"}}", email);
                try
                {
                    ///xem xem có lấy đc dữ liệu về ko?
                    ///
                    var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderResetPassword, input);
                    var resetStatus = JsonConvert.DeserializeObject<BaseResponse>(output);
                    if (resetStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS))//Code 0000
                    {
                        //Nếu ok
                        ///2. dis screen
                        ///3. thong bao
                        ///4. quay ve man hinh login
                        //

                        //2.
                        HideLoadingSreen();

                        //3.
                        MessageBox.Show(ConstantVariable.strResetPasswordSuccess);

                        //4.
                        NavigationService.Navigate(new Uri("/Pages/Login.xaml", UriKind.Relative));

                    }
                }
                catch (Exception)
                {

                    HideLoadingSreen();
                    MessageBox.Show("(Mã lỗi 3301) " + ConstantVariable.errServerErr); //Co loi may chu
                    Debug.WriteLine("Có lỗi fghn76 ở reset pass");
                }
            }
        }

        private void HideLoadingSreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Collapsed;
        }

        private void ShowLoadingSreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Visible;
        }

    }
}