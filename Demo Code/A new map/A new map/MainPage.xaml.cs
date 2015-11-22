using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using A_new_map.Resources;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using System.IO.IsolatedStorage;
using System.Device.Location;
using System.Windows.Shapes;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Media;
using Microsoft.Phone.Maps.Toolkit;

namespace A_new_map
{
    public partial class MainPage : PhoneApplicationPage
    {
        Geolocator myGeolocator = null;
        bool tracking = false;

        public MainPage()
        {
            InitializeComponent();
            this.GetCurrentCoordinate();

        }


        private void GetCurrentCoordinate()
        {
            if (!tracking)
            {
                myGeolocator = new Geolocator();
                myGeolocator.DesiredAccuracy = PositionAccuracy.High;
                myGeolocator.ReportInterval = 100; //milisec
                myGeolocator.MovementThreshold = 10; // The units are meters.

                myGeolocator.PositionChanged += geolocator_PositionChanged;

                tracking = true;

            }
            else
            {
                myGeolocator.PositionChanged -= geolocator_PositionChanged;
                myGeolocator = null;

                tracking = false;

            }
        }


        void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Dispatcher.BeginInvoke(() =>
            {

                Geocoordinate myGeocoordinate = args.Position.Coordinate;
                myMap.SetView(myGeocoordinate.ToGeoCoordinate(), 16, MapAnimationKind.Linear);

              
                UserLocationMarker newPin = (UserLocationMarker)this.FindName("UserLocationMarker");
                newPin.GeoCoordinate = myGeocoordinate.ToGeoCoordinate();

             
            });
        }


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                // User has opted in or out of Location
                return;
            }
            else
            {
                MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location. Is that ok?",
                    "Location",
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }

        private void myMap_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "ApJsz76A5DIw8fg7oU2F1nI2wO2p7xssazIh1SdqA_LcZskorBXeV9YEYdcG3GqI";
        }

    }
}