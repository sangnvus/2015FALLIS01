using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace FT_Driver.Pages
{
    public partial class DriverProfile : PhoneApplicationPage
    {
        public DriverProfile()
        {
            InitializeComponent();
        }

        private void tbl_Tap_ChangePassword(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/DriverChangePassword.xaml", UriKind.Relative));
        }

        private void tbn__Tap_AddWorkPlace(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/DriverAddWorkPlace.xaml", UriKind.Relative));
        }

        private void tbn__Tap_AddHomePlace(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/DriverAddHomePlace.xaml", UriKind.Relative));
        }
    }
}