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
using TaxiApp.Resources;
using TaxiApp.Model;

namespace TaxiApp
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

        

        private void btn_Login(object sender, RoutedEventArgs e)
        {
            if (txt_Account.Text != "" && pwb_Password.Password != "")
            {
                int Temp = 0;
                foreach (var UserLogin in ObjUserDataList)
                {
                    if (txt_Account.Text == UserLogin.Email && pwb_Password.Password == UserLogin.Password)
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
                        NavigationService.Navigate(new Uri("/RiderProfile.xaml", UriKind.Relative));
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
      

        private void txb_Account_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            txt_Account.Text = String.Empty; //Xóa text khi chọn vào
            txt_Account.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void txb_Password_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {            
        }

        private void btn_Signup(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/RiderRegister.xaml", UriKind.Relative));
        }

        private void txb_ChangePassword_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            NavigationService.Navigate(new Uri("/RiderLostPassword.xaml", UriKind.Relative));
        }
    }
}