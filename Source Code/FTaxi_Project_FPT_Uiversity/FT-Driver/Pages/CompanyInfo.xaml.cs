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
    public partial class CompanyInfo : PhoneApplicationPage
    {
        public CompanyInfo()
        {
            InitializeComponent();
        }

        private void btn_Tap_Comback(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/DriverProfile.xaml", UriKind.Relative));
        }
    }
}