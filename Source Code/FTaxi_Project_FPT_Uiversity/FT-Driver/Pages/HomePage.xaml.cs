using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Device.Location;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading;
using System.IO.IsolatedStorage;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Devices;
using Microsoft.Phone.Notification;
using Windows.Devices.Geolocation;
using Newtonsoft.Json;
using FT_Driver.Resources;
using FT_Driver.Classes;
using System.Text;
using System.Diagnostics;

namespace FT_Driver.Pages
{
    public partial class HomePage : PhoneApplicationPage
    {
        //USER DATA
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        IsolatedStorageSettings tNetAppSetting = IsolatedStorageSettings.ApplicationSettings;

        DriverLogin userData = new DriverLogin();
        string userId = "";
        string pwmd5 = "";


        //For Store Points
        List<GeoCoordinate> driverCoordinates = new List<GeoCoordinate>();

        //For Router        
        GeocodeQuery driverGeocodeQuery = null;
        RouteQuery driverQuery = null;
        MapRoute driverMapRoute = null;
        Route driverRoute = null;

        //For get Current Location
        Geolocator driverFirstGeolocator = null;
        Geoposition driverFirstGeoposition = null;
        MapOverlay driverMapOverlay = null;
        MapLayer driverMapLayer = null;


        //VibrateController
        VibrateController vibrateController = VibrateController.Default;

        //Rider Destination Icon Overlay
        MapOverlay driverDestinationIconOverlay = null;

        //For Update Current Location
        double currentLat;
        double currentLng;
        int countForUpdateLocation = 0;

        //For process bar
        double tmpLat;
        double tmpLng;

        //For menu
        double initialPosition;
        bool _viewMoved = false;

        //For City Name
        IDictionary<string, DriverGetCityList> cityNamesDB = new Dictionary<string, DriverGetCityList>();

        //For timer
        DispatcherTimer updateLocationTimer;

        //For trip
        //IDictionary<string, DriverNewtripNotification> newTripCollection = new Dictionary<string, DriverNewtripNotification>();
        DriverNewtripNotification newTrip = null;
        DriverNotificationUpdateTrip myTrip = null;
        long tlmd;

        //For Update Notification
        string pushChannelURI = string.Empty;
        string notificationReceivedString = string.Empty;
        string notificationType = string.Empty;


        //For Trip Complete
        double startLatitude = 0;
        double startLongitude = 0;
        double endLatitude = 0;
        double endLongitude = 0;
        bool isTrack = false;
        double estimateCost = 0;
        double totalFare = 0;
        DriverCompleteTrip completeTrip = null;
        bool isCalculateFare = false;


        //For Distance
        Double distanceKm;
        VehicleInfo mySelectedVehicle;
        IsolatedStorageSettings tNetTripData = IsolatedStorageSettings.ApplicationSettings;

        //For continous tracking on map
        double fiveStepBeforeLat = 0;
        double fiveStepBeforeLng = 0;
        double fiveStepAfterLat = 0;
        double fiveStepAfterLng = 0;
        RouteQuery driverMapTrackerQuery = null;
        MapRoute driverMapTrackerRoute = null;
        MapLayer driverMapTrakerLayer = null;
        MapOverlay driverMapTrackerOverlay = null;
        bool isTrackingRoute = false;
        int countTracking = 0;

        //For calculatoe fare
        double realDistance = 0;
        double realFare = 0;

        //For complete trip flaf
        bool isFinishTrip = false; //Cái này để biết khi nào chueyens đi dừng lại, tài xế nhấn thanh toán


        //for real gps
        GeoCoordinate realCoordinate = new GeoCoordinate();
        string rawPass = string.Empty;

        public HomePage()
        {
            InitializeComponent();

            //Get User data from login
            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                mySelectedVehicle = new VehicleInfo();

                userData = (DriverLogin)tNetUserLoginData["UserLoginData"];
                userId = (string)tNetUserLoginData["UserId"];
                pwmd5 = (string)tNetUserLoginData["PasswordMd5"];
                mySelectedVehicle = (VehicleInfo)tNetUserLoginData["MySelectedVehicle"];
                rawPass = (string)tNetUserLoginData["RawPassword"];
            }

            //Tạo kênh Notification
            CreatePushChannel();

            //get First Local Position
            GetCurrentCorrdinate();

            //Open Status Screen
            grv_ProcessScreen.Visibility = Visibility.Visible; //Enable Process bar

            LoadDriverProfile();
            UpdateDriverStatus(ConstantVariable.dStatusNotAvailable); //"NA"

            updateLocationTimer = new DispatcherTimer();
            updateLocationTimer.Tick += new EventHandler(updateLocationTimer_Tick);
            updateLocationTimer.Interval = new TimeSpan(0, 0, 0, 20); //Sau năm dây sẽ chạy cập nhật nếu như lần cập nhật trước không thành công    

            //Cập nhật tọa độ của lái xe lên server
            UpdateCurrentLocation(currentLat, currentLng);

            LoadCityNameDataBase();
        }



        /// <summary>
        /// Hàm này để đặt lại mọi trạng thái điều kiện
        /// </summary>
        private void ResetFlag()
        {
            //For Router        
            driverGeocodeQuery = null;
            driverQuery = null;
            driverMapRoute = null;
            driverRoute = null;

            //For get Current Location
            driverFirstGeolocator = null;
            driverFirstGeoposition = null;
            driverMapOverlay = null;
            driverMapLayer = null;

            //For Router        
            driverGeocodeQuery = null;
            driverQuery = null;
            driverMapRoute = null;
            driverRoute = null;

            //For get Current Location
            driverFirstGeolocator = null;
            driverFirstGeoposition = null;
            driverMapOverlay = null;
            driverMapLayer = null;

            //Rider Destination Icon Overlay
            driverDestinationIconOverlay = null;

            //For Update Current Location
            currentLat = 0;
            currentLng = 0;
            countForUpdateLocation = 0;

            //For trip
            newTrip = null;
            myTrip = null;

            //For Trip Complete
            startLatitude = 0;
            startLongitude = 0;
            endLatitude = 0;
            endLongitude = 0;
            isTrack = false;
            estimateCost = 0;
            totalFare = 0;

            //For Distance
            distanceKm = 0;

            mySelectedVehicle = null;
            if (tNetTripData != null)
            {
                tNetTripData = null;
            }

            //for complete trip
            completeTrip = null;

            //For continous tracking on map
            fiveStepBeforeLat = 0;
            fiveStepBeforeLng = 0;
            fiveStepAfterLat = 0;
            fiveStepAfterLng = 0;
            driverMapTrackerQuery = null;
            driverMapTrackerRoute = null;
            isTrackingRoute = false;
            countTracking = 0;
            driverMapTrakerLayer = null;
            driverMapTrackerOverlay = null;

            isCalculateFare = false;

            realDistance = 0;
            realFare = 0;
            isFinishTrip = false;
        }





