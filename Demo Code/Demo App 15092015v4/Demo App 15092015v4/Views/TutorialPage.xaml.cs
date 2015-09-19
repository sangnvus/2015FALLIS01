using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Demo_App_15092015v4
{
    public partial class tutPage01 : PhoneApplicationPage
    {
        public tutPage01()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/FunctionPage.xaml", UriKind.Relative));
        }
    }
}