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
using FT_Rider.Classes;

namespace FT_Rider.Pages
{
    public partial class RiderMyTrip : PhoneApplicationPage
    {
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        RiderLogin userData;

        public RiderMyTrip()
        {
            InitializeComponent();

            //Get User data from login
            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userData = new RiderLogin();
                userData = (RiderLogin)tNetUserLoginData["UserLoginData"];
            }

            //Load My trip
            GetMyTripData();
        }

        private void GetMyTripData()
        {

        }
    }
}