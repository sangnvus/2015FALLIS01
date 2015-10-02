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
    public partial class RiderAddHomePlace : PhoneApplicationPage
    {
        public RiderAddHomePlace()
        {
            InitializeComponent();
        }


        private void txb_Tap_Return(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/RiderProfile.xaml", UriKind.Relative));
        }

        private void txt_Address_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            txt_Address.Text = String.Empty;
        }

        private void txt_State_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            txt_State.Text = String.Empty;
        }

        private void txt_City_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            txt_City.Text = String.Empty;
        }
    }
}