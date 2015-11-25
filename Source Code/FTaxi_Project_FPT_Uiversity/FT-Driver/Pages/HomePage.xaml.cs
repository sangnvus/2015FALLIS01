using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Controls;
using System.Device.Location;
using Windows.Devices.Geolocation;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Devices;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using FT_Driver.Resources;
using FT_Driver.Classes;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading;

namespace FT_Driver.Pages
{
    public partial class HomePage : PhoneApplicationPage
    {
        //For Store Points
        List<GeoCoordinate> driverCoordinates = new List<GeoCoordinate>();

        //For Router        
        GeocodeQuery driverGeocodeQuery = null;
        RouteQuery driverQuery = null;
        MapRoute driverMapRoute = null;
        Route driverRoute = null;

        //For map layer
        MapLayer driverMapLayer;

        //VibrateController
        VibrateController vibrateController = VibrateController.Default;

        //For Distance
        Double distanceMeter;

        //Rider Destination Icon Overlay
        MapOverlay driverDestinationIconOverlay;

        //USER DATA
        DriverLogin userData = PhoneApplicationService.Current.State["UserInfo"] as DriverLogin;
               


        //For menu
        double initialPosition;
        bool _viewMoved = false;

        public HomePage()
        {
            InitializeComponent();
            //get First Local Position
            ShowCurrentLocalOnTheMap();
            LoadDriverProfile();
        }


        //------ BEGIN get driver profile ------//
        private void LoadDriverProfile()
        {
            tbl_FirstName.Text = userData.content.driverInfo.fName;
            tbl_LastName.Text = userData.content.driverInfo.lName;
        }
        //------ END get driver profile ------//



