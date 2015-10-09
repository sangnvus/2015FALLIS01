﻿using System;
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
using FT_Rider.Classes;
using FT_Rider.Resources;

namespace FT_Rider.Pages
{
    public partial class Login : PhoneApplicationPage
    {
        IsolatedStorageFile ISOFile = IsolatedStorageFile.GetUserStoreForApplication();
        List<UserData> ObjUserDataList = new List<UserData>();
        public Login()
        {
            InitializeComponent();
            this.Loaded += Login_Loaded;
        }

        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
            var Settings = IsolatedStorageSettings.ApplicationSettings;
            //Check if user already login,so we need to direclty navigate to details page instead of showing login page when user launch the app.  
            if (Settings.Contains("CheckLogin"))
            {
                NavigationService.Navigate(new Uri("RiderProfile.xaml", UriKind.Relative));
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

        private void tbl_Tap_Register(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/RiderRegister.xaml", UriKind.Relative));
        }

        private void tbn_Tap_Login(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (txt_Account.Text != "" && txt_Password.Text != "")
            {
                int Temp = 0;
                foreach (var UserLogin in ObjUserDataList)
                {
                    if (txt_Account.Text == UserLogin.Email && txt_Password.Text == UserLogin.Password)
                    {
                        Temp = 1;
                        var Settings = IsolatedStorageSettings.ApplicationSettings;
                        Settings["CheckLogin"] = "Login sucess";//write iso    

                        if (ISOFile.FileExists("CurrentLoginUserDetails"))
                        {
                            ISOFile.DeleteFile("CurrentLoginUserDetails");
                        }
                        using (IsolatedStorageFileStream fileStream = ISOFile.OpenFile("CurrentLoginUserDetails", FileMode.Create))
                        {
                            DataContractSerializer serializer = new DataContractSerializer(typeof(UserData));

                            serializer.WriteObject(fileStream, UserLogin);

                        }
                        NavigationService.Navigate(new Uri("/Pages/RiderProfile.xaml", UriKind.Relative));
                    }
                }
                if (Temp == 0)
                {
                    MessageBox.Show("Tài khoản hoặc mật khẩu không đúng, xin vui lòng nhập lại");
                }
            }
            else
            {
                MessageBox.Show("Xin vui lòng nhập tài khoản và mật khẩu");
            }    
           
        }
    }
}