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
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FT_Driver.Classes;
using FT_Driver.Resources;
using Telerik.Windows.Controls.PhoneTextBox;

namespace FT_Driver.Pages
{
    public partial class Login : PhoneApplicationPage
    {
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        List<UserData> ObjUserDataList = new List<UserData>();
        public Login()
        {
            InitializeComponent();
            this.rad_Account.DataContext = new Data { Name = "Email" };
            this.rad_Password.DataContext = new Data { Name = "Passsword" };
            this.Loaded += Login_Loaded;
        }

        private bool Validate(string text)
        {
            //Your validation logic
            return false;
        }



        public class Data
        {
            public string Name { get; set; }
        }


        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
            var Settings = IsolatedStorageSettings.ApplicationSettings;
            //Check if user already login,so we need to direclty navigate to details page instead of showing login page when user launch the app.  
            if (Settings.Contains("CheckLogin"))
            {
                NavigationService.Navigate(new Uri("DriverProfile.xaml", UriKind.Relative));
            }
            else
            {
                if (ISOFile.FileExists("RegistrationDetails"))//loaded previous items into list  
                {
                    using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("RegistrationDetails", FileMode.Open))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(List<UserData>));
                        ObjUserDataList = (List<UserData>)serializer.ReadObject(fileStream);

                    }
                }
            }
        }



        private void tbn_Tap_Login(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (rad_Account.Text != "" && rad_Password.ToString() != "")
            {
                int Temp = 0;
               
                    if (rad_Account.Text == "admin@gmail.com" && rad_Password.Password == "admin")
                    {
                        Temp = 1;
                        NavigationService.Navigate(new Uri("/Pages/DriverProfile.xaml", UriKind.Relative));
                    
                }
                if (Temp == 0)
                {
                    rad_Password.ChangeValidationState(ValidationState.Invalid, "");
                    rad_Account.ChangeValidationState(ValidationState.Invalid, "");
                }
            }
            else
            {
                rad_Password.ChangeValidationState(ValidationState.Invalid, "");
                rad_Account.ChangeValidationState(ValidationState.Invalid, "");
            }

        }

        private void tbn_Tap_Register(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/DriverRegister.xaml", UriKind.Relative));
        }

        private void tbl_Tap_LostPassword(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/DriverLostPassword.xaml", UriKind.Relative));
        }


        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
        }






    }
}