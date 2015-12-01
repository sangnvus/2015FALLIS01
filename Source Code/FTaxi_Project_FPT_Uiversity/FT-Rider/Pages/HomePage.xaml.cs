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
using System.Windows.Input;
using System.Windows.Threading;
using System.IO.IsolatedStorage;
using System.Device.Location;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Data.Linq;
using System.ComponentModel;
using System.IO;
using Windows.Storage;
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
using Microsoft.Phone.Notification;
using System.Text;


namespace FT_Rider.Pages
{
    public partial class HomePage : PhoneApplicationPage
    {
        //USER DATA PASS FROM LOGIN PAGE
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        RiderLogin userData = new RiderLogin();
        string userId = "";
        string pwmd5 = "";
        string rawPassword;

        //For Store Points
        List<GeoCoordinate> riderCoordinates = new List<GeoCoordinate>();

        //For Router        
        GeocodeQuery riderGeocodeQuery = null;
        RouteQuery riderQuery = null;
        MapRoute riderMapRoute = null;
        Route riderRoute = null;

        //For get Current Localtion
        Geolocator riderFirstGeolocator = null;
        Geoposition riderFirstGeoposition = null;
        MapOverlay riderMapOverlay = null;
        MapLayer riderMapLayer = null;


        //VibrateController
        VibrateController vibrateController = VibrateController.Default;

        //For Distance
        Double? distanceMeter = null;

        //Rider Destination Icon Overlay
        MapOverlay riderDestinationIconOverlay;

        //for car types
        string taxiType = null;

        //For left menu
        double initialPosition;
        bool _viewMoved = false;

        //For Timer
        //DispatcherTimer pickupTimer;
        bool isPickup = false;

        //For near driver
        IDictionary<string, ListDriverDTO> nearDriverCollection = new Dictionary<string, ListDriverDTO>();
        string selectedDid = null; //This variable to detect what car is choosen in map
        DispatcherTimer getNearDriverTimer = new DispatcherTimer();

        //For GET PICK UP & Create Trip
        double pickupLat;
        double pickupLng;
        double destinationLat; //Why Double? because destinationLat can be null
        double destinationLng;
        string pickupType = ConstantVariable.ONE_MANY;
        RiderCreateTrip createTrip;

        //For City Name
        IDictionary<string, RiderGetCityList> cityNamesDB = new Dictionary<string, RiderGetCityList>();

        //For process bar
        double tmpLat;
        double tmpLng;

        //For Notification 
        string pushChannelURI = "";


        public HomePage()
        {

            InitializeComponent();

            //Tạo kênh Notification
            CreatePushChannel();


            //Get User data from login
            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userData = (RiderLogin)tNetUserLoginData["UserLoginData"];
            }
            if (tNetUserLoginData.Contains("UserId") && tNetUserLoginData.Contains("PasswordMd5"))
            {
                userId = (string)tNetUserLoginData["UserId"];
                pwmd5 = (string)tNetUserLoginData["PasswordMd5"];
            }
            if (tNetUserLoginData.Contains("RawPassword"))
            {
                rawPassword = (string)tNetUserLoginData["RawPassword"];
            }

            //get First Local Position
            GetCurrentCoordinate();

            //hide all step screen
            grv_ProcessScreen.Visibility = Visibility.Visible; //Enable Process bar
            this.grv_Step02.Visibility = Visibility.Collapsed;
            this.grv_Step03.Visibility = Visibility.Collapsed;
            this.lls_AutoComplete.IsEnabled = false;

            //default taxi type
            taxiType = TaxiTypes.Type.ECO.ToString();

            //Load Rider Profile on Left Menu
            LoadRiderProfile();

            //Create CityName DB
            LoadCityNameDataBase();
            //
            //pickupTimer = new DispatcherTimer();
            //pickupTimer.Tick += new EventHandler(pickupTimer_Tick);
            //pickupTimer.Interval = new TimeSpan(0, 0, 0, 2);


            //Cái này để chạy Getneardriver. 
            //Nếu sau khi login mà chưa kịp cập nhật lat lng thì sau 3 giây sẽ gọi lại
            getNearDriverTimer = new DispatcherTimer();
            getNearDriverTimer.Tick += new EventHandler(getNearDriverTimer_Tick);
            getNearDriverTimer.Interval = new TimeSpan(0, 0, 0, 3);

        }

