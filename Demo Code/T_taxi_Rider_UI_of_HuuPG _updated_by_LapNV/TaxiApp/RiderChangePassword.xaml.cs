using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace TaxiApp
{
    public partial class RiderChangePassword : PhoneApplicationPage
    {
        public RiderChangePassword()
        {
            InitializeComponent();
        }


        private void txb_Tap_Return(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/RiderProfile.xaml", UriKind.Relative));
        }

        private void txt_OldPassword_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            txt_OldPassword.Text = String.Empty;
        }

        private void txt_NewPassword_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            txt_NewPassword.Text = String.Empty;
        }

        private void txt_NewPasswordAgain_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            txt_NewPasswordAgain.Text = String.Empty;
        }

        
    }
}