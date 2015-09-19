using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;

namespace Demo_App_15092015v4
{
    public partial class tutPage01 : PhoneApplicationPage
    {
        public tutPage01()
        {
            InitializeComponent();
            
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(2000);
            NavigationService.Navigate(new Uri("/Views/FunctionPage.xaml", UriKind.Relative));
        }
    }
}