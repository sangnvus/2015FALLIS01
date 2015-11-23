using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading;
using System.Windows.Threading;
using System.IO.IsolatedStorage;
using System.Device.Location;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Devices;
using Newtonsoft.Json;
using FT_Rider.Resources;
using FT_Rider.Classes;


namespace FT_Rider.Pages
{
    public partial class HomePage : PhoneApplicationPage
    {
        //For Store Points
        List<GeoCoordinate> riderCoordinates = new List<GeoCoordinate>();

        //For Router        
        GeocodeQuery riderGeocodeQuery = null;
        RouteQuery riderQuery = null;
        MapRoute riderMapRoute = null;
        Route riderRoute = null;

        //For get Current Possirion
        Geolocator riderFirstGeolocator = null;
        Geoposition riderFirstGeoposition = null;

        //For map layer
        MapLayer riderMapLayer;

        //VibrateController
        VibrateController vibrateController = VibrateController.Default;

        //For Distance
        Double distanceMeter;

        //Rider Destination Icon Overlay
        MapOverlay riderDestinationIconOverlay;

        //for car types
        string taxiType;

        //USER DATA
        RiderLogin userData = PhoneApplicationService.Current.State["UserInfo"] as RiderLogin;

        //For GET PICK UP
        Double pickupLat;
        Double pickupLng;


        //For menu
        double initialPosition;
        bool _viewMoved = false;

        //For Timer
        DispatcherTimer pickupTimer;
        bool isPickup = false;

        //for near driver
        IDictionary<string, ListDriverDTO> nearDriverCollection;

        public HomePage()
        {

            InitializeComponent();
            //get First Local Position
            GetCurrentCoordinate();
            //hide all step sceen
            this.grv_Step02.Visibility = Visibility.Collapsed;
            this.grv_Step03.Visibility = Visibility.Collapsed;

            this.lls_AutoComplete.IsEnabled = false;

            //default taxi type
            taxiType = TaxiTypes.Type.ECO.ToString();

            //Load Rider Profile on Left Menu
            LoadRiderProfile();

            //
            pickupTimer = new DispatcherTimer();
            pickupTimer.Tick += new EventHandler(pickupTimer_Tick);
            pickupTimer.Interval = new TimeSpan(0, 0, 0, 2);


            //21.038556, 105.800667
        }










        private void pickupTimer_Tick(object sender, EventArgs e)
        {
            img_PickerLabel.Source = new BitmapImage(new Uri("/Images/Picker/img_Picker_CallTaxi.png", UriKind.Relative));
            img_PickerLabel.Tap += img_PickerLabel_CallTaxi_tab;
        }

        private void img_PickerLabel_CallTaxi_tab(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.grv_Step02.Visibility = Visibility.Visible;
        }


        private void LoadRiderProfile()
        {
            tbl_LastName.Text = userData.content.lName;
            tbl_FirstName.Text = userData.content.fName;
        }


        //------ BEGIN get current Position ------//
        private async void GetCurrentCoordinate()
        {
            riderFirstGeolocator = new Geolocator();
            riderFirstGeolocator.DesiredAccuracy = PositionAccuracy.High;
            riderFirstGeolocator.MovementThreshold = 20;
            riderFirstGeolocator.ReportInterval = 100;
            riderFirstGeoposition = await riderFirstGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));

            //Set Center view
            map_RiderMap.SetView(riderFirstGeoposition.Coordinate.ToGeoCoordinate(), 16, MapAnimationKind.Linear);

            riderFirstGeolocator.PositionChanged += geolocator_PositionChanged;