        //------ BEGIN get current Position ------//
        private async void ShowCurrentLocalOnTheMap()
        {
       
            //get position
            Geolocator driverFirstGeolocator = new Geolocator();
            Geoposition driverFirstGeoposition = await driverFirstGeolocator.GetGeopositionAsync();
            Geocoordinate driverFirstGeocoordinate = driverFirstGeoposition.Coordinate;
            GeoCoordinate driverFirstGeoCoordinate = ConvertData.ConvertGeocoordinate(driverFirstGeocoordinate);


            //Adjust map on the phone screen - 0.001500 to move up the map
            this.map_DriverMap.Center = new GeoCoordinate(driverFirstGeoposition.Coordinate.Latitude - 0.001500, driverFirstGeoposition.Coordinate.Longitude);
            this.map_DriverMap.ZoomLevel = 16;

            //Show maker

            // Create a small circle to mark the current location.
            Ellipse firstDriverPositionIcon = new Ellipse();
            firstDriverPositionIcon.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)42, (byte)165, (byte)255)); //RGB of #2aa5ff
            firstDriverPositionIcon.Height = 15;
            firstDriverPositionIcon.Width = 15;
            firstDriverPositionIcon.Opacity = 100;

            // Create a MapOverlay to contain the circle.
            MapOverlay firstDriverLocationOverlay = new MapOverlay();
            firstDriverLocationOverlay.Content = firstDriverPositionIcon;

            //MapOverlay PositionOrigin to 0.9, 0. MapOverlay will align it's center towards the GeoCoordinate
            firstDriverLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
            firstDriverLocationOverlay.GeoCoordinate = driverFirstGeoCoordinate;

            // Create a MapLayer to contain the MapOverlay.
            driverMapLayer = new MapLayer();
            driverMapLayer.Add(firstDriverLocationOverlay);

            // Add the MapLayer to the Map.
            map_DriverMap.Layers.Add(driverMapLayer);

        }
        //------ END get current Position ------//






        //------ BEGIN route Direction on Map ------//
        private async void getMapRouteTo(double lat, double lng)
        {
            //driverCoordinates.RemoveAll(item => item == null);
            //Delete Previous Route if exist
            if (driverMapRoute != null)
            {
                //delete route
                map_DriverMap.RemoveRoute(driverMapRoute);
                driverMapRoute = null;
                driverQuery = null;                
                driverMapLayer.Remove(driverDestinationIconOverlay);
            }

            Geolocator driverGeolocator = new Geolocator();
            driverGeolocator.DesiredAccuracyInMeters = 5;
            Geoposition driverGeoPosition = null;
            try
            {
                //Set Position point
                driverGeoPosition = await driverGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
                driverCoordinates.Add(new GeoCoordinate(driverGeoPosition.Coordinate.Latitude, driverGeoPosition.Coordinate.Longitude));
            }
            catch (UnauthorizedAccessException)
            {
                //Dịch vụ định vị đang tắt, vui lòng bật lên hoặc kiểm tra lại các thiết đặt.
                MessageBox.Show(ConstantVariable.errServiceIsOff);
            }
            catch (Exception ex)
            {
                // Something else happened while acquiring the location.
                MessageBox.Show(ex.Message);
            }

            driverGeocodeQuery = new GeocodeQuery();
            driverGeocodeQuery.SearchTerm = lat.ToString().Replace(',', '.') + "," + lng.ToString().Replace(',', '.');
            driverGeocodeQuery.GeoCoordinate = new GeoCoordinate(driverGeoPosition.Coordinate.Latitude, driverGeoPosition.Coordinate.Longitude);


            driverGeocodeQuery.QueryCompleted += Mygeocodequery_QueryCompleted;
            driverGeocodeQuery.QueryAsync();
        }


        private void Mygeocodequery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Error == null)
            {
                try
                {
                    driverQuery = new RouteQuery();
                    driverCoordinates.Add(e.Result[0].GeoCoordinate);
                    driverQuery.Waypoints = driverCoordinates;
                    driverQuery.QueryCompleted += MyQuery_QueryCompleted;
                    driverQuery.QueryAsync();
                    driverGeocodeQuery.Dispose();
                }
                catch (Exception)
                {

                    MessageBox.Show(ConstantVariable.errInvalidAddress);
                }
            }
        }



        private void MyQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {

            //if valid address input
            if (e.Error == null)
            {
                //if (driverMapRoute != null)
                //{
                //    map_DriverMap.RemoveRoute(driverMapRoute);
                //    driverMapLayer.Remove(driverDestinationIconOverlay);
                //    driverMapRoute = null;
                //}                
                driverRoute = e.Result;
                driverMapRoute = new MapRoute(driverRoute);
                //Makeup for router
                driverMapRoute.Color = Color.FromArgb(255, (byte)185, (byte)207, (byte)231); // aRGB for #b9cfe7
                map_DriverMap.AddRoute(driverMapRoute);
                driverQuery.Dispose();

                //get Coordinate of Destination Point
                double destinationLatitude = driverCoordinates[driverCoordinates.Count - 1].Latitude;
                double destinationLongtitude = driverCoordinates[driverCoordinates.Count - 1].Longitude;

                //Set Map Center
                this.map_DriverMap.Center = new GeoCoordinate(destinationLatitude - 0.001500, destinationLongtitude);

                // Create a small Point to mark the current location.
                Image myPositionIcon = new Image();
                myPositionIcon.Source = new BitmapImage(new Uri("/Images/Icons/img_DestinationPoint.png", UriKind.Relative));
                myPositionIcon.Height = 35;
                myPositionIcon.Width = 29;

                // Create a MapOverlay to contain the circle.
                driverDestinationIconOverlay = new MapOverlay();
                driverDestinationIconOverlay.Content = myPositionIcon;

                //MapOverlay PositionOrigin to 0.3, 0.9 MapOverlay will align it's center towards the GeoCoordinate
                driverDestinationIconOverlay.PositionOrigin = new Point(0.3, 0.9);
                driverDestinationIconOverlay.GeoCoordinate = new GeoCoordinate(destinationLatitude, destinationLongtitude);

                // Create a MapLayer to contain the MapOverlay.
                driverMapLayer = new MapLayer();
                driverMapLayer.Add(driverDestinationIconOverlay);

                // Add the MapLayer to the Map.
                map_DriverMap.Layers.Add(driverMapLayer);

                //Calculate Distance
                distanceMeter = Math.Round(GetTotalDistance(driverCoordinates), 0); //Round double in zero decimal places
            }
            else
            {
                MessageBox.Show(ConstantVariable.errInvalidAddress);
            }
        }
        //------ END route Direction on Map ------//



        //private void getRouteTo(GeoCoordinate myPosition, GeoCoordinate destination)
        //{
        //    if (driverMapRoute != null)
        //    {
        //        map_DriverMap.RemoveRoute(driverMapRoute);
        //        driverMapRoute = null;
        //        driverQuery = null;
        //    }
        //    driverQuery = new RouteQuery()
        //    {
        //        TravelMode = TravelMode.Driving,
        //        Waypoints = new List<GeoCoordinate>()
        //    {
        //        myPosition, 
        //        destination
        //    },
        //        RouteOptimization = RouteOptimization.MinimizeTime
        //    };
        //    driverQuery.QueryCompleted += driverRouteQuery_QueryCompleted;
        //    driverQuery.QueryAsync();
        //}
        //void driverRouteQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        //{
        //    if (e.Error == null)
        //    {
        //        Route newRoute = e.Result;

        //        driverMapRoute = new MapRoute(newRoute);
        //        map_DriverMap.AddRoute(driverMapRoute);
        //        driverQuery.Dispose();
        //    }
        //}







        //------ BEGIN calculate Distance ------//
        private static double GetTotalDistance(IEnumerable<GeoCoordinate> coordinates)
        {
            double result = 0;

            if (coordinates.Count() > 1)
            {
                GeoCoordinate previous = coordinates.First();

                foreach (var current in coordinates)
                {
                    result += previous.GetDistanceTo(current);
                }
            }

            return result;
        }
        //------ END calculate Distance ------//





        //------ BEGIN show and Design UI 3 taxi near current position ------//
        private void ShowNearDrivers(double lat, double lng, string tName)
        {
            GeoCoordinate TaxiCoordinate = new GeoCoordinate(lat, lng);

            //Create taxi icon on map
            Image taxiIcon = new Image();
            taxiIcon.Source = new BitmapImage(new Uri("/Images/Taxis/img_CarIcon.png", UriKind.Relative));

            //Add a tapped event
            taxiIcon.Tap += taxiIcon_Tap;

            //Create Taxi Name 
            TextBlock taxiName = new TextBlock();
            taxiName.HorizontalAlignment = HorizontalAlignment.Center;
            taxiName.Text = tName;
            taxiName.FontSize = 12;
            taxiName.Foreground = new SolidColorBrush(Color.FromArgb(255, (byte)46, (byte)159, (byte)255)); //RBG color for #2e9fff

            //Create Stack Panel to group icon, taxi name, ...            
            Rectangle taxiNameBackground = new Rectangle();
            taxiNameBackground.Height = 18;
            taxiNameBackground.Width = taxiName.ToString().Length + 20;
            taxiNameBackground.RadiusX = 9;
            taxiNameBackground.RadiusY = 7;
            //taxiNameBackground.Stroke = new SolidColorBrush(Color.FromArgb(255, (byte)171, (byte)171, (byte)171)); //RBG color for #ababab
            taxiNameBackground.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)213, (byte)235, (byte)255)); //RBG color for #d5ebff

            Grid taxiNameGrid = new Grid();
            taxiNameGrid.Margin = new Thickness(0, 4, 0, 4); //Margin Top and Bottom 4px
            taxiNameGrid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            taxiNameGrid.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            taxiNameGrid.Children.Add(taxiNameBackground);
            taxiNameGrid.Children.Add(taxiName);

            StackPanel taxiStackPanel = new StackPanel();
            //taxiStackPanel.Margin  = new Thickness(5, 0, 5, 0);
            taxiStackPanel.Children.Add(taxiIcon);
            taxiStackPanel.Children.Add(taxiNameGrid);

            // Create a MapOverlay to contain the circle.
            MapOverlay myTaxiOvelay = new MapOverlay();
            //myTaxiOvelay.Content = myCircle;
            myTaxiOvelay.Content = taxiStackPanel;
            myTaxiOvelay.PositionOrigin = new Point(0.5, 0.5);
            myTaxiOvelay.GeoCoordinate = TaxiCoordinate;

            //Add to Map's Layer
            driverMapLayer = new MapLayer();
            driverMapLayer.Add(myTaxiOvelay);

            map_DriverMap.Layers.Add(driverMapLayer);
        }

        //Tapped event
        private void taxiIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
        }
        //------ END show and Design UI 3 taxi near current position ------//



        //------ BEGIN For open menu ------//
        private void btn_OpenMenu_Click(object sender, RoutedEventArgs e)
        {

            var left = Canvas.GetLeft(LayoutRoot);
            if (left > -100)
            {
                MoveViewWindow(-420);
            }
            else
            {
                MoveViewWindow(0);
            }
        }
        void MoveViewWindow(double left)
        {
            _viewMoved = true;
            ((Storyboard)canvas.Resources["moveAnimation"]).SkipToFill();
            ((DoubleAnimation)((Storyboard)canvas.Resources["moveAnimation"]).Children[0]).To = left;
            ((Storyboard)canvas.Resources["moveAnimation"]).Begin();
        }


        private void canvas_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            //if (e.DeltaManipulation.Translation.X != 0)
            //    Canvas.SetLeft(LayoutRoot, Math.Min(Math.Max(-840, Canvas.GetLeft(LayoutRoot) + e.DeltaManipulation.Translation.X), 0));
        }

        private void canvas_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            _viewMoved = false;
            initialPosition = Canvas.GetLeft(LayoutRoot);
        }

        private void canvas_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            var left = Canvas.GetLeft(LayoutRoot);
            if (_viewMoved)
                return;
            if (Math.Abs(initialPosition - left) < 100)
            {
                //bouncing back
                MoveViewWindow(initialPosition);
                return;
            }
            //change of state
            if (initialPosition - left > 0)
            {
                //slide to the left
                if (initialPosition > -420)
                    MoveViewWindow(-420);
                else
                    MoveViewWindow(-840);
            }
            else
            {
                //slide to the right
                if (initialPosition < -420)
                    MoveViewWindow(-420);
                else
                    MoveViewWindow(0);
            }
        }
        //------ END For open menu ------//





        //------ BEGIN Convert Lat & Lng from Address for Bing map Input ------//
        private void searchCoordinateFromAddress(string inputAddress)
        {
            //GoogleAPIGeocoding URL
            string URL = ConstantVariable.googleAPIGeocodingRequestsBaseURI + inputAddress + "&key=" + ConstantVariable.googleGeolocationAPIkey;

            //Query Autocomplete Responses to a JSON String
            WebClient proxy = new WebClient();
            proxy.DownloadStringCompleted +=
            new DownloadStringCompletedEventHandler(proxy_searchCoordinateFromAddress);
            proxy.DownloadStringAsync(new Uri(URL));
        }
        private void proxy_searchCoordinateFromAddress(object sender, DownloadStringCompletedEventArgs e)
        {
            //1. Convert Json String to an Object
            GoogleAPIGeocoding places = new GoogleAPIGeocoding();
            places = JsonConvert.DeserializeObject<GoogleAPIGeocoding>(e.Result);
            try
            {
                double lat = places.results[0].geometry.location.lat;
                double lng = places.results[0].geometry.location.lng;

                //route direction on map
                this.getMapRouteTo(lat, lng);
            }
            catch (Exception)
            {

                MessageBox.Show(ConstantVariable.errInvalidAddress);
            }
        }

        //------ END Convert Lat & Lng from Address for Bing map Input ------//


        private void map_DriverMap_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "5fcbf5e6-e6d0-48d7-a69d-8699df1b5318";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "I5nG-B7z5bxyTGww1PApXA";
        }

    }

}