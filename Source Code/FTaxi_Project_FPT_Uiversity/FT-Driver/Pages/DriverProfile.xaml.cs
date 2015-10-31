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
         
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        UserData ObjUserData = new UserData();

        
        public DriverProfile()
        {
            
            
            InitializeComponent();
            lpk_Language.SetValue(Microsoft.Phone.Controls.ListPicker.ItemCountThresholdProperty, 3);
            List<Language> source = new List<Language>();
            source.Add(new Language() { Name = "Tiếng Việt", Logo = "VI" });
            source.Add(new Language() { Name = "English", Logo = "EN" });
            this.lpk_Language.ItemsSource = source;
          

            this.Loaded += DriverProfile_Loaded;
        }

        private void DriverProfile_Loaded(object sender, RoutedEventArgs e)
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


        

        private void btn__Tap_Logout(object sender, System.Windows.Input.GestureEventArgs e)
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
                        NavigationService.Navigate(new Uri("/Pages/Login.xaml", UriKind.Relative));
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
            NavigationService.Navigate(new Uri("/Pages/DriverAddWorkPlace.xaml", UriKind.Relative));
        }



        

        

      
    }
}