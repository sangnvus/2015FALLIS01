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
using FT_Rider.Classes;
using FT_Rider.Resources;
using Telerik.Windows.Controls.PhoneTextBox;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace FT_Rider.Pages
{
    public partial class Login : PhoneApplicationPage
    {
        IsolatedStorageFile iSOFile = IsolatedStorageFile.GetUserStoreForApplication();
        List<UserData> objUserDataList = new List<UserData>();

        private bool Validate(string text)
        {
            //Your validation logic
            return false;
        }

        public class Data
        {
            public string Name { get; set; }
        }

        public Login()
        {
            InitializeComponent();
            this.rad_Account.DataContext = new Data { Name = "Email" };
            this.rad_Password.DataContext = new Data { Name = "Passsword" };
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
                if (iSOFile.FileExists("RegistrationDetails"))//loaded previous items into list  
                {
                    using (IsolatedStorageFileStream fileStream = iSOFile.OpenFile("RegistrationDetails", FileMode.Open))
                    {
                        DataContractSerializer serializer = new DataContractSerializer(typeof(List<UserData>));
                        objUserDataList = (List<UserData>)serializer.ReadObject(fileStream);

                    }
                }
            }
        }

        

        private void tbn_Tap_Login(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (rad_Account.Text != "" && rad_Password.ToString() != "")
            {
                int Temp = 0;
                foreach (var UserLogin in objUserDataList)
                {
                    if (rad_Account.Text == UserLogin.Email && rad_Password.ToString() == UserLogin.Password)
                    {
                        Temp = 1;
                        var Settings = IsolatedStorageSettings.ApplicationSettings;
                        Settings["CheckLogin"] = StaticVariables.strLoginSucess;//write iso    

                        if (iSOFile.FileExists("CurrentLoginUserDetails"))
                        {
                            iSOFile.DeleteFile("CurrentLoginUserDetails");
                        }
                        using (IsolatedStorageFileStream fileStream = iSOFile.OpenFile("CurrentLoginUserDetails", FileMode.Create))
                        {
                            DataContractSerializer serializer = new DataContractSerializer(typeof(UserData));

                            serializer.WriteObject(fileStream, UserLogin);

                        }
                        NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
                    }
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
            NavigationService.Navigate(new Uri("/Pages/RiderRegister.xaml", UriKind.Relative));
        }

        private void tbl_Tap_LostPassword(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/RiderLostPassword.xaml", UriKind.Relative));
        }


        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
        }


        private async void sendJsonRequest()
        {
            string URL = StaticVariables.tNetRiderLoginAddress; //"http://123.30.236.109:8088/TN/restServices/RiderController/LoginRider"

            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("json", "{\"uid\":\"apl.ytb2@gmail.com\",\"pw\":\"Abc123!\",\"mid\":\"\",\"mType\":\"AND\"}");


            HttpClient client = new HttpClient();
            HttpContent contents = new FormUrlEncodedContent(parameter);
            var response = await client.PostAsync(new Uri(URL), contents);
            var reply = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                //JObject jResult = JObject.Parse(response.Content.ReadAsStringAsync().Result.ToString());
                RiderLogin riderLogin = new RiderLogin();
                riderLogin = JsonConvert.DeserializeObject<RiderLogin>(response.Content.ReadAsStringAsync().Result);
                //MessageBox.Show(riderLogin.content.email);

                string json = JsonConvert.SerializeObject(riderLogin);
                MessageBox.Show(json);
            }
        }

        private void btn_t1_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            sendJsonRequest();
        }
    }
}