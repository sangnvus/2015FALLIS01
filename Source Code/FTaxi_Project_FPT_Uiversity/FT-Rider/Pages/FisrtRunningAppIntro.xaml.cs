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
using System.Windows.Input;

namespace FT_Rider.Pages
{
    public partial class FisrtRunningAppIntro : PhoneApplicationPage
    {
        public FisrtRunningAppIntro()
        {
            InitializeComponent();
        }

        private void btn_Go_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Login.xaml", UriKind.Relative));
        }
    }
}