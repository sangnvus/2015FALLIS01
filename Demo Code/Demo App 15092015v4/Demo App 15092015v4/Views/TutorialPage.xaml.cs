using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
<<<<<<< HEAD
using System.Threading.Tasks;
=======
>>>>>>> 8cb4a5fc0f0fbe698df4b934e6bda39f9e807da4

namespace Demo_App_15092015v4
{
    public partial class tutPage01 : PhoneApplicationPage
    {
        public tutPage01()
        {
            InitializeComponent();
<<<<<<< HEAD
            
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(2000);
=======
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
>>>>>>> 8cb4a5fc0f0fbe698df4b934e6bda39f9e807da4
            NavigationService.Navigate(new Uri("/Views/FunctionPage.xaml", UriKind.Relative));
        }
    }
}