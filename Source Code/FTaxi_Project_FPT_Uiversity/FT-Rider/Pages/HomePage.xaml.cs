﻿using System;
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
using System.Diagnostics;


namespace FT_Rider.Pages
{
    public partial class HomePage : PhoneApplicationPage
    {
        //USER DATA PASS FROM LOGIN PAGE
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        IsolatedStorageSettings tNetAppSetting = IsolatedStorageSettings.ApplicationSettings;

        RiderLogin userData;
        string userId = string.Empty;
        string pwmd5 = string.Empty;
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
        DispatcherTimer getNearDriverTimer;


        //For GET PICK UP & Create Trip
        double pickupLat;
        double pickupLng;
        double destinationLat = 0; //Why Double? because destinationLat can be null
        double destinationLng = 0;
        string pickupType = ConstantVariable.ONE_MANY;
        RiderCreateTrip createTrip;
        long tlmd;
        RiderNotificationUpdateTrip myTrip;

        //For City Name
        IDictionary<string, RiderGetCityList> cityNamesDB = new Dictionary<string, RiderGetCityList>();

        //For process bar
        double tmpLat;
        double tmpLng;

        //For Notification 
        string pushChannelURI = "";
        string notificationReceivedString = string.Empty;
        string notificationType = string.Empty;

        //For change label
        DispatcherTimer changeLabelRedTimer;


        public HomePage()
        {

            InitializeComponent();

            //Hiện loading screen
            ShowLoadingScreen();

            //Lấy dữ liệu login
            GetUserLoginData();

            //Tạo kênh Notification
            CreatePushChannel();


            //Lấy tọa độ hiện tại
            GetCurrentCoordinate();


            //Đặt kiểu mặc định cho taxi là Kinh tế
            taxiType = TaxiTypes.Type.ECO.ToString();

            //Load Rider Profile on Left Menu
            LoadRiderProfile();

            //Create CityName DB
            LoadCityNameDataBase();


            //Cái này để chạy Getneardriver. 
            //Nếu sau khi login mà chưa kịp cập nhật lat lng thì sau 3 giây sẽ gọi lại
            getNearDriverTimer = new DispatcherTimer();
            getNearDriverTimer.Tick += new EventHandler(getNearDriverTimer_Tick);
            getNearDriverTimer.Interval = new TimeSpan(0, 0, 0, 3);

            //For change red label
            changeLabelRedTimer = new DispatcherTimer();
            changeLabelRedTimer.Tick += new EventHandler(changeLabelRedTimer_Tick);
            changeLabelRedTimer.Interval = new TimeSpan(0, 0, 0, 2);
        }

        private void getNearDriverTimer_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("4433222 Bắt đầu chạy Get Near Driver Timer"); //DELETE AFTER FINISHED
            GetNearDriver();
            //throw new NotImplementedException();
        }




        private void GetUserLoginData()
        {
            //Get User data from login
            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userData = new RiderLogin();
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

                //Sau khi load xing thì đẩy vào isolate
                tNetUserLoginData["CityNamesDB"] = cityNamesDB;
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
            Debug.WriteLine("87wuyw Lấy địa Rider chỉ thành công"); //DELETE AFTER FINISHED
        }