            //StartPickupTimer();
            //GetNearDriver();
        }

        private void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {

                Geocoordinate geocoordinate = null;
                geocoordinate = args.Position.Coordinate;

                //map_RiderMap.SetView(geocoordinate.ToGeoCoordinate(), 16, MapAnimationKind.Linear);

                UserLocationMarker marker = (UserLocationMarker)this.FindName("UserLocationMarker");
                marker.GeoCoordinate = geocoordinate.ToGeoCoordinate();
            });

        }
        //------ END get current Position ------//





        //------ BEGIN get near Driver ------//
        private async void GetNearDriver()
        {
            var uid = userData.content.uid;
            var lat = pickupLat;
            var lng = pickupLng;
            var clvl = taxiType;

            var input = string.Format("{{\"uid\":\"{0}\",\"lat\":{1},\"lng\":{2},\"cLvl\":\"{3}\"}}", uid, lat.ToString().Replace(',', '.'), lng.ToString().Replace(',', '.'), clvl);
            var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderGetNerDriverAddress, input);
            var nearDriver = JsonConvert.DeserializeObject<RiderGetNearDriver>(output);
            if (nearDriver.content.listDriverDTO.Count > 0)
            {
                nearDriverCollection = new Dictionary<string, ListDriverDTO>();
                foreach (var item in nearDriver.content.listDriverDTO)
                {
                    nearDriverCollection[item.did.ToString()] = new ListDriverDTO
                    {
                        did = item.did,
                        fName = item.fName,
                        lName = item.lName,
                        cName = item.cName,
                        mobile = item.mobile,
                        rate = item.rate,
                        oPrice = item.oPrice,
                        oKm = item.oKm,
                        f1Price = item.f1Price,
                        f1Km = item.f1Km,
                        f2Price = item.f2Price,
                        f2Km = item.f2Km,
                        f3Price = item.f3Price,
                        f3Km = item.f3Km,
                        f4Price = item.f4Price,
                        f4Km = item.f4Km,
                        img = item.img,
                        lat = item.lat,
                        lng = item.lng
                    };
                }
                foreach (var item in nearDriverCollection.Values)
                {
                    ShowNearDrivers(item.did);
                }
            }
        }
        //------ END get near Driver ------//


        //------ BEGIN show and Design UI 3 taxi near current position ------//
        private void ShowNearDrivers(string did)
        {
            GeoCoordinate TaxiCoordinate = new GeoCoordinate(nearDriverCollection[did].lat, nearDriverCollection[did].lng);

            //Create taxi icon on map
            Image taxiIcon = new Image();
            taxiIcon.Source = new BitmapImage(new Uri("/Images/Taxis/img_CarIcon.png", UriKind.Relative));

            //Add a tapped event
            taxiIcon.Tap += taxiIcon_Tap;

            //Create Taxi Name 
            TextBlock taxiName = new TextBlock();
            taxiName.HorizontalAlignment = HorizontalAlignment.Center;
            taxiName.Text = nearDriverCollection[did].cName;
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
            riderMapLayer = new MapLayer();
            riderMapLayer.Add(myTaxiOvelay);

            map_RiderMap.Layers.Add(riderMapLayer);
        }

        //Tapped event
        private void taxiIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Hide Step 01
            this.grv_Step01.Visibility = Visibility.Collapsed;

            //Show Step 02
            this.grv_Step02.Visibility = Visibility.Visible;
            this.grv_Picker.Visibility = Visibility.Collapsed;
            //Step 2 info
            LoadStep2Info();
        }
        //------ END show and Design UI 3 taxi near current position ------//



        //------ BEGIN route Direction on Map ------//
        private async void getMapRouteTo(double lat, double lng)
        {
            //riderCoordinates.RemoveAll(item => item == null);
            //Delete Previous Route if exist
            if (riderMapRoute != null)
            {
                //delete route
                map_RiderMap.RemoveRoute(riderMapRoute);
                riderMapRoute = null;
                riderQuery = null;
                riderMapLayer.Remove(riderDestinationIconOverlay);
            }

            Geolocator riderGeolocator = new Geolocator();
            riderGeolocator.DesiredAccuracyInMeters = 5;
            Geoposition riderGeoPosition = null;
            try
            {
                //Set Position point
                riderGeoPosition = await riderGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
                riderCoordinates.Add(new GeoCoordinate(riderGeoPosition.Coordinate.Latitude, riderGeoPosition.Coordinate.Longitude));
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

            riderGeocodeQuery = new GeocodeQuery();
            riderGeocodeQuery.SearchTerm = lat.ToString().Replace(',', '.') + "," + lng.ToString().Replace(',', '.');
            riderGeocodeQuery.GeoCoordinate = new GeoCoordinate(riderGeoPosition.Coordinate.Latitude, riderGeoPosition.Coordinate.Longitude);


            riderGeocodeQuery.QueryCompleted += Mygeocodequery_QueryCompleted;
            riderGeocodeQuery.QueryAsync();
        }


        private void Mygeocodequery_QueryCompleted(object sender, QueryCompletedEventArgs<IList<MapLocation>> e)
        {
            if (e.Error == null)
            {
                try
                {
                    riderQuery = new RouteQuery();
                    riderCoordinates.Add(e.Result[0].GeoCoordinate);
                    riderQuery.Waypoints = riderCoordinates;
                    riderQuery.QueryCompleted += MyQuery_QueryCompleted;
                    riderQuery.QueryAsync();
                    riderGeocodeQuery.Dispose();
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
                //if (riderMapRoute != null)
                //{
                //    map_RiderMap.RemoveRoute(riderMapRoute);
                //    riderMapLayer.Remove(riderDestinationIconOverlay);
                //    riderMapRoute = null;
                //}                
                riderRoute = e.Result;
                riderMapRoute = new MapRoute(riderRoute);
                //Makeup for router
                riderMapRoute.Color = Color.FromArgb(255, (byte)185, (byte)207, (byte)231); // aRGB for #b9cfe7
                map_RiderMap.AddRoute(riderMapRoute);
                riderQuery.Dispose();

                //get Coordinate of Destination Point
                double destinationLatitude = riderCoordinates[riderCoordinates.Count - 1].Latitude;
                double destinationLongtitude = riderCoordinates[riderCoordinates.Count - 1].Longitude;

                //Set Map Center
                this.map_RiderMap.Center = new GeoCoordinate(destinationLatitude - 0.001500, destinationLongtitude);

                // Create a small Point to mark the current location.
                Image myPositionIcon = new Image();
                myPositionIcon.Source = new BitmapImage(new Uri("/Images/Icons/img_DestinationPoint.png", UriKind.Relative));
                myPositionIcon.Height = 35;
                myPositionIcon.Width = 29;

                // Create a MapOverlay to contain the circle.
                riderDestinationIconOverlay = new MapOverlay();
                riderDestinationIconOverlay.Content = myPositionIcon;

                //MapOverlay PositionOrigin to 0.3, 0.9 MapOverlay will align it's center towards the GeoCoordinate
                riderDestinationIconOverlay.PositionOrigin = new Point(0.3, 0.9);
                riderDestinationIconOverlay.GeoCoordinate = new GeoCoordinate(destinationLatitude, destinationLongtitude);

                // Create a MapLayer to contain the MapOverlay.
                riderMapLayer = new MapLayer();
                riderMapLayer.Add(riderDestinationIconOverlay);

                // Add the MapLayer to the Map.
                map_RiderMap.Layers.Add(riderMapLayer);

                //Calculate Distance
                distanceMeter = Math.Round(GetTotalDistance(riderCoordinates), 0); //Round double in zero decimal places
            }
            else
            {
                MessageBox.Show(ConstantVariable.errInvalidAddress);
                txt_InputAddress.Focus();
            }
        }
        //------ END route Direction on Map ------//






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






        //Load step 02 profile
        private async void LoadStep2Info()
        {
            var str = await GoogleAPIFunction.ConvertLatLngToAddress(pickupLat, pickupLng);
            var address = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(str);

            txt_RiderName.Text = userData.content.fName + " " + userData.content.lName;
            txt_PickupAddress.Text = address.results[0].formatted_address.ToString();

        }




        //------ BEGIN Taxi type bar ------//
        private void img_CarBar_SavingCar_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            img_CarBar_SavingCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Saving_Selected.png", UriKind.Relative));
            img_CarBar_EconomyCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Economy_NotSelected.png", UriKind.Relative));
            img_CarBar_LuxuryCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Luxury_NotSelected.png", UriKind.Relative));

            taxiType = TaxiTypes.Type.SAV.ToString();
        }
        private void img_CarBar_EconomyCar_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            img_CarBar_SavingCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Saving_NotSelected.png", UriKind.Relative));
            img_CarBar_EconomyCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Economy_Selected.png", UriKind.Relative));
            img_CarBar_LuxuryCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Luxury_NotSelected.png", UriKind.Relative));

            taxiType = TaxiTypes.Type.ECO.ToString();
        }
        private void img_CarBar_LuxuryCar_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            img_CarBar_SavingCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Saving_NotSelected.png", UriKind.Relative));
            img_CarBar_EconomyCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Economy_NotSelected.png", UriKind.Relative));
            img_CarBar_LuxuryCar.Source = new BitmapImage(new Uri("/Images/CarsBar/img_Carbar_Luxury_Selected.png", UriKind.Relative));

            taxiType = TaxiTypes.Type.LUX.ToString();
        }
        //------ END Taxi type bar ------//




        //------ BEGIN Car type bar chose ------//
        private void img_CallTaxi_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            this.grv_Step01.Visibility = Visibility.Collapsed;
            this.grv_Step02.Visibility = Visibility.Visible;
        }
        //------ END Car type bar chose ------//






        //------ BEGIN Map API key ------//
        private void map_RiderMap_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "5fcbf5e6-e6d0-48d7-a69d-8699df1b5318";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "I5nG-B7z5bxyTGww1PApXA";
        }
        //------ END Map API key ------//




        //------ BEGIN Convert Lat & Lng from Address for Bing map Input ------//
        private void searchCoordinateFromAddress(string inputAddress)
        {
            //GoogleAPIGeocoding URL
            string URL = ConstantVariable.googleAPIGeocodingAddressBaseURI + inputAddress + "&key=" + ConstantVariable.googleGeolocationAPIkey;

            //Query Autocomplete Responses to a JSON String
            WebClient proxy = new WebClient();
            proxy.DownloadStringCompleted +=
            new DownloadStringCompletedEventHandler(proxy_searchCoordinateFromAddress);
            proxy.DownloadStringAsync(new Uri(URL));
        }
        private void proxy_searchCoordinateFromAddress(object sender, DownloadStringCompletedEventArgs e)
        {
            //1. Convert Json String to an Object
            GoogleAPIAddressObj places = new GoogleAPIAddressObj();
            places = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(e.Result);
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






        //------ BEGIN Auto Complete ------//
        //Parse JSON
        private void loadAutoCompletePlace(string inputAddress)
        {
            //GoogleAPIQueryAutoComplete URL
            string URL = ConstantVariable.googleAPIQueryAutoCompleteRequestsBaseURI + ConstantVariable.googleGeolocationAPIkey + "&input=" + inputAddress;

            //Query Autocomplete Responses to a JSON String
            WebClient proxy = new WebClient();
            proxy.DownloadStringCompleted +=
            new DownloadStringCompletedEventHandler(proxy_loadAutoCompletePlace);
            proxy.DownloadStringAsync(new Uri(URL));
        }

        private void proxy_loadAutoCompletePlace(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                //1. Convert Json String to an Object
                GoogleAPIQueryAutoCompleteObj places = new GoogleAPIQueryAutoCompleteObj();
                places = JsonConvert.DeserializeObject<GoogleAPIQueryAutoCompleteObj>(e.Result);
                //2. Create Place list
                ObservableCollection<AutoCompletePlace> autoCompleteDataSource = new ObservableCollection<AutoCompletePlace>();
                lls_AutoComplete.ItemsSource = autoCompleteDataSource;
                //3. Loop to list all item in object
                foreach (var obj in places.predictions)
                {
                    autoCompleteDataSource.Add(new AutoCompletePlace(obj.description.ToString()));
                }
            }
            catch (Exception)
            {
                txt_InputAddress.Focus();
            }
        }

        //LonglistSelector selection event
        private void lls_AutoComplete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedPlace = ((AutoCompletePlace)(sender as LongListSelector).SelectedItem);
            // If selected item is null, do nothing
            if (lls_AutoComplete.SelectedItem == null)
                return;
            //else route direction
            searchCoordinateFromAddress(selectedPlace.Name.ToString());
            //showMapRoute(21.031579, 105.779560);
            //and fill to address textbox on search bar
            txt_InputAddress.Text = selectedPlace.Name.ToString();
            setCursorAtLast(txt_InputAddress);

            //vibrate phone
            vibrateController.Start(TimeSpan.FromSeconds(0.1));

            //clear lls
        }

        //------ END Auto Complete ------//


        private void enableAutoComplateGrid()
        {
            lls_AutoComplete.IsEnabled = true;
            lls_AutoComplete.Visibility = Visibility.Visible;
        }
        private void disenableAutoComplateGrid()
        {
            lls_AutoComplete.IsEnabled = false;
            lls_AutoComplete.Visibility = Visibility.Collapsed;
        }



        //------ BEGIN Search Bar EVENT ------//
        private void txt_InputAddress_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            img_CloseIcon.Visibility = Visibility.Visible;

            //enable lls
            enableAutoComplateGrid();
            string queryAddress = txt_InputAddress.Text;
            //lls_AutoComplete.Background = new SolidColorBrush(Color.FromArgb(255, (byte)16, (byte)15, (byte)39)); //RBG color for #060f27
            //Call Auto Complete function
            loadAutoCompletePlace(queryAddress);

        }

        private void tb_InputAddress_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //check if input is "Enter" key
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                string destinationAddress;
                destinationAddress = txt_InputAddress.Text;
                this.searchCoordinateFromAddress(destinationAddress);
                //showMapRoute(21.031579, 105.779560);
                //Hide keyboard
                this.Focus();
            }
        }


        private void txt_InputAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //check if text is "Địa chỉ đón"
            if (txt_InputAddress.Text == ConstantVariable.destiationAddressDescription)
            {
                txt_InputAddress.Text = string.Empty;
            }
            txt_InputAddress.Background = new SolidColorBrush(Colors.Transparent);
            //Show end of address
            setCursorAtLast(txt_InputAddress);
        }

        private void img_CloseIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            txt_InputAddress.Text = String.Empty;
            txt_InputAddress.Focus();
            //lls_AutoComplete.Visibility = Visibility.Collapsed;
            //img_CloseIcon.Visibility = Visibility.Collapsed;
            //lls_AutoComplete.IsEnabled = false;

        }

        //Textbox background focus transparent
        private void txt_InputAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            //Enable Auto Complete
            loadAutoCompletePlace("");
            enableAutoComplateGrid();

            TextBox addressTextbox = (TextBox)sender;
            addressTextbox.Background = new SolidColorBrush(Colors.Transparent);
            addressTextbox.BorderBrush = new SolidColorBrush(Colors.Transparent);
            addressTextbox.SelectionBackground = new SolidColorBrush(Colors.Transparent);

            //img_CloseIcon.Visibility = Visibility.Visible;

            if (txt_InputAddress.Text == ConstantVariable.destiationAddressDescription)
            {
                txt_InputAddress.Text = string.Empty;
            }


            //hide close icon
            if (txt_InputAddress.Text == String.Empty)
            {
                img_CloseIcon.Visibility = Visibility.Collapsed;
            }
            else
            {
                img_CloseIcon.Visibility = Visibility.Visible;
            }
            //Show end of address
            setCursorAtLast(txt_InputAddress);
        }

        private void txt_InputAddress_LostFocus(object sender, RoutedEventArgs e)
        {
            disenableAutoComplateGrid();

            img_CloseIcon.Visibility = Visibility.Collapsed;
            if (txt_InputAddress.Text == String.Empty)
            {
                txt_InputAddress.Text = ConstantVariable.destiationAddressDescription;
            }
            //Show first of address
            setCursorAtFirst(txt_InputAddress);
        }

        //Setting cursor at the end of any text of a textbox
        private void setCursorAtLast(TextBox txtBox)
        {
            txtBox.SelectionStart = txtBox.Text.Length; // add some logic if length is 0
            txtBox.SelectionLength = 0;
        }

        //Setting cursor at the first of any text of a textbox
        private void setCursorAtFirst(TextBox txtBox)
        {
            txtBox.SelectionStart = 0;
            txtBox.SelectionLength = 0;
        }

        private async void ShowPickerAddress()
        {
            var str = await GoogleAPIFunction.ConvertLatLngToAddress(pickupLat, pickupLng);
            var address = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(str);
            txt_InputAddress.Text = address.results[0].formatted_address.ToString();
        }


        //Event này để bắt trường hợp sau mỗi lần di chuyển map
        private void map_RiderMap_ResolveCompleted(object sender, MapResolveCompletedEventArgs e)
        {
            pickupLat = map_RiderMap.Center.Latitude;
            pickupLng = map_RiderMap.Center.Longitude;
            if (isPickup == true)
            {
                pickupTimer.Start();
                ShowPickerAddress();
                GetNearDriver();
            }
            isPickup = false;
        }


        private void map_RiderMap_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            img_PickerLabel.Source = new BitmapImage(new Uri("/Images/Picker/img_Picker_SetPickup.png", UriKind.Relative));
            pickupTimer.Stop();
            isPickup = true;
        }




        //------ END Search Bar EVENT ------//




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

        private void txt_PickupAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            //Trong suốt texbox
            TextBox addressTextbox = (TextBox)sender;
            addressTextbox.Background = new SolidColorBrush(Colors.Transparent);
            addressTextbox.BorderBrush = new SolidColorBrush(Colors.Transparent);
            addressTextbox.SelectionBackground = new SolidColorBrush(Colors.Transparent);
        }
        //------ END For open menu ------//



        //private async void GetNearDriverNew()
        //{
        //    var uid = userData.content.uid;
        //    var lat = pickupLat;
        //    var lng = pickupLng;
        //    var clvl = taxiType;

        //    var input = string.Format("{{\"uid\":\"{0}\",\"lat\":{1},\"lng\":{2},\"cLvl\":\"{3}\"}}", uid, lat.ToString().Replace(',', '.'), lng.ToString().Replace(',', '.'), clvl);
        //    var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderGetNerDriverAddress, input);
        //    var nearDriver = JsonConvert.DeserializeObject<RiderGetNearDriver>(output);
        //    if (nearDriver.content.listDriverDTO.Count > 0)
        //    {
        //        IDictionary<string, ListDriverDTO> col = new Dictionary<string, ListDriverDTO>();
        //        foreach (var item in nearDriver.content.listDriverDTO)
        //        {
        //            col[item.did.ToString()] = new ListDriverDTO
        //            {
        //                did = item.did,
        //                fName = item.fName,
        //                lName = item.lName,
        //                cName = item.cName,
        //                mobile = item.mobile,
        //                rate = item.rate,
        //                oPrice = item.oPrice,
        //                oKm = item.oKm,
        //                f1Price = item.f1Price,
        //                f1Km = item.f1Km,
        //                f2Price = item.f2Price,
        //                f2Km = item.f2Km,
        //                f3Price = item.f3Price,
        //                f3Km = item.f3Km,
        //                f4Price = item.f4Price,
        //                f4Km = item.f4Km,
        //                img = item.img,
        //                lat = item.lat,
        //                lng = item.lng
        //            };
        //        }
        //        foreach (var taxi in nearDriver.content.listDriverDTO)
        //        {
        //            //ShowNearDrivers(taxi.lat, taxi.lng, taxi.cName, taxi.did);
        //        }
        //    }
        //}

        private void canvas_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {

        }

    }

}