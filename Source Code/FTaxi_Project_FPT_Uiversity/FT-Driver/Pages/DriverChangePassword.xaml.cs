using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace FT_Driver.Pages
{
    public partial class DriverChangePassword : PhoneApplicationPage
    {
        public DriverChangePassword()
        {
            InitializeComponent();
        }

        private void txt_OldPassword_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            txt_OldPassword.Text = String.Empty;
            txt_OldPassword.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void txt_NewPassword_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            txt_NewPassword.Text = String.Empty;
            txt_NewPassword.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void txt_NewPassWordAgain_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            txt_NewPassWordAgain.Text = String.Empty;
            txt_NewPassWordAgain.Foreground = new SolidColorBrush(Colors.Black);
        }






    }
}