        private void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {

                Geocoordinate geocoordinate = geocoordinate = args.Position.Coordinate;
                riderMapOverlay.GeoCoordinate = geocoordinate.ToGeoCoordinate(); //Cứ mỗi lần thay đổi vị trí, Map sẽ cập nhật tọa độ của Marker

                Debug.WriteLine("35625 geolocator_PositionChanged"); //DELETE AFTER FINISHED

            });

        }
        //------ END get current Position ------//




        //------ BEGIN get near Driver ------//
        /// <summary>
        /// LƯU Ý: SẼ TẠO TIMER CHO VIỆC 30s lấy vị trí một lần
        /// NHƯNG SAU KHI CRETE TRIP THÀNH CÔNG THÌ HỦY TIMER
        /// THAY VÀO ĐÓ LÀ CẬP NHẬT VỊ TRÍ CỦA DRIVER SAU MỖI 30s
        /// </summary>
        private async void GetNearDriver()
        {
            HideCallTaxiCenterPicker();

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


                        //Nếu như không có xe nào thì hiện nút gọi hãng
                        if (nearDriverCollection.Count == 0)
                        {

                            changeLabelRedTimer.Start();
                        }

                        foreach (KeyValuePair<string, ListDriverDTO> tmpIter in nearDriverCollection)
                        {
                            ShowNearDrivers(tmpIter.Key);
                        }

                        Debug.WriteLine("473625 Nhảy vào hàm lấy xe"); //DELETE AFTER FINISHED
                        //Sau đó sẽ cho dừng Timer lại
                        getNearDriverTimer.Stop();
                    }
                    else
                    {
                        // MessageBox.Show("Co loi 1");
                        Debug.WriteLine("87653 Không có xe nào xung quanh"); //DELETE AFTER FINISHED
                        //Thêm code cho việc chuyển label ở đây
                    }

                    Debug.WriteLine("987253 Lấy taxi xung quanh OK"); //DELETE AFTER FINISHED
                }
                catch (Exception)
                {

                    //MessageBox.Show("Co loi 2");
                    Debug.WriteLine("87763355  lấy được chuỗi Json GetNearDriver"); //DELETE AFTER FINISHED
                }
            }
            else
            {
                getNearDriverTimer.Start();
            }



        }


        /// <summary>
        /// CÁI NÀY LÀ ĐỂ ĐỔI LABEL RED
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeLabelRedTimer_Tick(object sender, EventArgs e)
        {
            ShowCallTaxiCenterPicker();
            changeLabelRedTimer.Stop();
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
                try
                {
                    //thử xem có tính tiền đc ko?

                    estimateKm = await GoogleAPIFunction.GetDistance(pickupLat, pickupLng, destinationLat, destinationLng);
                    estimateCost = RiderFunctions.EstimateCostCalculate(nearDriverCollection, did, estimateCost);
                }
                catch (Exception)
                {

                    //Nếu ko tính đc thì...
                    Debug.WriteLine("Có lỗi 689rggg ở Tính km và tính tiền");
                }
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
                Debug.WriteLine("Chạm vào một Taxi thành công");

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
        /// <summary>
        /// HÀM NÀY TRẢ VỀ TỌA ĐỘ CỦA MỘT ĐIỂM KHI NHẬP VÀO
        /// </summary>
        /// <param name="inputAddress"></param>
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
            string URL = ConstantVariable.googleAPIQueryAutoCompleteRequestsBaseURI + ConstantVariable.googleGeolocationAPIkey + "&types=geocode&language=vi" + "&input=" + inputAddress;

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
                ObservableCollection<AutoCompletePlaceLLSObj> autoCompleteDataSource = new ObservableCollection<AutoCompletePlaceLLSObj>();
                lls_AutoComplete.ItemsSource = autoCompleteDataSource;
                //3. Loop to list all item in object
                foreach (var obj in places.predictions)
                {
                    autoCompleteDataSource.Add(new AutoCompletePlaceLLSObj(obj.description.ToString()));
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
            var selectedPlace = ((AutoCompletePlaceLLSObj)(sender as LongListSelector).SelectedItem);
            // If selected item is null, do nothing
            if (lls_AutoComplete.SelectedItem == null)
                return;

            //Khi nhấn vào 1 đỉa chỉ trong danh sách tự động tìm địa chỉ thì sẽ đặt địa chỉ đón
            setPickupAddressFromSearchBar(selectedPlace.Name.ToString());

            //Và điền địa chỉ vào ô tìm kiếm
            txt_InputAddress.Text = selectedPlace.Name.ToString();
            setCursorAtLast(txt_InputAddress);

            //rung phản hồi
            TouchFeedback();

            //Xóa lls
            //lls_AutoComplete = null;
        }

        //------ END Auto Complete ------//







        //------ BEGIN set Pickup Address from address search ------//
        /// <summary>
        /// CÁI NÀY ĐỂ SAU KHI CHỌN ĐIỂM ĐÓN THÌ MAP SẼ CHẠY LẠI ĐỊA CHỈ ĐÓ
        /// </summary>
        /// <param name="inputAddress"></param>
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
            //1. Convert chuối json lấy về thành object
            GoogleAPIAddressObj places = new GoogleAPIAddressObj();
            places = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(e.Result);
            try
            {
                //Lấy tọa độ của điểm mới tìm được
                double lat = places.results[0].geometry.location.lat;
                double lng = places.results[0].geometry.location.lng;

                //Và dời vị trí map về đó
                map_RiderMap.SetView(new GeoCoordinate(lat, lng), 16, MapAnimationKind.Linear);
                //Hiện picker
                ShowGridPiker();
                //Get near Driver
                GetNearDriver();
            }
            catch (Exception)
            {

                MessageBox.Show(ConstantVariable.errInvalidAddress);
            }
        }
        //------ END set Pickup Address from address search  ------//





        //------ BEGIN Search Bar EVENT ------//
        /// <summary>
        /// HÀM NÀY ĐỂ CHẠY AUTOCOMPLETE, CỨ SAU KHI NHẤN VÀO 1 PHÍM THÌ SẼ CHẠY AC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_InputAddress_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Hiện nút xóa / đóng trên ô search
            ShowSearchCloseIcon();

            //mở long list search
            //EnableSearchLongList();

            //Chạy autocomplete và load dữ liệu vào Longlistselector
            string queryAddress = txt_InputAddress.Text;
            loadAutoCompletePlace(queryAddress);
        }



        /// <summary>
        /// Trong ô tìm kiếm, nếu như nhấn phím Enter trên bàn phím ảo, thì sẽ chạy sự kiện này
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tb_InputAddress_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Kiểm tra xem phím nhấn có phải là phím Enter ko
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                string destinationAddress;
                destinationAddress = txt_InputAddress.Text;
                this.searchCoordinateFromAddress(destinationAddress);
                this.Focus();
            }
        }


        private void txt_InputAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //KIÊM TRA XEM, NẾU LÀ TỪ "ĐỊA CHỈ ĐÓN" thì xóa
            CheckInputAddressTap();

            txt_InputAddress.Background = new SolidColorBrush(Colors.Transparent);

            //Show end of address
            setCursorAtLast(txt_InputAddress);
        }



        /// <summary>
        /// KIÊM TRA XEM, NẾU LÀ TỪ "ĐỊA CHỈ ĐÓN" thì xóa
        /// </summary>
        private void CheckInputAddressTap()
        {
            if (txt_InputAddress.Text == ConstantVariable.destiationAddressDescription)
            {
                txt_InputAddress.Text = string.Empty;
            }
        }



        /// <summary>
        /// KHI NHẤN VÀO NÚT CLOSE TRÊN SEARCH BAR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_CloseIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Nếu như textbox đang không có gì thì:
            if (txt_InputAddress.Text == string.Empty)
            {
                //Trở về màn hình chính
                map_RiderMap.Focus();

                //Hiện cụm picker
                ShowGridPiker();
            }
            else
            {
                //Nếu không thì xóa text đi rồi cho nhập lại
                txt_InputAddress.Text = String.Empty;
                txt_InputAddress.Focus();
            }
        }

        /// <summary>
        /// Cái này để xử lý tình huống khi ô tìm kiếm điểm đón được sử dụng
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txt_InputAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            EnableSearchLongList();

            //Khi nhấn vào ô search thì picker bị ẩn đi
            HideGridPicker();

            //Hiện icon xóa trên thanh Search
            ShowSearchCloseIcon();
            
            //Enable Auto Complete
            if (txt_InputAddress.Text.Length > 0)
            {
                loadAutoCompletePlace(txt_InputAddress.Text);
            }
            else
            {
                loadAutoCompletePlace("");
            }
            
            


            //Cái này để làm cho textbox trong suốt khi tap vào
            TextBox addressTextbox = (TextBox)sender;
            addressTextbox.Background = new SolidColorBrush(Colors.Transparent);
            addressTextbox.BorderBrush = new SolidColorBrush(Colors.Transparent);

            //Nếu khi tap vào mà nội dung là "Chọn điểm đón thì sẽ xóa đi
            if (txt_InputAddress.Text == ConstantVariable.destiationAddressDescription)
            {
                txt_InputAddress.Text = string.Empty;
            }

            //Đặt con trỏ chuột ở cuối dòng khi tap vào           
            setCursorAtLast(txt_InputAddress);
        }



        private void txt_InputAddress_LostFocus(object sender, RoutedEventArgs e)
        {
            //bật lại picker pin
            ShowGridPiker();

            //Nhưng tắt Red picker
            HideCallTaxiCenterPicker();

            //Ẩn close icon
            HideSearchCloseIcon();

            if (txt_InputAddress.Text == String.Empty)
            {
                txt_InputAddress.Text = ConstantVariable.destiationAddressDescription;
            }

            //Đặt con trò chuột lên đầu tiên của search box
            setCursorAtFirst(txt_InputAddress);

            //Tắt long list
            HideSearchLongList();
        }

        private void HideSearchLongList()
        {
            lls_AutoComplete.Visibility = Visibility.Collapsed;
        }



        /// <summary>
        /// Cái này để hiện thông tin sau khi picker di chuyển lên thanh search
        /// </summary>
        private async void ShowPickerAddress()
        {
            try
            {
                var str = await GoogleAPIFunction.ConvertLatLngToAddress(pickupLat, pickupLng);
                var address = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(str);
                txt_InputAddress.Text = address.results[0].formatted_address.ToString();
            }
            catch (Exception)
            {

                MessageBox.Show("(Mã lỗi 35604) " + ConstantVariable.errConnectingError);
            }
        }


        //Event này để bắt trường hợp sau mỗi lần di chuyển map
        /// <summary>
        /// SAU KHI MAP DI CHUỂN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void map_RiderMap_ResolveCompleted(object sender, MapResolveCompletedEventArgs e)
        {

            if (new GeoCoordinate(Math.Round(map_RiderMap.Center.Latitude, 5), Math.Round(map_RiderMap.Center.Longitude, 5)).Equals(new GeoCoordinate(tmpLat, tmpLng)))
            {
                grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable process bar
            }

            img_PickerLabel.Visibility = Visibility.Visible; //Enable Pickup label

            //Nạp tọa độ đón
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
            Debug.WriteLine("map_RiderMap_ResolveCompleted");
        }


        private void map_RiderMap_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            //Tắt timer của change red label
            //changeLabelRedTimer.Start();
            changeLabelRedTimer.Stop();

            //Ẩn picker label khi di chuyển map
            HideAllPickerLabel();

            //Đặt trạng thái picker là true
            isPickup = true;

            Debug.WriteLine("RiderMap_MouseLeftButtonDown");
        }




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

        //private void txt_PickupAddress_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    //Trong suốt texbox
        //    TextBox addressTextbox = (TextBox)sender;
        //    addressTextbox.Background = new SolidColorBrush(Colors.Transparent);
        //    addressTextbox.BorderBrush = new SolidColorBrush(Colors.Transparent);
        //    addressTextbox.SelectionBackground = new SolidColorBrush(Colors.Transparent);
        //}
        //------ END For open menu ------//




        private void canvas_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {

        }

        private void txt_PickupAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            setCursorAtLast(txt_PickupAddress);
        }


        //Focus to "PickupType"
        private void img_RequestTaxiButton_Tap(object sender, System.Windows.Input.GestureEventArgs e) //Check null input
        {

        }



        //This function to get City Code From City Name in City Dictionary
        private int GetCityCodeFromCityName(string cityName)
        {
            int cityCode = 0;
            try
            {
                cityCode = cityNamesDB[cityName].cityId;
            }
            catch (Exception)
            {

                MessageBox.Show("(Mã lỗi 1601) " + ConstantVariable.errServerErr);
            }
            return cityCode;
        }


        /// <summary>
        /// HIỆN TRẠNG THÁI LOADING
        /// </summary>
        private void ShowButtonRequestLoadingState()
        {
            grv_ProcessBarButton.Visibility = Visibility.Visible; //Khi bắt đầu nhấn nút "Yêu cầu Taxi" thì hệ thống sẽ hiện Process Bar "Vui lòng đợi"
        }

        private void HideButtonRequestLoadingState()
        {
            grv_ProcessBarButton.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Hàm này để yêu cầu 1 chuyến đi qua bên Driver
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_RequestTaxi_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //HIỆN LOADING
            ShowButtonRequestLoadingState();

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
                sAddr = txt_PickupAddress.Text, //Hiện tại Notification đang không gửi được thông báo với input là tiếng việt
                eAddr = txt_DestinationAddress.Text, //nên sẽ conver hai địa chỉ đi và đón qua tiếng anh
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
                createTrip.sAddr, //ConvertData.ConvertVietnamCharacter(),//Hiện tại Notification đang không gửi được thông báo với input là tiếng việt
                createTrip.eAddr, //ConvertData.ConvertVietnamCharacter(),//nên sẽ conver hai địa chỉ đi và đón qua tiếng anh
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

                //SAU KHI REQ XONG THÌ CHUYỂN QUA MÀN HÌNH "VUI LÒNG ĐỢI", đồng thời tắt LOADING
                grv_CancelTaxi.Visibility = Visibility.Visible; //Hiện màn hình "Vui lòng đợi" kèm Button "Hủy Chuyến"
                grv_ProcessBarButton.Visibility = Visibility.Collapsed; //Đóng màn hình loading

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
            //Sau khi buông tay thì cho hiện black picker
            ShowGridPiker();
            //trong khi vẫn ẩn picker gọi hãng
            HideCallTaxiCenterPicker();
        }

        private void TouchFeedback()
        {
            vibrateController.Start(TimeSpan.FromSeconds(0.1));
        }

        private void img_PickerLabel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            chk_AutoRecall.IsChecked = true;
            grv_Step02.Visibility = Visibility.Visible;
            btn_RequestTaxiMN.Visibility = Visibility.Visible;
            btn_RequestTaxi.Visibility = Visibility.Collapsed;
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
                if (tNetAppSetting.Contains("PushChannelURI"))
                {
                    tNetAppSetting["PushChannelURI"] = pushChannelURI.ToString(); //Cái này để lưu lại uri
                }
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
                if (tNetAppSetting.Contains("PushChannelURI"))
                {
                    tNetAppSetting["PushChannelURI"] = pushChannelURI.ToString(); //Cái này để lưu lại uri
                }
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
            Debug.WriteLine("f111 Nhận được thông báo"); //DELETE AFTER FINISH

            StringBuilder message = new StringBuilder();
            //string relativeUri = string.Empty;
            //message.AppendFormat("Received Toast {0}:\n", DateTime.Now.ToShortTimeString());

            // Parse out the information that was part of the message.
            foreach (string key in e.Collection.Keys)
            {
                //message.AppendFormat("{0}: {1}\n", key, e.Collection[key]);

                if (string.Compare(
                    key,
                    "wp:Param",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.CompareOptions.IgnoreCase) == 0)
                {
                    //Lấy chuỗi thông báo từ Notification
                    notificationReceivedString = e.Collection[key];
                }
            }

            // Display a dialog of all the fields in the toast.
            Dispatcher.BeginInvoke(() =>
            {
                ///Nếu app đang hoạt động thì sẽ chạy hàm này
                ///Lấy dữ liệu và show lên màn hình
                if (notificationReceivedString != string.Empty)
                {

                    //Hàm này để lấy ra chuỗi Json trong một String gửi qua notification
                    int a = notificationReceivedString.IndexOf("json=");
                    int b = notificationReceivedString.IndexOf("}");
                    int c = notificationReceivedString.IndexOf("notiType=");
                    string tmpStirng = notificationReceivedString.Substring(a + 5, b - a - 4);
                    //Cái này để lấy kiểu 
                    notificationType = notificationReceivedString.Substring(c + 9, notificationReceivedString.Length - c - 9);
                    notificationReceivedString = tmpStirng;

                    //Sau đó chạy thông báo
                    ShowNotification();
                }
            });

        }



        /// <summary>
        /// hàm này để hiện thị thông báo lên màn hình
        /// Tham số sử dụng là chuỗi notificationReceivedString lấy ra từ kênh Notification
        /// </summary>
        private void ShowNotification()
        {
            //Nếu như có tồn tại chuỗi Json
            if (notificationReceivedString != string.Empty && notificationType != string.Empty)
            {
                switch (notificationType)
                {
                    case ConstantVariable.notiTypeNewTrip: //Nếu là "NT" thì sẽ chạy hàm Show New Trip Notification
                        ShowNotificationNewTrip();
                        break;
                    case ConstantVariable.notiTypeUpdateTrip: //Nếu là "UT" thì sẽ chạy hàm Show Update Trip Notification
                        ShowNotificationUpdateTrip();
                        break;
                    case ConstantVariable.notiTypePromotionTeip: //Nếu là "PT" thì sẽ chạy hàm Show Promotion Trip Notification
                        ShowNotificationPromotionTrip();
                        break;
                }
            }
            else
            {
                MessageBox.Show("(Mã lỗi 402) " + ConstantVariable.errServerErr);
            }

        }




        /// <summary>
        /// Các trường hợp của thông báo
        /// </summary>
        private void ShowNotificationNewTrip()
        {

        }
        private void ShowNotificationUpdateTrip()
        {
            var input = notificationReceivedString;
            myTrip = new RiderNotificationUpdateTrip();
            try
            {
                myTrip = JsonConvert.DeserializeObject<RiderNotificationUpdateTrip>(input);//Tạo đối tượng UpdateTrip từ Json Input                

                ///1. Update LMD để có thể cancel chuyến
                ///2. Kiểm tra mã Notification để hiện thông báo cho khách hàng
                ///2.1 RJ - Reject
                ///2.2 PD - Picked
                ///2.3 PI - Picking
                ///2.4 CA - Cancelled
                ///2.5 TA - Trip Complete
                tlmd = myTrip.lmd;

                switch (myTrip.tStatus) //<<<<< Cái này trả về thông tin bên ông Driver. vd: Nếu nhấn Start thì status là PI
                {
                    case ConstantVariable.tripStatusPicking: //Nếu là "PI" thì sẽ chạy hàm thông báo "Xe đang tới"
                        SwitchToPikingStatus();
                        break;
                    case ConstantVariable.tripStatusPicked: //Nếu là "PD" Thì chuyến đi đã bắt đầu
                        SwitchToStartedStatus();
                        break;
                    case ConstantVariable.tripStatusReject: //Nếu là "RJ" thì sẽ chạy hàm Thông báo xe bị hủy
                        SwitchToRejectStatus();
                        break;
                    case ConstantVariable.tripStatusCancelled: //Nếu là "CA" thì sẽ chạy hàm Thông báo hủy chuyến
                        SwitchToCanceledStatus();
                        break;
                    case ConstantVariable.tripStatusComplete: //Nếu là "TC" thì sẽ chạy hàm Hoàn thành chuyến đi
                        SwitchToCompletedStatus();
                        break;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("(Mã lỗi 403) " + ConstantVariable.errHasErrInProcess);
            }
        }


        private void ShowNotificationPromotionTrip()
        {
        }


        /// <summary>
        /// CÁC TRƯỜNG HỢP CỦA UPDATE TRIP
        /// </summary>
        private void SwitchToPikingStatus()
        {
            ///0.1 CHO ÂM THANH HIỆU ỨNG
            ///Hiện thông báo
            tbl_DriverStatus.Text = ConstantVariable.strCarAreComming; //HIỆN THÔNG ÁO "XE ĐANG TỚI.."

        }

        private void SwitchToStartedStatus()
        {
            ///1. Hiện mess
            ///2. chờ sau 3 giây tắt grid
            ///3. hiện vị trí xe
            ///
            MessageBox.Show(ConstantVariable.strCarAreStarting);

            //1.
            tbl_DriverStatus.Text = ConstantVariable.strCarAreStarting;

            //2.
            Thread.Sleep(1500);

            grv_Step02.Visibility = Visibility.Collapsed;
            //3.



        }


        private void SwitchToRejectStatus()
        {
            tbl_DriverStatus.Text = ConstantVariable.strCarRejected; //HIỆN THÔNG ÁO "YÊU CẦU BỊ HỦY BỎ.."
            ///1. VIẾT TIẾP HÀM CHO VIỆC LÀM LẠI CHU TRÌNH GỌI XE HOẶC GỌI TỔNG ĐÀI
            ///2. Chuyển qua Button Gọi hãng
            ///3. CHO ÂM THANH HIỆU ỨNG            

            //2. 
            DeleteTripDate();

            //Show messeage
            MessageBox.Show(ConstantVariable.strCarRejected);

            //3.
            grv_Step02.Visibility = Visibility.Collapsed;
            SetHomeViewState();

            //4. Get near car
            GetNearDriver();
        }

        private void SwitchToCanceledStatus()
        {
            ///0. Show thoong bao
            ///1. CHO ÂM THANH HIỆU ỨNG
            ///2. Xóa thông tin trip
            ///3. Về màn hình chính

            //0.
            tbl_DriverStatus.Text = ConstantVariable.strCarCanceled; //HIỆN THÔNG ÁO "YÊU CẦU BỊ HỦY BỎ.."

            //1. 

            //2. 
            DeleteTripDate();

            //Show messeage
            MessageBox.Show(ConstantVariable.strCarCanceled);

            //3.
            grv_Step02.Visibility = Visibility.Collapsed;
            SetHomeViewState();

            //4. Get near car
            GetNearDriver();

        }



        private void SwitchToCompletedStatus()
        {
            ///CHO ÂM THANH HIỆU ỨNG
        }

        /// <summary>
        /// Nhận thông tin từ Notification
        /// Mỗi khi App không hoạt động (Ở màn hình Home, Lock, hay tắt màn hình) sẽ hiện thông báo
        /// khi ta nhấn vào thông báo, sẽ điều hướng tới trang /Pages/HomePage.xaml
        /// Và OnNavigatedTo là để lấy nội dung của thông báo
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                if (this.NavigationContext.QueryString["json"].ToString() != null)
                {
                    notificationReceivedString = this.NavigationContext.QueryString["json"].ToString(); //Gán chuỗi Json 
                    notificationType = this.NavigationContext.QueryString["amp;notiType"].ToString(); //Gán kiểu noti
                    //Sau cùng là chạy hàm hiển thị notification
                    ShowNotification();
                }

            }
            catch (KeyNotFoundException)
            {
                //MessageBox.Show("(Mã lỗi 302) " + ConstantVariable.errServerError);
            }

        }


        private void DeleteTripDate()
        {
            myTrip = null;
            createTrip = null;
        }

        /// <summary>
        /// Cái này là để cập nhật URI mỗi khi có thay đổi
        /// Cứ mỗi khi URI thay đổi, hệ thống sẽ cập nhật lên sv
        /// </summary>
        /// <param name="uri"></param>
        private async void UpdateNotificationURI(string uri)
        {
            var uid = userId;
            var mType = ConstantVariable.mTypeWIN;
            var role = ConstantVariable.dRole;
            var id = userData.content.rid;
            var input = string.Format("{{\"uid\":\"{0}\",\"mid\":\"{1}\",\"mType\":\"{2}\",\"role\":\"{3}\",\"id\":\"{4}\"}}", uid, uri, mType, role, id);
            try
            {
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderUpdateRegId, input);
            }
            catch (Exception)
            {
                //Lỗi máy chủ
                MessageBox.Show("(Mã lỗi 401) " + ConstantVariable.errServerErr);
            }

        }


        /// <summary>
        /// VIẾT HÀM HỦY BỎ YÊU CẦU VÀO ĐÂY
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_CancelTaxi_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                //Thiết lập messagebox                
                Message = ConstantVariable.cfbCancelTaxi, // "Bạn sẽ phải trả tiền gọi xe theo giá mở cửa nếu hủy chuyến."
                LeftButtonContent = ConstantVariable.cfbYes,
                RightButtonContent = ConstantVariable.cfbNo
            };

            //Add the dismissed event handler
            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        CancelTaxiTrip();
                        break;
                    case CustomMessageBoxResult.RightButton:
                        messageBox.Dismiss();

                        break;
                    case CustomMessageBoxResult.None:
                        // Do something.
                        break;
                    default:
                        break;
                }
            };

            //add the show method
            messageBox.Show();

        }

        private async void CancelTaxiTrip()
        {
            //HIỆN LOADING PROCESS
            grv_ProcessBarButton.Visibility = Visibility.Visible;

            RiderCancelTrip cancelTrip = new RiderCancelTrip
            {
                uid = userId,
                pw = pwmd5,
                tid = myTrip.tid
            };
            var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"tid\":\"{2}\"}}", cancelTrip.uid, cancelTrip.pw, cancelTrip.tid);
            try
            {
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderCancelTrip, input);
                if (output != null)
                {
                    var cancelStatus = JsonConvert.DeserializeObject<BaseResponse>(output);
                    if (cancelStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS)) //0000
                    {
                        ///Nếu như hủy bỏ chuyển thành công                        
                        ///1. Update lmd
                        ///2. Xóa tât cả các biến về Tạo một Trip
                        ///3. Trở về màn hình đầu tiên

                        //1. Update LMD cho phiên làm việc tiếp theo
                        tlmd = (long)cancelStatus.lmd;

                        //2. Xóa các thông tin liên quan đến trip
                        createTrip = null;

                        //3. Về màn hình đầu tiên
                        //SAU KHI HOÀN THÀNH REQ HỦY CHUYẾN THÌ TẮT LOADING
                        grv_ProcessBarButton.Visibility = Visibility.Visible;
                        //Đưa màn hình về trạng thái ban đầu
                        SetHomeViewState();

                    }
                    else if (cancelStatus.status.Equals(ConstantVariable.RESPONSECODE_TRIP_TAKEN)) //013
                    {
                        ///CODE CHO VIỆC THÔNG BÁO ĐÃ BỊ CHIẾM KHÁCH
                        ///CHO TRỞ VỀ MÀN HÌNH MAP
                        ///XÓA NEW TRIP
                        ///
                    }
                    else
                    {
                        Debug.WriteLine("Mã lỗi 576fg ở Cancel Trip");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("(Mã lỗi 408) " + ConstantVariable.errServerErr);
            }
            
        }



        /// <summary>
        /// HÀM NÀY ĐỂ ĐƯA MÀN HÌNH VỀ BAN ĐẦU
        /// </summary>
        private void SetHomeViewState()
        {
            grv_Step01.Visibility = Visibility.Visible;
            grv_Step02.Visibility = Visibility.Collapsed;
        }

        private void tbl_MyTrips_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/RiderMyTrip.xaml", UriKind.Relative));
        }

        private void tbl_CompanyInfo_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/CompanyInfo.xaml", UriKind.Relative));
        }


        /// <summary>
        /// Nhấn vào avatar để truy cập vào Profile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_MenuAvatar_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/RiderProfile.xaml", UriKind.Relative));
        }

        private void btn_Logout_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LogOut();
        }



        private void LogOut()
        {
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                //set the properties                
                Message = ConstantVariable.cfbLogOut, // "Bạn có chắc là bạn muốn thoát tài khoản không?";
                LeftButtonContent = ConstantVariable.cfbYes,
                RightButtonContent = ConstantVariable.cfbNo
            };

            //Add the dismissed event handler
            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        if (tNetAppSetting.Contains("isLogin"))
                        {
                            tNetAppSetting.Remove("isLogin");
                            tNetUserLoginData.Remove("UserId");
                            tNetUserLoginData.Remove("PasswordMd5");
                            tNetUserLoginData.Remove("RawPassword");
                            tNetUserLoginData.Remove("UserLmd");
                            //tNetUserLoginData.Remove("PushChannelURI");
                            NavigationService.Navigate(new Uri("/Pages/Login.xaml", UriKind.Relative));
                        }
                        break;
                    case CustomMessageBoxResult.RightButton:
                        messageBox.Dismiss();

                        break;
                    case CustomMessageBoxResult.None:
                        // Do something.
                        break;
                    default:
                        break;
                }
            };

            //add the show method
            messageBox.Show();

        }



        private void tbl_VipRider_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/RiderMyFavoriteDriver.xaml", UriKind.Relative));
        }

        private void tbl_LastName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/RiderProfile.xaml", UriKind.Relative));
        }

        private void tbl_FirstName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/RiderProfile.xaml", UriKind.Relative));
        }

        private void tbl_MyProfile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/RiderProfile.xaml", UriKind.Relative));
        }

        private void tbl_About_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/About.xaml", UriKind.Relative));
        }

        private async void btn_RequestTaxiMN_Tap(object sender, System.Windows.Input.GestureEventArgs e)
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

            // Kiểm tra khi không có xe nào xung quanh >> bug
            try
            {
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
            catch (Exception)
            {

                //Code thong bao, khong co xe nao xung quanh
                MessageBox.Show("Rất tiếc, không có xe nào xung quanh!");
            }
        }

        private void img_PickerLabel_Red_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/TaxiList.xaml", UriKind.Relative));
        }


        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //Cái này để làm cho textbox trong suốt khi tap vào
            TextBox addressTextbox = (TextBox)sender;
            addressTextbox.Background = new SolidColorBrush(Colors.Transparent);
            addressTextbox.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }


        /// <summary>
        /// CÁI NÀY LÀ KHI NHẤN VÀO NÚT CLOSE Ở DESTINATION SEARCH
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_CloseClearIcon_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Kiểm tra xem nếu lúc nhấn icon Close mà chưa có nội dung gì. thì sẽ đóng Grid Search
            if (txt_DestinationSearchAddress.Text.Equals(string.Empty))
            {
                HideGridDestinationAddressSearch();
            }

            //Nếu lúc đó có text thì xóa đi
            if (txt_DestinationSearchAddress.Text.Length > 0)
            {
                txt_DestinationSearchAddress.Text = string.Empty;
            }
        }
        /// <summary>
        /// CÁI NÀY ĐỂ CỨ SAU KHI NHẤN 1 PHÍM SẼ CHẠY AUTOCOMPLETE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            //Chạy autocomplete và load dữ liệu vào Longlistselector
            string queryAddress = txt_DestinationSearchAddress.Text;
            AutoCompleteDestinationSearch(queryAddress);
        }








        /// <summary>
        /// HIỆN ĐỊA CHỈ GỢI Ý
        /// </summary>
        private void AutoCompleteDestinationSearch(string inputAddress)
        {
            //GoogleAPIQueryAutoComplete URL
            string URL = ConstantVariable.googleAPIQueryAutoCompleteRequestsBaseURI + ConstantVariable.googleGeolocationAPIkey + "&types=geocode&language=vi" + "&input=" + inputAddress;

            //Query Autocomplete Responses to a JSON String
            WebClient proxy = new WebClient();
            proxy.DownloadStringCompleted +=
            new DownloadStringCompletedEventHandler(proxy_AutoCompleteDestinationSearch);
            proxy.DownloadStringAsync(new Uri(URL));
        }
        private void proxy_AutoCompleteDestinationSearch(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                //1. Convert Json String to an Object
                GoogleAPIQueryAutoCompleteObj places = new GoogleAPIQueryAutoCompleteObj();
                places = JsonConvert.DeserializeObject<GoogleAPIQueryAutoCompleteObj>(e.Result);
                //2. Create Place list
                ObservableCollection<AutoCompletePlaceLLSObj> autoCompleteDataSource = new ObservableCollection<AutoCompletePlaceLLSObj>();
                lls_DestinationAddress.ItemsSource = autoCompleteDataSource;
                //3. Loop to list all item in object
                foreach (var obj in places.predictions)
                {
                    autoCompleteDataSource.Add(new AutoCompletePlaceLLSObj(obj.description.ToString()));
                }
            }
            catch (Exception)
            {
                txt_DestinationSearchAddress.Focus();
            }
        }
        /// <summary>
        /// KHI NHẤN VÀO MỘT ĐỊA CHỈ TRONG LONGLIST
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lls_DestinationAddress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedPlace = ((AutoCompletePlaceLLSObj)(sender as LongListSelector).SelectedItem);
            // If selected item is null, do nothing
            if (lls_DestinationAddress.SelectedItem == null)
                return;

            //Rung phản hồi
            TouchFeedback();

            //Khi nhấn vào 1 đỉa chỉ trong danh sách tự động tìm địa chỉ thì sẽ đặt địa chỉ đến
            txt_DestinationAddress.Text = selectedPlace.Name.ToString();

            setDestinationAddressFromSearchBar(selectedPlace.Name.ToString());

            //Tắt grid tìm điểm đến
            HideGridDestinationAddressSearch();

            //Xóa lls
            lls_DestinationAddress = null;
        }
        private void setDestinationAddressFromSearchBar(string inputAddress)
        {
            //GoogleAPIGeocoding URL
            string URL = ConstantVariable.googleAPIGeocodingAddressBaseURI + inputAddress + "&key=" + ConstantVariable.googleGeolocationAPIkey;

            //Query Autocomplete Responses to a JSON String
            WebClient proxy = new WebClient();
            proxy.DownloadStringCompleted +=
            new DownloadStringCompletedEventHandler(proxy_setDestinationAddressFromSearchBar);
            proxy.DownloadStringAsync(new Uri(URL));
        }
        private void proxy_setDestinationAddressFromSearchBar(object sender, DownloadStringCompletedEventArgs e)
        {
            //1. Convert chuối json lấy về thành object
            GoogleAPIAddressObj places = new GoogleAPIAddressObj();
            places = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(e.Result);
            try
            {
                //Lấy tọa độ của điểm mới tìm được
                double lat = places.results[0].geometry.location.lat;
                double lng = places.results[0].geometry.location.lng;

                //Và dời vị trí map về đó
                map_RiderMap.SetView(new GeoCoordinate(lat, lng), 16, MapAnimationKind.Linear);
                //Hiện picker
                ShowGridPiker();


            }
            catch (Exception)
            {

                MessageBox.Show(ConstantVariable.errInvalidAddress);
            }
        }
        /// <summary>
        /// CÁI NÀY ĐỂ HIỆN MARKER ĐIỂM ĐẾN
        /// </summary>
        private void ShowDestinationMarkerOnMap()
        {

        }


        /// <summary>
        /// CÁI NÀY ĐỂ VÔ HIỆU HÓA NÚT BACK Ở HOME
        /// </summary>
        /// <param name="e"></param>
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            //base.OnBackKeyPress(e);
            //MessageBox.Show("You can not use Hardware back button");
            e.Cancel = true;
        }





        /// <summary>
        /// CÁI NÀY ĐỂ HIỆN / Ẩn MÀN HÌNH TÌM KIẾM ĐIỂM ĐẾN
        /// </summary>
        private void ShowGridDestinationAddressSearch()
        {
            (this.Resources["showDestinationSearch"] as Storyboard).Begin();
            grv_DestinationSearch.Visibility = Visibility.Visible;
        }
        private void HideGridDestinationAddressSearch()
        {
            grv_DestinationSearch.Visibility = Visibility.Collapsed;
        }






        /// <summary>
        /// CÁI NÀY ĐỂ HIỆN CỤM PICKER
        /// </summary>
        private void ShowGridPiker()
        {
            grv_Picker.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// CÁI NÀY ĐỂ ẨN CỤM PICKER
        /// </summary>
        private void HideGridPicker()
        {
            grv_Picker.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// Cái này để bật và tắt longlist trên màn hình search
        /// </summary>
        private void DisableSearchLongList()
        {
            lls_AutoComplete.Visibility = Visibility.Collapsed;
            lls_AutoComplete.IsEnabled = false;
        }

        private void EnableSearchLongList()
        {
            lls_AutoComplete.IsEnabled = true;
            lls_AutoComplete.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// Cái này để hiện cái nút màu đỏ (Gọi hãng)
        /// </summary>
        private void ShowCallTaxiCenterPicker()
        {
            img_PickerLabel_Red.Visibility = Visibility.Visible;
        }

        private void HideCallTaxiCenterPicker()
        {
            img_PickerLabel_Red.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// Cái này để hiện màn hình loading
        /// </summary>
        private void ShowLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Visible;
        }

        private void HideLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// CÁI NÀY ĐỂ HIỆN NÚT TẮT TRONG SEARCH BAR
        /// </summary>
        private void ShowSearchCloseIcon()
        {
            img_CloseIcon.Visibility = Visibility.Visible;
        }

        private void HideSearchCloseIcon()
        {
            img_CloseIcon.Visibility = Visibility.Collapsed;
        }



        /// <summary>
        /// HAI HÀM NÀY ĐỂ ĐĂT TRỎ CHUỘT VÀO ĐẦU VÀ CUỐI DÒNG
        /// </summary>
        /// <param name="txtBox"></param>
        private void setCursorAtLast(TextBox txtBox)
        {
            txtBox.SelectionStart = txtBox.Text.Length;
            txtBox.SelectionLength = 0;
        }
        private void setCursorAtFirst(TextBox txtBox)
        {
            txtBox.SelectionStart = 0;
            txtBox.SelectionLength = 0;
        }


        private void HideAllPickerLabel()
        {
            img_PickerLabel.Visibility = Visibility.Collapsed; //Disable Pickup label
            img_PickerLabel_Red.Visibility = Visibility.Collapsed;
        }



        /// <summary>
        /// KHI NHẤN VÀO TEXTBOX ĐIỂM ĐẾN SẼ HIỆN GRID SEARCH ĐIỂM ĐẾN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void txt_DestinationAddress_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowGridDestinationAddressSearch();
            txt_DestinationSearchAddress.Focus();
        }



        private void txt_DestinationSearchAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            //Xóa text khi mở grid lên
            txt_DestinationAddress.Text = string.Empty;

            //Trong suốt texbox
            TextBox addressTextbox = (TextBox)sender;
            addressTextbox.Background = new SolidColorBrush(Colors.Transparent);
            addressTextbox.BorderBrush = new SolidColorBrush(Colors.Transparent);
            //addressTextbox.SelectionBackground = new SolidColorBrush(Colors.Transparent);
        }





    }
}