        private void updateLocationTimer_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("Chạy Update Location Timer"); //DELETE AFTER FINISH
            UpdateCurrentLocation(currentLat, currentLng);
            //throw new NotImplementedException();
        }



        /// <summary>
        /// Mỗi khi map thay đổi, hai biến currentLat và currentLng sẽ được cập nhật
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void map_DriverMap_CenterChanged(object sender, MapCenterChangedEventArgs e)
        {
            currentLat = map_DriverMap.Center.Latitude;
            currentLng = map_DriverMap.Center.Longitude;
        }

        private void map_DriverMap_ResolveCompleted(object sender, MapResolveCompletedEventArgs e)
        {
            Debug.WriteLine("DriverMap_ResolveCompleted"); //DELETE AFTER FINISH

            if (new GeoCoordinate(Math.Round(map_DriverMap.Center.Latitude, 5), Math.Round(map_DriverMap.Center.Longitude, 5)).Equals(new GeoCoordinate(tmpLat, tmpLng)))
            {
                grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable process bar
            }
        }



        //------ BEGIN Update Driver Status ------//
        private async void UpdateDriverStatus(string stt) //Chưa check try catch
        {
            //{"uid":"dao@gmail.com","pw":"b65bd772c3b0dfebf0a189efd420352d","status":"NA"}
            //Nếu đang hoạt động (AC), muốn chuyển qua chế độ nghỉ thì truyền vào "AC"
            //Nếu đang ở trạn thái nghỉ (NA) muốn chuyển qua chế độ hoạt động thì truyền vào "NA"
            //Nếu bắt đầu đón khách thì chuyển qua chế độ bận (BU)
            var uid = userId;
            var pw = pwmd5;
            var status = stt;
            var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"status\":\"{2}\"}}", uid, pw, status);
            try
            {
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverUpdateStatus, input);
                var driverUpdate = JsonConvert.DeserializeObject<BaseResponse>(output);
                if (driverUpdate.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS)) //Neu chuyen thanh cong
                {
                    Debug.WriteLine("256fght Chuyen thanh cong");
                }
                else
                {
                    MessageBox.Show("(Mã lỗi 3301) " + ConstantVariable.errServerError); //Có lỗi máy chủ
                }
            }
            catch (Exception)
            {
                Debug.WriteLine("1256ghn Update Driver Status không thành công"); //DELETE AFTER FINISH
            }

            Debug.WriteLine("879s233 Update Driver Status OK"); //DELETE AFTER FINISH
        }
        //------ END Update Driver Status ------//





        //------ BEGIN Update Current Location to Server ------//
        private async void UpdateCurrentLocation(double lat, double lng)
        {
            //{"uid":"dao@gmail.com","lat":"21.038993","lng":"105.799242"}
            if (Math.Round(lat, 0) != 0 && Math.Round(lng, 0) != 0)
            {
                var uid = userId;
                var input = string.Format("{{\"uid\":\"{0}\",\"lat\":\"{1}\",\"lng\":\"{2}\"}}", uid, lat.ToString().Replace(',', '.'), lng.ToString().Replace(',', '.'));
                try
                {
                    var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverUpdateCurrentLocation, input);
                    if (output != null)
                    {
                        var driverUpdate = JsonConvert.DeserializeObject<DriverUpdateCurrentLocation>(output);
                        //Sau khi chạy OK sẽ tắt Thread
                        updateLocationTimer.Stop();
                        Debug.WriteLine("Cập nhật vị trí xe cho Driver OK");
                        //MessageBox.Show("Cập nhật vị trí ve thành công");
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("Update Location Không OK"); //DELETE AFTER FINISH
                    ///
                }
            }
            else
            {
                updateLocationTimer.Start();
                Debug.WriteLine("Không chạy được cập nhật vị trí xe");
            }

        }
        //------ END Update Current Location to Server ------//





        //------ BEGIN get driver profile ------//
        private void LoadDriverProfile()
        {
            tbl_FirstName.Text = userData.content.driverInfo.fName;
            tbl_LastName.Text = userData.content.driverInfo.lName;

            Debug.WriteLine("Load Driver Profile OK"); //DELETE AFTER FINISH
        }
        //------ END get driver profile ------//



        //------ BEGIN get current Position ------//
        private async void GetCurrentCorrdinate()
        {

            //get position
            driverFirstGeolocator = new Geolocator();
            driverFirstGeolocator.DesiredAccuracy = PositionAccuracy.High;
            driverFirstGeolocator.MovementThreshold = 20;
            driverFirstGeolocator.ReportInterval = 100;
            driverFirstGeoposition = await driverFirstGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));


            //Add img_CurrentLocation to Map
            Image currentLocationPin = new Image();
            currentLocationPin.Source = new BitmapImage(new Uri("/Images/Icons/img_CurrentLocation.png", UriKind.Relative));
            currentLocationPin.Height = 27;
            currentLocationPin.Width = 25;

            driverMapOverlay = new MapOverlay();
            driverMapOverlay.Content = currentLocationPin; //Phải khai báo 1 lớp Overlay vì Overlay có thuộc tính tọa độ (GeoCoordinate)
            driverMapOverlay.GeoCoordinate = new GeoCoordinate(driverFirstGeoposition.Coordinate.Latitude, driverFirstGeoposition.Coordinate.Longitude);
            driverMapOverlay.PositionOrigin = new Point(0.5, 0.5);

            driverMapLayer = new MapLayer();
            driverMapLayer.Add(driverMapOverlay); //Phải khai báo 1 Layer vì không thể add trực tiếp Overlay vào Map, mà phải thông qua Layer của Map
            map_DriverMap.Layers.Add(driverMapLayer);

            //// initialize pickup coordinates
            tmpLat = Math.Round(driverFirstGeoposition.Coordinate.Latitude, 5);
            tmpLng = Math.Round(driverFirstGeoposition.Coordinate.Longitude, 5);

            driverFirstGeolocator.PositionChanged += geolocator_PositionChanged;

            //Set Center view
            map_DriverMap.SetView(driverFirstGeoposition.Coordinate.ToGeoCoordinate(), 16, MapAnimationKind.Linear);

            Debug.WriteLine("Get Current Coordinate OK"); //DELETE AFTER FINISH

        }

        private void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            Debug.WriteLine("Thay đổi vị trí map"); //DELETE AFTER FINISH
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {

                Geocoordinate geocoordinate = geocoordinate = args.Position.Coordinate;
                driverMapOverlay.GeoCoordinate = geocoordinate.ToGeoCoordinate(); //Cứ mỗi lần thay đổi vị trí, Map sẽ cập nhật tọa độ của Marker

                //For Update Current Location
                //Cứ sau 5 lần thay đổi vị trí trên Map, thì sẽ cập nhật vị trí lên server một lần
                //Việc này để giảm req lên sv và tránh hao pin cho tiết bị
                if (countForUpdateLocation > 5)
                {
                    Debug.WriteLine("Cập nhật vị trí Driver lên Server"); //DELETE AFTER FINISH
                    UpdateCurrentLocation(geocoordinate.Latitude, geocoordinate.Longitude);
                    countForUpdateLocation = 0;
                }

                //Cái này để luôn nhận đc tọa độ chuẩn
                realCoordinate = geocoordinate.ToGeoCoordinate();

                //Lấy tọa độ điểm cuối cùng của hành trình
                //isTrack sẽ được chuyển qua TRUE ở button Start
                //KHI XE DI CHUYỂN THÌ SẼ NẠP ĐIỂM CUỐI VÀO
                if (isTrack)
                {
                    SetEndCoordinateOfTrip(geocoordinate.Latitude, geocoordinate.Longitude);
                    //CalculateTripDistanceAndCost();
                }

                //Vẽ đường Tracking map trong trường hợp đã nhấn nút Start Trip
                if (isTrackingRoute == true && countTracking > 5 && isFinishTrip == false)
                {
                    //Lấy điểm cuối
                    fiveStepAfterLat = geocoordinate.Latitude;
                    fiveStepAfterLng = geocoordinate.Longitude;

                    //Vẽ Map
                    TrackingRouteOnMap(new GeoCoordinate(fiveStepBeforeLat, fiveStepBeforeLng), new GeoCoordinate(fiveStepAfterLat, fiveStepAfterLng));
                    //Tính Khoảng cách và tính tiền
                    CalculateRealDistanceAndCost(fiveStepBeforeLat, fiveStepBeforeLng, fiveStepAfterLat, fiveStepAfterLng);                    

                    //Và đặt lại điểm đầu của route
                    fiveStepBeforeLat = fiveStepAfterLat;
                    fiveStepBeforeLng = fiveStepAfterLng;

                    //Reset bộ đếm
                    countTracking = 0;
                }

                //Cái này để giảm số lần chạy code khi thay đổi vị trí map
                countForUpdateLocation++;
                countTracking++;
            });

        }

        /// <summary>
        /// Hàm này để tình khoảng cách thực
        /// </summary>
        /// <param name="fiveStepBeforeLat"></param>
        /// <param name="fiveStepBeforeLng"></param>
        /// <param name="fiveStepAfterLat"></param>
        /// <param name="fiveStepAfterLng"></param>
        private async void CalculateRealDistanceAndCost(double fiveStepBeforeLat, double fiveStepBeforeLng, double fiveStepAfterLat, double fiveStepAfterLng)
        {
            //Cộng dồn khoảng cách
            realDistance += await GoogleAPIFunctions.GetDistance(fiveStepBeforeLat, fiveStepBeforeLng, fiveStepAfterLat, fiveStepAfterLng);

            //Và tính tiền
            realFare = DriverFunctions.FareCalculate(mySelectedVehicle,realDistance);
        }


        //------ END get current Position ------//
        private async void CalculateTripDistanceAndCost()
        {
            distanceKm = await GoogleAPIFunctions.GetDistance(startLatitude, startLongitude, endLatitude, endLongitude);
            estimateCost = DriverFunctions.FareCalculate(mySelectedVehicle, distanceKm);////
        }


        private void SetEndCoordinateOfTrip(double lat, double lng)
        {
            endLatitude = lat;
            endLongitude = lng;
        }

        private void SetStartCoordinateOfTrip(double lat, double lng)
        {
            startLatitude = lat;
            startLongitude = lng;
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



        private void map_DriverMap_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "5fcbf5e6-e6d0-48d7-a69d-8699df1b5318";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "I5nG-B7z5bxyTGww1PApXA";
        }



        private void SetChangeStatusButtonIsGreen()
        {
            btn_ChangeStatus_Red.Visibility = Visibility.Collapsed;
            btn_ChangeStatus_Green.Visibility = Visibility.Visible;
        }

        private void SetChangeStatusButtonIsRed()
        {
            btn_ChangeStatus_Red.Visibility = Visibility.Visible;
            btn_ChangeStatus_Green.Visibility = Visibility.Collapsed;
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
                            tNetUserLoginData.Remove("PushChannelURI");
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




        /// <summary>
        /// NOTIFICATION CHANNEL
        /// </summary>
        private void CreatePushChannel()
        {
            HttpNotificationChannel pushChannel;
            string channelName = "FtaxiDriverChannel";
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
                ///
                ///CODE UPDATE URI HERE///
                ///

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
                ///
                ///CODE LOAD URI HERE///
                ///
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
            Debug.WriteLine("Nhận được thông báo"); //DELETE AFTER FINISH


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
            //Dispatcher.BeginInvoke(() => MessageBox.Show(message.ToString()));          
            Dispatcher.BeginInvoke(() =>
            {
                //notificationReceivedString = e.Collection.Keys.;
                //MessageBox.Show(notificationReceivedString);
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
                    case ConstantVariable.notiTypeNewTrip: //Nếu là "NT" thì sẽ chạy hàm Show New Trip Notification    <<<<< TRIP HERE
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
                MessageBox.Show("(Mã lối 308) " + ConstantVariable.errServerError);
            }

        }



        /// <summary>
        /// Cái này để phản hồi lại
        /// RUNG ĐIỆN THOẠI
        /// </summary>
        private void TouchFeedback()
        {
            vibrateController.Start(TimeSpan.FromSeconds(0.1));
        }





        /// <summary>
        /// HÀM NÀY ĐƯỢC GỌI NGAY KHI CÓ THÔNG BÁO MỚI
        /// </summary>
        private async void ShowNotificationNewTrip()
        {
            //Chạy âm thanh thông báo và rung điện thoại
            TouchFeedback();
            Alert_Trip_New();

            //Hiển thị Grid thông tin Trip lên màn hình
            ShowAcceptRejectGrid();
            //Đồng thời hiện thị Loading Grid screen
            ShowLoadingGridScreen();

            var input = notificationReceivedString;
            newTrip = new DriverNewtripNotification();
            try
            {
                newTrip = JsonConvert.DeserializeObject<DriverNewtripNotification>(input); //Tạo đối tượng NewTrip từ Json Input
                var addressString = await GoogleAPIFunctions.ConvertLatLngToAddress(newTrip.sLat, newTrip.sLng);
                var address = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(addressString);

                ///1. Nạp thông tin lên grid
                ///2. tắt Loading grid screen
                ///3. Hiển thị vị trí khách hàng có yêu cầu 

                //1. Nạp thông tin lên grid
                txt_RiderAddress.Text = address.results[0].formatted_address.ToString();
                txt_RiderMobile.Text = newTrip.mobile;
                txt_RiderName.Text = newTrip.rName;

                //2. tắt Loading grid screen
                HideLoadingGridScreen();

                var myTempCoordinate = await GetCurrentPosition.GetGeoCoordinate();
                //3. Hiển thị vị trí khách hàng có yêu cầu 
                GetRouteOnMap(myTempCoordinate, new GeoCoordinate(newTrip.sLat, newTrip.sLng));

                //4. Căn giữa hai điểm đầu cuối
                map_DriverMap.SetView(new GeoCoordinate((myTempCoordinate.Latitude + newTrip.sLat) / 2, (myTempCoordinate.Longitude + newTrip.sLng) / 2), 14, MapAnimationKind.Parabolic);


            }
            catch (Exception)
            {

                MessageBox.Show("(Mã lỗi 309) " + ConstantVariable.errHasErrInProcess);
            }
        }
        private void ShowNotificationUpdateTrip()
        {
            var input = notificationReceivedString;
            myTrip = new DriverNotificationUpdateTrip();
            try
            {
                myTrip = JsonConvert.DeserializeObject<DriverNotificationUpdateTrip>(input);//Tạo đối tượng UpdateTrip từ Json Input                

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

        private void SwitchToPikingStatus()
        {

        }
        private void SwitchToRejectStatus()
        {

        }
        private void SwitchToCanceledStatus()
        {
            ///HÀM NÀY ĐỂ XỬ LÝ KHI KHÁCH HÀNG HỦY CHUYẾN ĐI
            ///0. CÓ ÂM THANH CẢNH BÁO, CHIA BUỒN
            ///1. Hiện thông báo hủy chuyến
            ///T

            TouchFeedback();
            Alert_Trip_Cancel();
            ShowCancelGird();


        }
        private void SwitchToCompletedStatus()
        {

        }

        private void ShowCancelGird()
        {
            grv_RiderCancel.Visibility = Visibility.Visible;
        }

        private void HideCancelGird()
        {
            grv_RiderCancel.Visibility = Visibility.Collapsed;
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
            var id = userData.content.driverInfo.did;
            var input = string.Format("{{\"uid\":\"{0}\",\"mid\":\"{1}\",\"mType\":\"{2}\",\"role\":\"{3}\",\"id\":\"{4}\"}}", uid, uri, mType, role, id);
            try
            {
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverUpdateRegId, input);
                var updateStatus = JsonConvert.DeserializeObject<BaseResponse>(output);
                if (updateStatus != null)
                {
                    if (updateStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS)) //0000 OK
                    {
                        //update lmd
                        tNetUserLoginData["UserLmd"] = updateStatus.lmd;
                    }
                }
                else
                {
                    //Lỗi máy chủ
                    MessageBox.Show("(Mã lỗi 4010) " + ConstantVariable.errServerError);
                }
            }
            catch (Exception)
            {
                //Lỗi máy chủ
                MessageBox.Show("(Mã lỗi 307) " + ConstantVariable.errServerError);
            }

        }


        /// <summary>
        /// CÁI NÀY ĐỂ VẼ ĐƯỜNG ĐI GIỮA 2 điểm
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        private void GetRouteOnMap(GeoCoordinate startPosition, GeoCoordinate endPosition)
        {
            if (driverMapRoute != null)
            {
                map_DriverMap.RemoveRoute(driverMapRoute);
                driverMapRoute = null;
                driverQuery = null;
                map_DriverMap.Layers.Remove(driverMapLayer);
            }
            driverQuery = new RouteQuery()
            {
                TravelMode = TravelMode.Driving,
                Waypoints = new List<GeoCoordinate>()
            {
                startPosition, 
                endPosition
            },
                RouteOptimization = RouteOptimization.MinimizeTime
            };
            driverQuery.QueryCompleted += driverRouteQuery_QueryCompleted;
            driverQuery.QueryAsync();
        }
        void driverRouteQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            if (e.Error == null)
            {
                Route newRoute = e.Result;

                driverMapRoute = new MapRoute(newRoute);
                driverMapRoute.Color = Color.FromArgb(255, (byte)0, (byte)171, (byte)243); // aRGB for #00abf3
                driverMapRoute.OutlineColor = Color.FromArgb(255, (byte)45, (byte)119, (byte)191); //2d77bf
                map_DriverMap.AddRoute(driverMapRoute);
                //map_RiderMap.SetView(newRoute);                
                driverQuery.Dispose();

            }
        }




        /// <summary>
        /// CÁI NÀY LÀ ĐỂ KẺ LẠI MỘT ĐưỜNG MAP TRONG QUÁ TRÌNH TAXI ĐANG DI CHuyển
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        private void TrackingRouteOnMap(GeoCoordinate startPosition, GeoCoordinate endPosition)
        {
            driverMapTrackerQuery = new RouteQuery()
            {
                TravelMode = TravelMode.Driving,
                Waypoints = new List<GeoCoordinate>()
            {
                startPosition, 
                endPosition
            },
                RouteOptimization = RouteOptimization.MinimizeTime
            };
            driverMapTrackerQuery.QueryCompleted += driverTrackingRouteQuery_QueryCompleted;
            driverMapTrackerQuery.QueryAsync();
        }
        void driverTrackingRouteQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            if (e.Error == null)
            {
                Route newRoute = e.Result;

                driverMapTrackerRoute = new MapRoute(newRoute);
                driverMapTrackerRoute.Color = Color.FromArgb(255, (byte)220, (byte)29, (byte)81); // aRGB for #dc1d51
                driverMapTrackerRoute.OutlineColor = Color.FromArgb(255, (byte)193, (byte)5, (byte)5); //c10503
                map_DriverMap.AddRoute(driverMapTrackerRoute);
                //map_RiderMap.SetView(newRoute);                
                driverMapTrackerQuery.Dispose();

            }
        }


        /// <summary>
        /// Cái này để xóa route
        /// </summary>
        private void RemoveMapRoute()
        {
            if (driverMapRoute != null)
            {
                map_DriverMap.RemoveRoute(driverMapRoute);
                driverMapRoute = null;
                driverQuery = null;
                //riderMapOverlay = null;
                driverDestinationIconOverlay = null;
                driverMapLayer.Remove(driverDestinationIconOverlay);
                map_DriverMap.Layers.Remove(driverMapLayer);
            }
        }


        /// <summary>
        /// Cái này để xóa route
        /// </summary>
        private void RemoveTrakingMapRoute()
        {
            if (driverMapTrackerRoute != null)
            {
                map_DriverMap.RemoveRoute(driverMapTrackerRoute);
                driverMapTrackerRoute = null;
                driverMapTrackerQuery = null;
                //riderMapOverlay = null;
                driverMapTrackerOverlay = null;
                driverMapLayer.Remove(driverMapTrackerOverlay);
                map_DriverMap.Layers.Remove(driverMapTrakerLayer);
            }
        }


        /// <summary>
        /// KHI DRIVER CHẤP NHẬN CHUYẾN ĐI
        /// SẼ CHẠY HÀM NÀY
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_AcceptTrip_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ///CODE CHO HIỂN THỊ LOADING
            ///Bắt đầu req lên Server, sẽ hiện loading page
            ShowLoadingGridScreen();

            DriverAcceptTripObj acceptTrip = new DriverAcceptTripObj
            {
                uid = userId,
                pw = pwmd5,
                tid = newTrip.tid,
                lmd = newTrip.lmd
            };

            var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"tid\":\"{2}\",\"lmd\":\"{3}\"}}", acceptTrip.uid, acceptTrip.pw, acceptTrip.tid, acceptTrip.lmd);
            try
            {
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverAcceptTrip, input);
                if (output != null)
                {
                    var acceptStatus = JsonConvert.DeserializeObject<BaseResponse>(output);
                    if (acceptStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS)) //0000
                    {
                        //Tắt Process bar sau khi hoàn thành
                        //pb_ButtonWait.Visibility = Visibility.Collapsed;
                        ///1. CODE CHO HIỂN THỊ MÀN HÌNH BẮT ĐẦU / HỦY BỎ và tắt cái Chấp nhận / Từ chối
                        ///2. CHO ĐIỆN THOẠI RUNG
                        ///3. ĐỔ ÂM BÁO CÓ KHÁCH GỌI
                        ///4. Lưu lmd để sử dụng cho cái sau
                        ///5. Cập nhật vị trí ce 3 phút một lần
                        ///6. Trạng thái lái xe chuyên qua BUSY ("BU")
                        ///6.2 CHẠY TIMER CẬP NHẬT VỊ TRÍ XE
                        ///7. Chuyển trạng thái của Lái xe qua Picking (PI)

                        //0. Tắt loading screen
                        HideLoadingGridScreen();

                        //1.
                        ShowStartCancelGird();

                        //4.
                        tlmd = (long)acceptStatus.lmd;

                        //5.

                        //6. Trạng thái lái xe chuyên qua BUSY ("BU")
                        UpdateDriverStatus(ConstantVariable.dStatusBusy);
                    }
                    else if (acceptStatus.status.Equals(ConstantVariable.RESPONSECODE_TRIP_TAKEN)) //013
                    {
                        ///CODE CHO VIỆC THÔNG BÁO ĐÃ BỊ CHIẾM KHÁCH
                        ///CHO TRỞ VỀ MÀN HÌNH MAP
                        ///XÓA NEW TRIP
                        ///
                        MessageBox.Show("Đã có tài xế chấp nhận yêu cầu trước bạn. Chuyến đi sẽ bị hủy!");

                        RemoveMapRoute();

                        ResetFlag();

                        SetViewAtHomeState();

                        DeleteTrip();
                    }
                    else
                    {
                        Debug.WriteLine("Mã lỗi 8fefe ở AcepTrip");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("(Mã lỗi 310) " + ConstantVariable.errServerError);
            }

        }


        /// <summary>
        /// KHI DRIVER TỪ CHỐI CHUẾN ĐI
        /// SẼ CHẠY HÀM NÀY
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_RejectTrip_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            
            //Show loading Sceen
            ShowLoadingGridScreen();

            DriverAcceptTripObj rejectTrip = new DriverAcceptTripObj
            {
                uid = userId,
                pw = pwmd5,
                tid = newTrip.tid,
                lmd = newTrip.lmd
            };

            var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"tid\":\"{2}\",\"lmd\":\"{3}\"}}", rejectTrip.uid, rejectTrip.pw, rejectTrip.tid, rejectTrip.lmd);
            try
            {
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverAcceptTrip, input);
                if (output != null)
                {
                    var rejectStatus = JsonConvert.DeserializeObject<BaseResponse>(output);
                    if (rejectStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS)) //0000
                    {
                        ///1. Update lmd
                        ///2. Xóa toàn bộ thông tin Trip
                        ///3. CHUYỂN VỀ TRẠNG THÁI KHÔNG PHỤC VỤ
                        ///4. TRỞ VỀ MÀN HÌNH HOME
                        ///note. Để lấy lại trạng thái sẵn sàng thì a. THoát và đăng nhập lại b. Lựa chọn (Hình F trong tài liệu)
                        
                        //1.  Update lmd
                        tlmd = (long)rejectStatus.lmd;

                        /*
                        //2. Xóa toàn bộ thông tin Trip
                        //DeleteTrip();
                        ResetFlag();

                        //3. CHUYỂN VỀ TRẠNG THÁI KHÔNG PHỤC VỤ
                        UpdateDriverStatus(ConstantVariable.dStatusAvailable); //Để chuyển thành Not Available thì gửi lên "AC"
                        ShowChangeStatusButton(); // <=========== Cần kích hoạt nút này lên để chuyển qua chế độ off

                        //Trước khi về màn hình home thì tắt loading screen
                        HideLoadingGridScreen();
                        // grv_AcceptReject.Visibility = Visibility.Collapsed;

                        //Xóa toàn bộ route
                        RemoveMapRoute();
                        RemoveTrakingMapRoute();

                        //4.TRỞ VỀ MÀN HÌNH HOME
                        SetViewAtHomeState();
                        */
                        ResetAllData();

                    }
                    else
                    {
                        Debug.WriteLine("Mã lỗi 5frt ở RejectTrip");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("(Mã lỗi 312) " + ConstantVariable.errServerError);
            }
        }


        private void ResetAllData()
        {
            //2. Xóa toàn bộ thông tin Trip
            //DeleteTrip();
            ResetFlag();

            //3. CHUYỂN VỀ TRẠNG THÁI KHÔNG PHỤC VỤ
            UpdateDriverStatus(ConstantVariable.dStatusAvailable); //Để chuyển thành Not Available thì gửi lên "AC"
            ShowChangeStatusButton(); // <=========== Cần kích hoạt nút này lên để chuyển qua chế độ off

            //Trước khi về màn hình home thì tắt loading screen
            HideLoadingGridScreen();
            // grv_AcceptReject.Visibility = Visibility.Collapsed;

            //Xóa toàn bộ route
            RemoveMapRoute();
            RemoveTrakingMapRoute();

            //4.TRỞ VỀ MÀN HÌNH HOME
            SetViewAtHomeState();
        }


        private async void btn_StartTrip_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Hiển thị Loading grid
            ShowLoadingGridScreen();

            //Kích hoạt lấy tọa độ điểm cuối
            isTrack = true;

            //Xóa đường đi hiện tại trên map
            RemoveMapRoute();

            //Và khai báo tọa độ điểm đầu của chuyến đi (Chính là địa điểm hiện tại)
            SetStartCoordinateOfTrip(currentLat, currentLng);

            DriverStartTripObj startTrip = new DriverStartTripObj
            {
                uid = userId,
                //pw = pwmd5,
                tid = newTrip.tid,
                //status = ConstantVariable.tripStatusPicked, //"PD"
                lmd = tlmd //Cái này bây giờ không còn là của lmd Create trip nữa. mà của Accept Trip
            };


            //Gửi thông tin lên sv
            var input = string.Format("{{\"uid\":\"{0}\",\"tid\":\"{1}\",\"lmd\":\"{2}\"}}", startTrip.uid, startTrip.tid, startTrip.lmd);
            try
            {
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverStartTrip, input);
                if (output != null)
                {
                    var startStatus = JsonConvert.DeserializeObject<BaseResponse>(output);
                    if (startStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS)) //0000
                    {

                        //Nếu thành công thì
                        ///1. Update lmd
                        ///2 Hiện Button "chạm để thanh toán"
                        ///3. Hiển thị giá, quãng đường, cập nhật sau 20s

                        //1.  Update lmd
                        tlmd = (long)startStatus.lmd;

                        //2. Tắt loading grid
                        HideLoadingGridScreen();

                        //Vẽ một đường map từ vị trí hiện tại đến điểm đến (Nếu có)
                        //Nếu khách hàng cung cấp địa chỉ đến thì mới chạy hàm này
                        if (newTrip.eLat != 0 && newTrip.eLng != 0)
                        {
                            GetRouteOnMap(new GeoCoordinate(newTrip.sLat, newTrip.sLng), new GeoCoordinate(newTrip.eLat, newTrip.eLng));
                            
                        }


                        //Hiện button "Tap to pay"
                        ShowStartTripScreen();

                        //Bật khóa để cho phép tracking route
                        //Từ bây giờ bắt đầu vẽ map
                        isTrackingRoute = true;

                        //Bật hàm tính tiền
                        isCalculateFare = true;

                        //Gán điểm step đầu và cuối
                        //Ngay khi ấn nút Start Trip thì sẽ gán điểm Đầu vào 
                        //Khi nào map di chuyển đc 5 lần sẽ gán điểm Cuối > Vẽ Map > rôi chueyenr điểm cuối thành điểm đầu
                        fiveStepBeforeLat = currentLat;
                        fiveStepBeforeLng = currentLng;

                        map_DriverMap.SetView(realCoordinate, 16, MapAnimationKind.Linear);

                    }
                    else
                    {
                        Debug.WriteLine("Mã lỗi 7hgtr4 ở StartTrip");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("(Mã lỗi 312) " + ConstantVariable.errServerError);
            }
        }
        

        private async void btn_CancelTrip_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Hiện màn hình Loading
            ShowLoadingGridScreen();

            DriverStartTripObj cancelTrip = new DriverStartTripObj
            {
                uid = userId,
                //pw = pwmd5,
                tid = newTrip.tid,
                //status = "", //không truyền lên status
                lmd = tlmd //Cái này bây giờ không còn là của lmd Create trip nữa. mà của Accept Trip
            };

            var input = string.Format("{{\"uid\":\"{0}\",\"tid\":\"{1}\",\"lmd\":\"{2}\"}}", cancelTrip.uid, cancelTrip.tid, cancelTrip.lmd);
            try
            {
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverCancelTrip, input);
                if (output != null)
                {
                    var cancelStatus = JsonConvert.DeserializeObject<BaseResponse>(output);
                    if (cancelStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS)) //0000
                    {
                        ///1. Update lmd
                        ///2. Xóa toàn bộ thông tin Trip
                        ///3. CHUYỂN VỀ TRẠNG THÁI KHÔNG PHỤC VỤ
                        ///4. TRỞ VỀ MÀN HÌNH HOME

                        //1. Update lmd
                        tlmd = (long)cancelStatus.lmd;
                        /*
                        //2. Xóa toàn bộ thông tin trip
                        //DeleteTrip();
                        ResetFlag();

                        //3. CHUYỂN VỀ TRẠNG THÁI KHÔNG PHỤC VỤ
                        UpdateDriverStatus(ConstantVariable.dStatusAvailable); //Chuyển qua không phục vụ - gửi lên AC   
                        ShowChangeStatusButton(); // <=========== Cần kích hoạt nút này lên để chuyển qua chế độ off

                        //Trước khi về màn hình home thì tắt loading screen
                        HideLoadingGridScreen();

                        //Xóa toàn bộ route
                        RemoveMapRoute();
                        RemoveTrakingMapRoute();

                        //4.TRỞ VỀ MÀN HÌNH HOME
                        SetViewAtHomeState();
                        */

                        ResetAllData();
                    }
                    else
                    {
                        Debug.WriteLine("Mã lỗi 5ew33 ở CancelTrip");
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("(Mã lỗi 313) " + ConstantVariable.errServerError);
            }
        }


        private void ShowChangeStatusButton()
        {
            btn_ChangeStatus_Green.Visibility = Visibility.Collapsed;
            btn_ChangeStatus_Red.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// NHẤN NÚT NÀY ĐỂ CHUYỂN QUA TRANG THANH TOÁN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_TapToPay_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                //set the properties                
                Message = ConstantVariable.cfbTapToPay, // "Bạn có chắc là bạn muốn kết thúc chuyến đi không?";
                LeftButtonContent = ConstantVariable.cfbYes,
                RightButtonContent = ConstantVariable.cfbNo
            };

            //Add the dismissed event handler
            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        TapToPay();
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

        private async void TapToPay()
        {
            //Thông báo là đã kết thúc chuyến đi rồi
            isFinishTrip = true;

            //Khai báo điểm cuối hành trình
            SetEndCoordinateOfTrip(currentLat, currentLng);

            //Hiện loading grid screen
            ShowTapToPayLoadingScreen();

            //Chuyển đổi tọa độ qua địa chỉ
            var endAddressString = await GoogleAPIFunctions.ConvertLatLngToAddress(endLatitude, endLongitude);
            var endAddress = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(endAddressString);
            var startAddressString = await GoogleAPIFunctions.ConvertLatLngToAddress(startLatitude, startLongitude);
            var startAddress = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(startAddressString);

            //Tạo obj Complete Trip //
            completeTrip = new DriverCompleteTrip
            {
                uid = userId,
                pw = "",
                tid = newTrip.tid,
                eAdd = endAddress.results[0].formatted_address.ToString(),//Cái này để lấy địa chỉ từ tọa độ
                eCityName = endAddress.results[0].address_components[endAddress.results[0].address_components.Count - 2].long_name, //"Hà Nội"
                eLat = endLatitude,
                eLng = endLongitude,
                dis = 0, //Cập nhật Discount vào đây
                fare = estimateCost,
                lmd = tlmd,
                eCityId = GetCityCodeFromCityName(endAddress.results[0].address_components[endAddress.results[0].address_components.Count - 2].long_name)
            };

            //Sau cùng, Hiện grid Thanh toán
            ShowBillDetailGrid();

            //Hiện thông tin lên bill
            txt_BD_RiderName.Text = newTrip.rName;
            txt_BD_Mobile.Text = newTrip.mobile;
            txt_BD_From.Text = startAddress.results[0].formatted_address.ToString();//Cái này để lấy địa chỉ từ tọa độ
            txt_BD_To.Text = endAddress.results[0].formatted_address.ToString();//Cái này để lấy địa chỉ từ tọa độ
            txt_BD_Route.Text = realDistance.ToString();
            txt_BD_Cost.Text = realFare.ToString();
            txt_BD_Discount.Text = "0.0";
            txt_BD_TotalCost.Text = realFare.ToString();
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

                MessageBox.Show("(Mã lỗi 5301) " + ConstantVariable.errServerError);
            }
            return cityCode;
        }



        private void ShowInforWhenStartTrip()
        {
            txt_DistanceKm.Text = distanceKm.ToString();
            txt_PricePerDistance.Text = estimateCost.ToString();
            txt_PromotionPrice.Text = "0.0";
            txt_RiderNameStartTrip.Text = newTrip.rName;
            txt_TotalPrice.Text = estimateCost.ToString();

        }




        //Check exxception input // and cntry is VN
        private async void LoadCityNameDataBase()
        {
            //{"uid":"apl.ytb2@gmail.com","pw":"Abc123!","lan":"VI","cntry":"VN"}
            var uid = userId;
            var pw = rawPass;
            var lan = "VI";
            var cntry = "VN";
            var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"lan\":\"{2}\",\"cntry\":\"{3}\"}}", uid, pw, lan, cntry);
            var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverGetCityName, input);
            DriverGetCityNames cityItem;
            try
            {

                cityItem = JsonConvert.DeserializeObject<DriverGetCityNames>(output);
                if (cityItem != null)
                {
                    foreach (var item in cityItem.content.list)
                    {
                        cityNamesDB[item.cityName] = new DriverGetCityList
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
                }else
                {
                    Debug.WriteLine(ConstantVariable.errServerError);
                }
            }
            catch (NullReferenceException)
            {

                MessageBox.Show(ConstantVariable.errServerError);
            }
        }


        private void DeleteTrip()
        {
            Debug.WriteLine("Đã xóa dữ liệu chuyến đi");
            myTrip = null;
            newTrip = null;
        }


        /// <summary>
        /// TẤT CẢ CÁC HÀM SHOW và HIDE trạng thái màn hình
        /// </summary>
        private void ShowLoadingGridScreen()
        {
            Debug.WriteLine("ShowLoadingGridScreen");
            grv_LoadingGridScreen.Visibility = Visibility.Visible;
        }

        private void HideLoadingGridScreen()
        {
            Debug.WriteLine("HideLoadingGridScreen");
            grv_LoadingGridScreen.Visibility = Visibility.Collapsed;
        }
        private void ShowAcceptRejectGrid()
        {
            Debug.WriteLine("ShowAcceptRejectGrid");

            (this.Resources["showAcceptRejectGrid"] as Storyboard).Begin();
            grv_AcceptReject.Visibility = Visibility.Visible;
            grv_TapToPayProcess.Visibility = Visibility.Collapsed;
        }

        private void ShowStartCancelGird()
        {
            Debug.WriteLine("ShowStartCancelGird");
            grv_StartCancelbtn.Visibility = Visibility.Visible; //Bật cụm button Start / Cancel
            grv_AcceptRejectbtn.Visibility = Visibility.Collapsed; //Tắt cụm button Accept / Reject
        }

        private void ShowStartTripScreen()
        {
            Debug.WriteLine("ShowStartTripScreen");

            (this.Resources["showStartTripGrid"] as Storyboard).Begin();
            grv_AcceptReject.Visibility = Visibility.Collapsed;
            grv_TapToPayProcess.Visibility = Visibility.Collapsed;
            grv_StartTrip.Visibility = Visibility.Visible;
        }

        private void ShowTapToPayLoadingScreen()
        {
            Debug.WriteLine("ShowTapToPayLoadingScreen");
            grv_TapToPayProcess.Visibility = Visibility.Visible;
        }

        private void HideTapToPayLoadingScreen()
        {
            Debug.WriteLine("HideTapToPayLoadingScreen");
            grv_TapToPayProcess.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// HÀM NÀY ĐỂ ĐƯA DRIVER VỀ MÀN HÌNH CHÍNH
        /// </summary>
        private void SetViewAtHomeState()
        {
            grv_ChangeStatus.Visibility = Visibility.Visible;
            btn_ChangeStatus_Green.Visibility = Visibility.Visible;
            btn_ChangeStatus_Red.Visibility = Visibility.Collapsed;

            grv_AcceptReject.Visibility = Visibility.Collapsed;
            grv_StartCancelbtn.Visibility = Visibility.Collapsed;

            grv_RiderCancel.Visibility = Visibility.Collapsed;

            grv_StartTrip.Visibility = Visibility.Collapsed;
            grv_ProcessScreen.Visibility = Visibility.Collapsed;

            grv_BillDetail.Visibility = Visibility.Collapsed;

            ResetMyCoordinate();
        }


        /// <summary>
        /// Đặt lại vị trí đang đứng
        /// </summary>
        private async void ResetMyCoordinate()
        {
            GeoCoordinate currentPosition = await GetCurrentPosition.GetGeoCoordinate();
            //Đưa về vị trí ban đầu
            map_DriverMap.SetView(currentPosition, 16, MapAnimationKind.Parabolic);
        }

        private void btn_ChangeStatus_Red_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Chuyển qua trang chọn xe để kích hoạt trạng thái
            NavigationService.Navigate(new Uri("/Pages/DriverCarList.xaml", UriKind.Relative));
        }

        private void btn_ChangeStatus_Green_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ChangeDriverStatus();
        }

        private void ChangeDriverStatus()
        {
            CustomMessageBox messageBox = new CustomMessageBox()
            {
                //set the properties                
                Message = ConstantVariable.cfbChangeStatus,
                LeftButtonContent = ConstantVariable.cfbYes,
                RightButtonContent = ConstantVariable.cfbNo
            };

            //Add the dismissed event handler
            messageBox.Dismissed += (s1, e1) =>
            {
                switch (e1.Result)
                {
                    case CustomMessageBoxResult.LeftButton:
                        ShowLoadingGridScreen();
                        // Update Status here
                        // Change Button Color Here after change 
                        UpdateDriverStatus(ConstantVariable.dStatusAvailable); //Truyền lên AC để chuyển qua NotAvailable

                        //2 CHuyển trạng thái button 
                        SetChangeStatusButtonIsRed();

                        HideLoadingGridScreen();

                        MessageBox.Show(ConstantVariable.strStatusNotAvaiable);// Không hoạt động
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
            messageBox.Show();
        }

        private void tbl_MyProfile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            NavigationService.Navigate(new Uri("/Pages/DriverProfile.xaml", UriKind.Relative));
        }

        private void txt_RiderMobile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            DriverFunctions.CallToNumber(txt_RiderName.Text, txt_RiderMobile.Text);
        }


        //Cái này để tạo hiệu ứng cho nút gọi phone
        private void img_RiderMobile_Button_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            img_RiderMobile_Button.Source = new BitmapImage(new Uri("/Images/Grid_AcceptReject/img_AcceptRejectGridPhoneButton_Clicked.jpg", UriKind.Relative));
        }

        private void img_RiderMobile_Button_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            img_RiderMobile_Button.Source = new BitmapImage(new Uri("/Images/Grid_AcceptReject/img_AcceptRejectGridPhoneButton.jpg", UriKind.Relative));
        }

        /// <summary>
        /// Cái này để chạy âm thanh
        /// </summary>
        private void Alert_Trip_Start()
        {
            me_Trip_StartTrip.Play();
        }
        private void Alert_Trip_Cancel()
        {
            me_Trip_CancelTrip.Play();
        }
        private void Alert_Trip_Comlete()
        {
            me_Trip_StartTrip.Play();
        }
        private void Alert_Trip_Update1()
        {
            me_Trip_Update1.Play();
        }
        private void Alert_Trip_Update2()
        {
            me_Trip_Update2.Play();
        }
        private void Alert_Trip_New()
        {
            me_Trip_NewTrip.Play();
        }


        private void ShowBillDetailGrid()
        {
            (this.Resources["showBillDetailGrid"] as Storyboard).Begin();
            grv_BillDetail.Visibility = Visibility.Visible;
        }

        private void HideBillDetailGrid()
        {
            grv_BillDetail.Visibility = Visibility.Collapsed;
        }


        /// <summary>
        /// Khi nhấn vào nút này sẽ chạy hàm Complete Trip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btn_Payment_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Kiểm tra mật khẩu
            //MD5.MD5 pw = new MD5.MD5();
            //pw.Value = txt_Password.ActionButtonCommandParameter.ToString();
            string pw = pwmd5;
            var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"tid\":\"{2}\",\"eAdd\":\"{3}\",\"eCityName\":\"{4}\",\"eLat\":\"{5}\",\"eLng\":\"{6}\",\"dis\":\"{7}\",\"fare\":\"{8}\",\"lmd\":\"{9}\",\"eCityId\":\"{10}\"}}", completeTrip.uid, pw, completeTrip.tid, completeTrip.eAdd, completeTrip.eCityName, completeTrip.eLat.ToString().Replace(',', '.'), completeTrip.eLng.ToString().Replace(',', '.'), completeTrip.dis, completeTrip.fare, completeTrip.lmd, completeTrip.eCityId);
            try
            {
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverCompleteTrip, input);
                if (output != null)
                {
                    var completeStatus = JsonConvert.DeserializeObject<BaseResponse>(output);
                    if (completeStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS))
                    {
                        ///1. HIện thông báo thành ôcng
                        ///2. xóa toàn bộ thôn tin trip
                        ///3. Về màn hình Home

                        //1
                        MessageBox.Show("Thanh toán thành công. Chúc bạn ngày làm việc hiệu quả!");

                        //2.
                        //tNetTripData.Remove("CompleteTripBill");
                        DeleteTrip();
                        RemoveMapRoute();
                        RemoveTrakingMapRoute();
                        ResetFlag();

                        //3.
                        SetViewAtHomeState();
                        
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("(Mã lỗi 901) " + ConstantVariable.errServerError);
                Debug.WriteLine("Mã lỗi 15fht không lấy get json string từ completetrip");
            }
            //Xóa dữ liệu
        }

        private void txt_BD_Mobile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            DriverFunctions.CallToNumber(txt_BD_RiderName.Text, txt_BD_Mobile.Text);
        }

        private void btn_ConfirmCancelFromRider_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ///HỦY TOÀN BỘ THÔNG TIN TRIP
            ///TẮT GRID CANCEL
            ///VỀ MÀN HÌNH CHÍNH
            //
            DeleteTrip();
            SetViewAtHomeState();
            HideCancelGird();
            ResetFlag();

        }
    }
}
