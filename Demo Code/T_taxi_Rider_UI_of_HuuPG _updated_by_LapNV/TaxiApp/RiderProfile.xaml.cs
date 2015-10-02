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
using TaxiApp.Model;



namespace TaxiApp
{
    public partial class RiderProfile : PhoneApplicationPage

    {
         
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        UserData ObjUserData = new UserData();

        String[] Languages = { "Tiếng Việt", " Tiếng Anh" };
        public RiderProfile()
        {
            
            
            InitializeComponent();
            this.lpk_Language.ItemsSource = Languages;
          

            this.Loaded += RiderProfile_Loaded;
        }

        private void RiderProfile_Loaded(object sender, RoutedEventArgs e)
        {
            if (ISOFile.FileExists("CurrentLoginUserDetails"))//read current user login details    
            {
                using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("CurrentLoginUserDetails", FileMode.Open))
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(UserData));
                    ObjUserData = (UserData)serializer.ReadObject(fileStream);

                }
                StckUserDetailsUI.DataContext = ObjUserData;  
            }  
        }


        private void txb_Tap_Return(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void tbn__Tap_Logout(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LogOut();
        }

        public void LogOut()
        {
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                //set the properties
                Caption = "Thông báo",
                Message = "Bạn có muốn thoát không?",
                LeftButtonContent = "Có",
                RightButtonContent = "Không"
            };

            //Add the dismissed event handler
            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                         var Settings = IsolatedStorageSettings.ApplicationSettings;
                        Settings.Remove("CheckLogin");
                        NavigationService.Navigate(new Uri("/Login.xaml", UriKind.Relative));
                        break;
                    case CustomMessageBoxResult.RightButton:
                        //add the task you wish to perform when user clicks on no button here

                        break;
                    case CustomMessageBoxResult.None:
                        // Do something.
                        break;
                    default:
                        break;
                }
            };

            //add the show method
            messageBox.Show();

            
        }
        private void tbn__Tap_AddHomePlace(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/RiderAddHomePlace.xaml", UriKind.Relative));
        }

        private void tbn__Tap_AddWorkPlace(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/RiderAddWorkPlace.xaml", UriKind.Relative));
        }

        private void txb_Tap_ChangePassword(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/RiderChangePassword.xaml", UriKind.Relative));
        }

        private void txb_Tap_Edit(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        

        

      
    }
}