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
using FT_Rider.Resources;
using FT_Rider.Classes;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace FT_Rider.Pages
{
    public partial class HomePage : PhoneApplicationPage
    {
        //For Store Points
        List<GeoCoordinate> MyCoordinates = new List<GeoCoordinate>();

        //For Position
        Geolocator MyGeolocator = new Geolocator();
        Geoposition MyGeoPosition = null;


        //For Router
        RouteQuery MyQuery = null;
        GeocodeQuery Mygeocodequery = null;
        MapRoute MyMapRoute;

        //VibrateController
        VibrateController VibrateController = VibrateController.Default;


        //For Distance
        Double distanceMeter;

        //For menu
        double initialPosition;
        bool _viewMoved = false;

        //For show taxi layer
        MapLayer myTaxiLayer = new MapLayer();


        public HomePage()
        {
            InitializeComponent();

            //get My Position
            this.GetMyPosition();

            //HardCode Taxi position
            this.getTaxiPosition(47.678603, -122.134643);
            this.getTaxiPosition(47.678574, -122.127626);
            this.getTaxiPosition(47.676291, -122.134407);

            //Show taxi point to maps
            map_RiderMap.Layers.Add(myTaxiLayer);

            //hide all step sceen
            this.grv_Step02.Visibility = Visibility.Collapsed;
            this.grv_Step03.Visibility = Visibility.Collapsed;

            //auto complete


        }



        //========================= BEGIN get current Position =========================//
        public async void GetMyPosition()
        {
            //await new MessageDialog("Data Loaded!").ShowAsync();
            //get position
            MyGeoPosition = await MyGeolocator.GetGeopositionAsync();
            Geocoordinate MyGeocoordinate = MyGeoPosition.Coordinate;
            GeoCoordinate MyGeoCoordinate = GeoCoordinateConvert.ConvertGeocoordinate(MyGeocoordinate);
            MyGeoPosition = await MyGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
            //Adjust map on the phone screen - 0.001500 to move up the map
            this.map_RiderMap.Center = new GeoCoordinate(MyGeoPosition.Coordinate.Latitude - 0.001500, MyGeoPosition.Coordinate.Longitude);
            this.map_RiderMap.ZoomLevel = 16;


            //Show maker
            // Create a small Point to mark the current location.
            Image myPositionIcon = new Image();
            myPositionIcon.Source = new BitmapImage(new Uri("/Images/Icons/img_MyPositionIcon.png", UriKind.Relative));
            myPositionIcon.Height = 35;
            myPositionIcon.Width = 25;

            // Create a MapOverlay to contain the circle.
            MapOverlay myLocationOverlay = new MapOverlay();
            myLocationOverlay.Content = myPositionIcon;

            //MapOverlay PositionOrigin to 0.9, 0. MapOverlay will align it's center towards the GeoCoordinate
            myLocationOverlay.PositionOrigin = new Point(0.9, 0.9);
            myLocationOverlay.GeoCoordinate = MyGeoCoordinate;

            // Create a MapLayer to contain the MapOverlay.
            MapLayer myLocationLayer = new MapLayer();
            myLocationLayer.Add(myLocationOverlay);

            // Add the MapLayer to the Map.
            map_RiderMap.Layers.Add(myLocationLayer);
        }
        //========================= END get current Position =========================//



        //========================= BEGIN route Direction on Map =========================//
        private async void GetCoordinates(String destinationAddressInput)
        {
            MyGeolocator.DesiredAccuracyInMeters = 5;
            try
            {
                //Set Position point
                MyGeoPosition = await MyGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
                MyCoordinates.Add(new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude));
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Dịch vụ định vị đang tắt, vui lòng bật lên hoặc kiểm tra lại các thiết đặt.");
            }
            catch (Exception ex)
            {
                // Something else happened while acquiring the location.
                MessageBox.Show(ex.Message);
            }

            Mygeocodequery = new GeocodeQuery();
            Mygeocodequery.SearchTerm = destinationAddressInput;
            Mygeocodequery.GeoCoordinate = new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude);


            Mygeocodequery.QueryCompleted += Mygeocodequery_QueryCompleted;
            Mygeocodequery.QueryAsync();
        }


        void Mygeocodequery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Error == null)
            {
                MyQuery = new RouteQuery();
                MyCoordinates.Add(e.Result[0].GeoCoordinate);
                MyQuery.Waypoints = MyCoordinates;
                MyQuery.QueryCompleted += MyQuery_QueryCompleted;
                MyQuery.QueryAsync();
                Mygeocodequery.Dispose();
            }
        }



        void MyQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            if (e.Error == null)
            {  
                Route MyRoute = e.Result;
                MyMapRoute = new MapRoute(MyRoute);
                //Makeup for router
                MyMapRoute.Color = Color.FromArgb(255, (byte)185, (byte)207, (byte)231); // aRGB for #b9cfe7
                map_RiderMap.AddRoute(MyMapRoute);
                MyQuery.Dispose();

                //get Coordinate of Destination Point
                double destinationLatitude = MyCoordinates[MyCoordinates.Count - 1].Latitude;
                double destinationLongtitude = MyCoordinates[MyCoordinates.Count - 1].Longitude;

                //Set Map Center
                this.map_RiderMap.Center = new GeoCoordinate(destinationLatitude - 0.001500, destinationLongtitude);

                // Create a small Point to mark the current location.
                Image myPositionIcon = new Image();
                myPositionIcon.Source = new BitmapImage(new Uri("/Images/Icons/img_DestinationPoint.png", UriKind.Relative));
                myPositionIcon.Height = 35;
                myPositionIcon.Width = 29;

                // Create a MapOverlay to contain the circle.
                MapOverlay myLocationOverlay = new MapOverlay();
                myLocationOverlay.Content = myPositionIcon;

                //MapOverlay PositionOrigin to 0.3, 0.9 MapOverlay will align it's center towards the GeoCoordinate
                myLocationOverlay.PositionOrigin = new Point(0.3, 0.9);
                myLocationOverlay.GeoCoordinate = new GeoCoordinate(destinationLatitude, destinationLongtitude);

                // Create a MapLayer to contain the MapOverlay.
                MapLayer myLocationLayer = new MapLayer();
                myLocationLayer.Add(myLocationOverlay);

                // Add the MapLayer to the Map.
                map_RiderMap.Layers.Add(myLocationLayer);

                //Calculate Distance
                distanceMeter = Math.Round(GetTotalDistance(MyCoordinates), 0); //Round double in zero decimal places
            }
        }
        //========================= END route Direction on Map =========================//



        //========================= BEGIN calculate Distance =========================//
        public static double GetTotalDistance(IEnumerable<GeoCoordinate> coordinates)
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
        //========================= END calculate Distance =========================//





        //========================= BEGIN show and Design UI 3 taxi near current position =========================//
        public void getTaxiPosition(double lat, double lng)
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
            taxiName.Text = "ACB Taxi";
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
            myTaxiLayer.Add(myTaxiOvelay);
        }

        //Tapped event
        void taxiIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Hide Step 01
            this.grv_Step01.Visibility = Visibility.Collapsed;

            //Show Step 02
            this.grv_Step02.Visibility = Visibility.Visible;
        }
        //========================= END show and Design UI 3 taxi near current position =========================//



        //========================= BEGIN For open menu =========================//
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
            if (e.DeltaManipulation.Translation.X != 0)
                Canvas.SetLeft(LayoutRoot, Math.Min(Math.Max(-840, Canvas.GetLeft(LayoutRoot) + e.DeltaManipulation.Translation.X), 0));
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
        //========================= END For open menu =========================//




        //========================= BEGIN Taxi type bar =========================//
        private void img_CarBar_SavingCar_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            img_CarBar_SavingCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Saving_Selected.png", UriKind.Relative));
            img_CarBar_EconomyCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Economy_NotSelected.png", UriKind.Relative));
            img_CarBar_LuxuryCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Luxury_NotSelected.png", UriKind.Relative));
        }
        private void img_CarBar_EconomyCar_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            img_CarBar_SavingCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Saving_NotSelected.png", UriKind.Relative));
            img_CarBar_EconomyCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Economy_Selected.png", UriKind.Relative));
            img_CarBar_LuxuryCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Luxury_NotSelected.png", UriKind.Relative));
        }
        private void img_CarBar_LuxuryCar_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            img_CarBar_SavingCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Saving_NotSelected.png", UriKind.Relative));
            img_CarBar_EconomyCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Economy_NotSelected.png", UriKind.Relative));
            img_CarBar_LuxuryCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Luxury_Selected.png", UriKind.Relative));
        }
        //========================= END Taxi type bar =========================//





        //========================= BEGIN Car type bar chose =========================//
        private void img_CallTaxi_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.grv_Step01.Visibility = Visibility.Collapsed;
            this.grv_Step02.Visibility = Visibility.Visible;
        }
        //========================= END Car type bar chose =========================//



        //========================= BEGIN Map API key =========================//
        private void map_RiderMap_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "5fcbf5e6-e6d0-48d7-a69d-8699df1b5318";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "I5nG-B7z5bxyTGww1PApXA";
        }
        //========================= END Map API key =========================//


        private void tb_InputAddress_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //check if input is "Enter" key
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                string destinationAddress;
                destinationAddress = txt_InputAddress.Text;
                this.GetCoordinates(destinationAddress);
                //Hide keyboard
                this.Focus();
            }
        }

        private void txt_InputAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //check if text is "Địa chỉ đón"
            if (txt_InputAddress.Text == StaticVariables.destiationAddressDescription)
            {
                txt_InputAddress.Text = string.Empty;
                txt_InputAddress.Foreground = new SolidColorBrush(Colors.Black);
            }
        }



        //========================= BEGIN Auto Complete =========================//
        private void txt_InputAddress_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            lls_AutoComplete.Visibility = System.Windows.Visibility.Visible;
            string queryAddress = txt_InputAddress.Text;
            lls_AutoComplete.Background = new SolidColorBrush(Color.FromArgb(255, (byte)16, (byte)15, (byte)39)); //RBG color for #060f27
            loadAutoCompletePlace(queryAddress);

        }

        private void loadAutoCompletePlace(string inputAddress)
        {
            string URL = StaticVariables.queryAutocompleteRequestsBaseURI + StaticVariables.googleGeolocationAPIkey + "&input=" + inputAddress;

            //Query Autocomplete Responses to a JSON String
            WebClient proxy = new WebClient();
            proxy.DownloadStringCompleted +=
            new DownloadStringCompletedEventHandler(proxy_DownloadStringCompleted);
            proxy.DownloadStringAsync(new Uri(URL));
        }

        private void proxy_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            //1. Convert Json String to an Object
            GoogleAPIQueryAutoComplete places = new GoogleAPIQueryAutoComplete();
            places = JsonConvert.DeserializeObject<GoogleAPIQueryAutoComplete>(e.Result);
            //2. Create Place list
            ObservableCollection<AutoCompletePlaceList> autoCompleteDataSource = new ObservableCollection<AutoCompletePlaceList>();
            lls_AutoComplete.ItemsSource = autoCompleteDataSource;
            //3. Loop to list all item in object
            foreach (var obj in places.predictions)
            {
                autoCompleteDataSource.Add(new AutoCompletePlaceList(obj.description.ToString()));
            }

        }

        private void lls_AutoComplete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedPlace = ((AutoCompletePlaceList)(sender as LongListSelector).SelectedItem);
            // If selected item is null, do nothing
            if (lls_AutoComplete.SelectedItem == null)
                return;
            this.GetCoordinates(selectedPlace.Name.ToString());

            //clear lls
            lls_AutoComplete.Visibility = System.Windows.Visibility.Collapsed;
            lls_AutoComplete.SelectedItem = null;
        }
        //========================= END Auto Complete =========================//


        
    }

}