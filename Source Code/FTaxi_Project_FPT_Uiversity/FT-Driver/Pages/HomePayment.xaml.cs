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
using FT_Driver.Classes;

namespace FT_Driver.Pages
{
    public partial class HomePayment : PhoneApplicationPage
    {
        IsolatedStorageSettings tNetTripData = IsolatedStorageSettings.ApplicationSettings;
        DriverCompleteTrip completeTrip;
        
        public HomePayment()
        {
            InitializeComponent();
            if (tNetTripData.Contains("CompleteTripBill"))
            {
                completeTrip = new DriverCompleteTrip();
                completeTrip = (DriverCompleteTrip)tNetTripData["CompleteTripBill"];
            }
        }

        

        private void btn_Payment_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
            //Xóa dữ liệu
        }
    }
}