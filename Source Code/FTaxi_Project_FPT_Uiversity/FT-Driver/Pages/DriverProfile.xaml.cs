using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using FT_Driver.Classes;
using ListPickerDemo;



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

        private void img_EditIcon_HomeAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/DriverAddHomePlace.xaml", UriKind.Relative));
        }

        private void img_EditIcon_OfficeAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/CompanyInfo.xaml", UriKind.Relative));
        }
      
    }
}