        private void getNearDriverTimer_Tick(object sender, EventArgs e)
        {
            GetNearDriver();
            getNearDriverTimer.Stop();
            //throw new NotImplementedException();
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



        //Check exxception input // and cntry is VN
        public async void LoadCityNameDataBase()
        {
            //{"uid":"apl.ytb2@gmail.com","pw":"Abc123!","lan":"VI","cntry":"VN"}
            var uid = userData.content.uid;
            //var pw = pwmd5;
            var lan = userData.content.lan;
            var cntry = userData.content.cntry;
            var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"lan\":\"{2}\",\"cntry\":\"{3}\"}}", uid, rawPassword, lan, cntry);
            var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderGetCityName, input);
            RiderGetCityNames cityItem;
            try
            {

                cityItem = JsonConvert.DeserializeObject<RiderGetCityNames>(output);
                foreach (var item in cityItem.content.list)
                {
                    cityNamesDB[item.cityName] = new RiderGetCityList
                    {
                        cityId = item.cityId,
                        lan = item.lan,
                        cityName = item.cityName,
                        googleName = item.googleName,
                        lat = item.lat,
                        lng = item.lng
                    };
                }
            }
            catch (NullReferenceException)
            {

                MessageBox.Show(ConstantVariable.errServerErr);
            }
        }



        //------ BEGIN get current Position ------//
        private async void GetCurrentCoordinate()
        {
            riderFirstGeolocator = new Geolocator();
            riderFirstGeolocator.DesiredAccuracy = PositionAccuracy.High;
            riderFirstGeolocator.MovementThreshold = 20;
            riderFirstGeolocator.ReportInterval = 100;
            riderFirstGeoposition = await riderFirstGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));


            //Add img_CurrentLocation to Map
            Image currentLocationPin = new Image();
            currentLocationPin.Source = new BitmapImage(new Uri("/Images/Icons/img_CurrentLocation.png", UriKind.Relative));
            currentLocationPin.Height = 27;
            currentLocationPin.Width = 25;

            riderMapOverlay = new MapOverlay();
            riderMapOverlay.Content = currentLocationPin; //Phải khai báo 1 lớp Overlay vì Overlay có thuộc tính tọa độ (GeoCoordinate)
            riderMapOverlay.GeoCoordinate = new GeoCoordinate(riderFirstGeoposition.Coordinate.Latitude, riderFirstGeoposition.Coordinate.Longitude);
            riderMapOverlay.PositionOrigin = new Point(0.5, 0.5);

            riderMapLayer = new MapLayer();
            riderMapLayer.Add(riderMapOverlay); //Phải khai báo 1 Layer vì không thể add trực tiếp Overlay vào Map, mà phải thông qua Layer của Map
            map_RiderMap.Layers.Add(riderMapLayer);

            // initialize pickup coordinates
            //pickupLat = riderFirstGeoposition.Coordinate.Latitude; //Có thể xóa
            //pickupLng = riderFirstGeoposition.Coordinate.Longitude;

            //// initialize pickup coordinates
            tmpLat = Math.Round(riderFirstGeoposition.Coordinate.Latitude, 5);
            tmpLng = Math.Round(riderFirstGeoposition.Coordinate.Longitude, 5);

            riderFirstGeolocator.PositionChanged += geolocator_PositionChanged;

            //Set Center view
            map_RiderMap.SetView(riderFirstGeoposition.Coordinate.ToGeoCoordinate(), 16, MapAnimationKind.Linear);

            GetNearDriver();

        }

        private void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {

                Geocoordinate geocoordinate = geocoordinate = args.Position.Coordinate;
                riderMapOverlay.GeoCoordinate = geocoordinate.ToGeoCoordinate(); //Cứ mỗi lần thay đổi vị trí, Map sẽ cập nhật tọa độ của Marker

            });

        }
        //------ END get current Position ------//





        //------ BEGIN get near Driver ------//
        private async void GetNearDriver()
        {
            if (pickupLat != 0 && pickupLng != 0)
            {
                var uid = userData.content.uid;
                var lat = pickupLat;
                var lng = pickupLng;
                var clvl = taxiType;

                var input = string.Format("{{\"uid\":\"{0}\",\"lat\":{1},\"lng\":{2},\"cLvl\":\"{3}\"}}", uid, lat.ToString().Replace(',', '.'), lng.ToString().Replace(',', '.'), clvl);
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderGetNerDriverAddress, input);
                RiderGetNearDriver nearDriver;
                try
                {
                    nearDriver = JsonConvert.DeserializeObject<RiderGetNearDriver>(output);
                    if (nearDriver.content != null)
                    {
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

                        foreach (KeyValuePair<string, ListDriverDTO> tmpIter in nearDriverCollection)
                        {
                            ShowNearDrivers(tmpIter.Key);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Co loi 1");
                    }
                }
                catch (Exception)
                {

                    MessageBox.Show("Co loi 2");
                }
            }
            else
            {
                getNearDriverTimer.Start();
            }



        }
        //------ END get near Driver ------//


        //------ BEGIN show and Design UI 3 taxi near current position ------//
        private async void ShowNearDrivers(string did)
        {
            GeoCoordinate TaxiCoordinate = new GeoCoordinate(nearDriverCollection[did].lat, nearDriverCollection[did].lng);

            double openPrice = nearDriverCollection[did].oPrice;
            double estimateCost = 0;
            double estimateKm = 0;
            string driverName = nearDriverCollection[did].lName + ", " + nearDriverCollection[did].fName;
            if (destinationLat != 0 && destinationLng != 0)
            {
                estimateKm = await GoogleAPIFunction.GetDistance(pickupLat, pickupLng, destinationLat, destinationLng);
                estimateCost = RiderFunctions.EstimateCostCalculate(nearDriverCollection, did, estimateCost);
            }
            var str = await GoogleAPIFunction.ConvertLatLngToAddress(pickupLat, pickupLng);
            var address = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(str);


            //Create taxi icon on map
            Image taxiIcon = new Image();
            taxiIcon.Source = new BitmapImage(new Uri("/Images/Taxis/img_CarIcon.png", UriKind.Relative));

            //Add a tapped event
            //Show taxi and trip infor
            //taxiIcon.Tap += taxiIcon_Tap;

            taxiIcon.Tap += (sender, eventArgs) =>
            {
                selectedDid = did;
                txt_OpenPrice.Text = openPrice.ToString();
                txt_EstimatedCost.Text = estimateCost.ToString();
                txt_RiderName.Text = driverName;
                txt_PickupAddress.Text = address.results[0].formatted_address.ToString();

                //Hide Step 01
                this.grv_Step01.Visibility = Visibility.Collapsed;

                //Show Step 02
                this.grv_Step02.Visibility = Visibility.Visible;
                this.grv_Picker.Visibility = Visibility.Collapsed;
            };



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
                //distanceMeter = Math.Round(GetTotalDistance(riderCoordinates), 0); //Round double in zero decimal places
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






        //------ BEGIN Complete ------//
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
            ///searchCoordinateFromAddress(selectedPlace.Name.ToString());
            setPickupAddressFromSearchBar(selectedPlace.Name.ToString());
            //showMapRoute(21.031579, 105.779560);



            //and fill to address textbox on search bar
            txt_InputAddress.Text = selectedPlace.Name.ToString();
            setCursorAtLast(txt_InputAddress);

            //vibrate phone
            //vibrateController.Start(TimeSpan.FromSeconds(0.1));
            TouchFeedback();

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




        //------ BEGIN set Pickup Address from address search ------//
        private void setPickupAddressFromSearchBar(string inputAddress)
        {
            //GoogleAPIGeocoding URL
            string URL = ConstantVariable.googleAPIGeocodingAddressBaseURI + inputAddress + "&key=" + ConstantVariable.googleGeolocationAPIkey;

            //Query Autocomplete Responses to a JSON String
            WebClient proxy = new WebClient();
            proxy.DownloadStringCompleted +=
            new DownloadStringCompletedEventHandler(proxy_setPickupAddressFromSearchBar);
            proxy.DownloadStringAsync(new Uri(URL));
        }
        private void proxy_setPickupAddressFromSearchBar(object sender, DownloadStringCompletedEventArgs e)
        {
            //1. Convert Json String to an Object
            GoogleAPIAddressObj places = new GoogleAPIAddressObj();
            places = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(e.Result);
            try
            {
                double lat = places.results[0].geometry.location.lat;
                double lng = places.results[0].geometry.location.lng;

                //route direction on map
                map_RiderMap.SetView(new GeoCoordinate(lat, lng), 16, MapAnimationKind.Linear);
                img_PickerPin.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {

                MessageBox.Show(ConstantVariable.errInvalidAddress);
            }
        }
        //------ END set Pickup Address from address search  ------//





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

            if (txt_InputAddress.Text == string.Empty)
            {
                map_RiderMap.Focus();
                img_PickerLabel.Visibility = Visibility.Visible;
                img_PickerPin.Visibility = Visibility.Visible;
            }
            else
            {
                txt_InputAddress.Text = String.Empty;
                txt_InputAddress.Focus();
            }
            //lls_AutoComplete.Visibility = Visibility.Collapsed;
            //img_CloseIcon.Visibility = Visibility.Collapsed;
            //lls_AutoComplete.IsEnabled = false;

        }

        //Textbox background focus transparent
        private void txt_InputAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            //Hide Pickup icon
            img_PickerLabel.Visibility = Visibility.Collapsed;
            img_PickerPin.Visibility = Visibility.Collapsed;

            //Enable Auto Complete
            loadAutoCompletePlace("");
            enableAutoComplateGrid();

            TextBox addressTextbox = (TextBox)sender;
            addressTextbox.Background = new SolidColorBrush(Colors.Transparent);
            addressTextbox.BorderBrush = new SolidColorBrush(Colors.Transparent);
            addressTextbox.SelectionBackground = new SolidColorBrush(Colors.Transparent); ;

            //img_CloseIcon.Visibility = Visibility.Visible;

            if (txt_InputAddress.Text == ConstantVariable.destiationAddressDescription)
            {
                txt_InputAddress.Text = string.Empty;
            }

            //Display Close icon
            img_CloseIcon.Visibility = Visibility.Visible;

            //hide close icon
            //if (txt_InputAddress.Text == String.Empty)
            //{
            //    img_CloseIcon.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    img_CloseIcon.Visibility = Visibility.Visible;
            //}
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
            if (new GeoCoordinate(Math.Round(map_RiderMap.Center.Latitude, 5), Math.Round(map_RiderMap.Center.Longitude, 5)).Equals(new GeoCoordinate(tmpLat, tmpLng)))
            {
                grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable process bar
            }

            img_PickerLabel.Visibility = Visibility.Visible; //Enable Pickup label
            //img_PickerLabel.Source = new BitmapImage(new Uri("/Images/Picker/img_Picker_CallTaxi.png", UriKind.Relative));
            pickupLat = map_RiderMap.Center.Latitude;
            pickupLng = map_RiderMap.Center.Longitude;
            if (isPickup == true)
            {
                //pickupTimer.Start();
                ShowPickerAddress();
                GetNearDriver();
                //
            }
            isPickup = false;
        }


        private void map_RiderMap_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //((Storyboard)FindName("animate")).Begin(img_PickerLabel);
            //img_PickerLabel.Source = new BitmapImage(new Uri("/Images/Picker/img_Picker_SetPickup.png", UriKind.Relative));
            //pickupTimer.Stop();
            img_PickerLabel.Visibility = Visibility.Collapsed; //Disable Pickup label
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




        private void canvas_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {

        }

        private void txt_PickupAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            setCursorAtLast(txt_PickupAddress);
        }


        //Focus to "PickupType"
        private async void img_RequestTaxiButton_Tap(object sender, System.Windows.Input.GestureEventArgs e) //Check null input
        {

        }



        //This function to get City Code From City Name in City Dictionary
        private int GetCityCodeFromCityName(string cityName)
        {
            return cityNamesDB[cityName].cityId;
        }

        private async void btn_RequestTaxi_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            pb_PleaseWait.Visibility = Visibility.Visible; //Khi bắt đầu nhấn nút "Yêu cầu Taxi" thì hệ thống sẽ hiện ProcessC Bar "Vui lòng đợi"
            SwitchToWaitingStatus();
            chk_AutoRecall.IsEnabled = false;

            int sCity = GetCityCodeFromCityName(await GoogleAPIFunction.GetCityNameFromCoordinate(pickupLat, pickupLng));
            string eCityName;
            int eCity;
            if (destinationLat == 0 && destinationLng == 0)
            {
                eCity = 0;
                eCityName = "";
            }
            else
            {
                eCity = GetCityCodeFromCityName(await GoogleAPIFunction.GetCityNameFromCoordinate(destinationLat, destinationLng));
                eCityName = await GoogleAPIFunction.GetCityNameFromCoordinate(destinationLat, destinationLng);
            }
            string sCityName = await GoogleAPIFunction.GetCityNameFromCoordinate(pickupLat, pickupLng);
            string cntry = await GoogleAPIFunction.GetCountryNameFromCoordinate(pickupLat, pickupLng);
            string proCode = "";

            ///Có nghĩa là sao? 
            ///Nếu như không tích vào ô tự động gọi xe khác thì hệ thống chỉ gửi 1 did thôi
            ///còn nếu có tích vào ô đó thì hệ thống sẽ gửi kèm lên 1 list các did
            List<string> didList = new List<string>();
            if (chk_AutoRecall.IsChecked == false)
            {
                didList = new List<string> { selectedDid };
            }
            else
            {

                foreach (KeyValuePair<string, ListDriverDTO> tmpIter in nearDriverCollection)
                {
                    didList.Add(tmpIter.Key);
                }
            }

            //Create Request Trip Onject 
            RiderCreateTrip createTrip = new RiderCreateTrip
            {
                uid = userData.content.uid,
                rid = userData.content.rid,
                did = didList,
                sAddr = txt_PickupAddress.Text,
                eAddr = txt_DestinationAddress.Text,
                sLat = pickupLat,
                sLng = pickupLng,
                eLat = destinationLat,
                eLng = destinationLng,
                sCity = sCity,
                eCity = eCity,
                sCityName = sCityName,
                eCityName = eCityName,
                cntry = cntry,
                proCode = proCode,
                rType = pickupType
            };

            string didString = "";
            for (int i = 0; i < createTrip.did.Count; i++)
            {
                didString += didList[i];
                didString += ",";
            }

            var input = string.Format("{{\"uid\":\"{0}\",\"rid\":\"{1}\",\"did\":[\"{2}\"],\"sAddr\":\"{3}\","
                + "\"eAddr\":\"{4}\",\"sLat\":\"{5}\",\"sLng\":\"{6}\","
                + "\"eLat\":\"{7}\",\"eLng\":\"{8}\",\"sCity\":\"{9}\","
                + "\"eCity\":\"{10}\",\"sCityName\":\"{11}\",\"eCityName\":\"{12}\","
                + "\"cntry\":\"{13}\",\"proCode\":\"{14}\",\"rType\":\"{15}\"}}",
                createTrip.uid,
                createTrip.rid,
                didString.Remove(didString.Length - 1),
                createTrip.sAddr,
                createTrip.eAddr,
                createTrip.sLat.ToString().Replace(',', '.'),
                createTrip.sLng.ToString().Replace(',', '.'),
                createTrip.eLat.ToString().Replace(',', '.'),
                createTrip.eLng.ToString().Replace(',', '.'),
                createTrip.sCity,
                createTrip.eCity,
                createTrip.sCityName,
                createTrip.eCityName,
                createTrip.cntry,
                createTrip.proCode,
                createTrip.rType);
            var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderCreateTrip, input);
            var createTripResponse = JsonConvert.DeserializeObject<BaseResponse>(output);
            if (createTripResponse.lmd != 0) //check if create trip ok
            {
                //btn_RequestTaxi.IsEnabled = false;
                //btn_RequestTaxi.Content = "Vui lòng đợi...";
                //btn_RequestTaxi.BorderBrush.Opacity = 0;
                //SwitchToWaitingStatus();

            }
        }

        private void SwitchToWaitingStatus()
        {
            if (grv_Step02.Visibility == Visibility.Collapsed)
            {
                grv_Step02.Visibility = Visibility.Visible;
            }
            btn_RequestTaxi.IsEnabled = false;
            btn_RequestTaxi.Content = ConstantVariable.strPleseWait;
            btn_RequestTaxi.BorderBrush.Opacity = 0;
        }

        private void map_RiderMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            img_PickerLabel.Visibility = Visibility.Visible;
        }

        private void TouchFeedback()
        {
            vibrateController.Start(TimeSpan.FromSeconds(0.1));
        }

        private async void img_PickerLabel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            btn_RequestTaxi.IsEnabled = false;
            TouchFeedback();
            grv_Picker.Visibility = Visibility.Collapsed;
            SwitchToWaitingStatus();


            //Prepare for req service
            int sCity = GetCityCodeFromCityName(await GoogleAPIFunction.GetCityNameFromCoordinate(pickupLat, pickupLng));
            string eCityName;
            int eCity;
            if (destinationLat == 0 && destinationLng == 0)
            {
                eCity = 0;
                eCityName = "";
            }
            else
            {
                eCity = GetCityCodeFromCityName(await GoogleAPIFunction.GetCityNameFromCoordinate(destinationLat, destinationLng));
                eCityName = await GoogleAPIFunction.GetCityNameFromCoordinate(destinationLat, destinationLng);
            }
            string sCityName = await GoogleAPIFunction.GetCityNameFromCoordinate(pickupLat, pickupLng);
            string cntry = await GoogleAPIFunction.GetCountryNameFromCoordinate(pickupLat, pickupLng);
            string proCode = "";
            pickupType = ConstantVariable.MANY;
            List<string> didList = new List<string>();
            foreach (KeyValuePair<string, ListDriverDTO> tmpIter in nearDriverCollection)
            {
                didList.Add(tmpIter.Key);
            }

            //Create Request Trip Onject 
            RiderCreateTrip createTrip = new RiderCreateTrip
            {
                uid = userData.content.uid,
                rid = userData.content.rid,
                did = didList,///
                sAddr = txt_PickupAddress.Text,
                eAddr = txt_DestinationAddress.Text,
                sLat = pickupLat,
                sLng = pickupLng,
                eLat = destinationLat,
                eLng = destinationLng,
                sCity = sCity,
                eCity = eCity,
                sCityName = sCityName,
                eCityName = eCityName,
                cntry = cntry,
                proCode = proCode,
                rType = pickupType///
            };

            string didString = "";
            for (int i = 0; i < createTrip.did.Count; i++)
            {
                didString += didList[i];
                didString += ",";
            }

            var input = string.Format("{{\"uid\":\"{0}\",\"rid\":\"{1}\",\"did\":[\"{2}\"],\"sAddr\":\"{3}\","
                + "\"eAddr\":\"{4}\",\"sLat\":\"{5}\",\"sLng\":\"{6}\","
                + "\"eLat\":\"{7}\",\"eLng\":\"{8}\",\"sCity\":\"{9}\","
                + "\"eCity\":\"{10}\",\"sCityName\":\"{11}\",\"eCityName\":\"{12}\","
                + "\"cntry\":\"{13}\",\"proCode\":\"{14}\",\"rType\":\"{15}\"}}",
                createTrip.uid,
                createTrip.rid,
                didString.Remove(didString.Length - 1), //.Remove(didString.Length-1) to cut "," character at the end of string // ["a","b","c"]
                createTrip.sAddr,
                createTrip.eAddr,
                createTrip.sLat.ToString().Replace(',', '.'),
                createTrip.sLng.ToString().Replace(',', '.'),
                createTrip.eLat.ToString().Replace(',', '.'),
                createTrip.eLng.ToString().Replace(',', '.'),
                createTrip.sCity,
                createTrip.eCity,
                createTrip.sCityName,
                createTrip.eCityName,
                createTrip.cntry,
                createTrip.proCode,
                createTrip.rType);
            var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderCreateTrip, input);
            var createTripResponse = JsonConvert.DeserializeObject<BaseResponse>(output);
            if (createTripResponse.lmd != 0) //check if create trip ok
            {

                /* btn_RequestTaxi.Content = "";*/
                ///Code for create trip successed//
            }

        }


        ///NOTIFICATION CHANNEL
        private void CreatePushChannel()
        {
            HttpNotificationChannel pushChannel;
            string channelName = "FtaxiRiderChannel";
            pushChannel = HttpNotificationChannel.Find(channelName);

            if (pushChannel == null)
            {
                pushChannel = new HttpNotificationChannel(channelName);

                // Register for all the events before attempting to open the channel.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                pushChannel.Open();

                // Bind this new channel for toast events.
                pushChannel.BindToShellToast();

            }
            else
            {
                // The channel was already open, so just register for all the events.
                pushChannel.ChannelUriUpdated += new EventHandler<NotificationChannelUriEventArgs>(PushChannel_ChannelUriUpdated);
                pushChannel.ErrorOccurred += new EventHandler<NotificationChannelErrorEventArgs>(PushChannel_ErrorOccurred);

                // Register for this notification only if you need to receive the notifications while your application is running.
                pushChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

                // Display the URI for testing purposes. Normally, the URI would be passed back to your web service at this point.
                System.Diagnostics.Debug.WriteLine(pushChannel.ChannelUri.ToString());

                pushChannelURI = pushChannel.ChannelUri.ToString();
                UpdateNotificationURI(pushChannelURI);
                //tNetAppSetting["NotificationURI"] = pushChannelURI;
                ///
                ///CODE UPDATE URI HERE///
                ///

                //MessageBox.Show(String.Format("Channel Uri is {0}", pushChannel.ChannelUri.ToString()));

            }
        }

        // Display the new URI for testing purposes.   Normally, the URI would be passed back to your web service at this point.
        void PushChannel_ChannelUriUpdated(object sender, NotificationChannelUriEventArgs e)
        {

            Dispatcher.BeginInvoke(() =>
            {
                System.Diagnostics.Debug.WriteLine(e.ChannelUri.ToString());
                pushChannelURI = e.ChannelUri.ToString();
                UpdateNotificationURI(pushChannelURI);
                //tNetAppSetting["NotificationURI"] = pushChannelURI; //Truyền URI QUA CÁC TRANG KHÁC
                ///
                ///CODE LOAD URI HERE///
                ///

                //MessageBox.Show(String.Format("Channel Uri is {0}",e.ChannelUri.ToString()));
                //>>>>>>>>>>>>>>>>>>>>>>>>> Chan URI HERE <<<<<<<<<<<<<<<<<<<<<<
            });
        }


        // Error handling logic for your particular application would be here.
        void PushChannel_ErrorOccurred(object sender, NotificationChannelErrorEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
                MessageBox.Show(String.Format("A push notification {0} error occurred.  {1} ({2}) {3}",
                    e.ErrorType, e.Message, e.ErrorCode, e.ErrorAdditionalData))
                    );
        }


        // Parse out the information that was part of the message.
        void PushChannel_ShellToastNotificationReceived(object sender, NotificationEventArgs e)
        {
            StringBuilder message = new StringBuilder();
            string relativeUri = string.Empty;

            message.AppendFormat("Received Toast {0}:\n", DateTime.Now.ToShortTimeString());

            // Parse out the information that was part of the message.
            foreach (string key in e.Collection.Keys)
            {
                message.AppendFormat("{0}: {1}\n", key, e.Collection[key]);

                if (string.Compare(
                    key,
                    "wp:Param",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.CompareOptions.IgnoreCase) == 0)
                {
                    relativeUri = e.Collection[key];
                }
            }

            // Display a dialog of all the fields in the toast.
            Dispatcher.BeginInvoke(() => MessageBox.Show(message.ToString()));

        }


        //Cứ mỗi khi URI thay đổi, hệ thống sẽ cập nhật lên sv
        private async void UpdateNotificationURI(string uri)
        {
            var uid = userId;
            var mType = ConstantVariable.mTypeWIN;
            var role = ConstantVariable.dRole;
            var id = userData.content.rid;
            var input = string.Format("{{\"mid\":\"{0}\",\"mid\":\"{1}\",\"mType\":\"{2}\",\"role\":\"{3}\",\"id\":\"{4}\"}}", uid, uri, mType, role, id);
            try
            {
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderUpdateRegId, input);
            }
            catch (Exception)
            {
                //Lỗi máy chủ
                MessageBox.Show(ConstantVariable.errServerErr);
            }

        }

    }
}