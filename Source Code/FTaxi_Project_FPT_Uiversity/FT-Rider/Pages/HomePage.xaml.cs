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

        //VibrateController
        VibrateController VibrateController = VibrateController.Default;


        //For Distance
        Double distanceMeter;

        //For menu
        double initialPosition;
        bool _viewMoved = false;


        public HomePage()
        {
            InitializeComponent();

            //to localize the ApplicationBar
            //VisualStateManager.GoToState(this, "Normal", false);
            //BuildLocalizedApplicationBar();
            
            this.GetMyPosition();            
        }

        public async void GetMyPosition()
        {
            //get position
            MyGeoPosition = await MyGeolocator.GetGeopositionAsync();
            Geocoordinate MyGeocoordinate = MyGeoPosition.Coordinate;
            GeoCoordinate MyGeoCoordinate = GeoCoordinateConvert.ConvertGeocoordinate(MyGeocoordinate);
            MyGeoPosition = await MyGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
            this.map_RiderMap.Center = MyGeoCoordinate;
            this.map_RiderMap.ZoomLevel = 15;

            //Show maker
            // Create a small circle to mark the current location.
            Ellipse myCircle = new Ellipse();
            myCircle.Fill = new SolidColorBrush(Colors.Blue);
            myCircle.Height = 15;
            myCircle.Width = 15;
            myCircle.Opacity = 30;
            // Create a MapOverlay to contain the circle.
            MapOverlay myLocationOverlay = new MapOverlay();
            myLocationOverlay.Content = myCircle;
            myLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
            myLocationOverlay.GeoCoordinate = MyGeoCoordinate;
            // Create a MapLayer to contain the MapOverlay.
            MapLayer myLocationLayer = new MapLayer();
            myLocationLayer.Add(myLocationOverlay);
            // Add the MapLayer to the Map.
            map_RiderMap.Layers.Add(myLocationLayer);
        }

        private async void GetCoordinates(String addressInput)
        {
            MyGeolocator.DesiredAccuracyInMeters = 5;


            try
            {
                MyGeoPosition = await MyGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));
                MyCoordinates.Add(new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude));
                this.map_RiderMap.Center = new GeoCoordinate(MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude);
                this.map_RiderMap.ZoomLevel = 15;
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
            Mygeocodequery.SearchTerm = addressInput;
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
                MapRoute MyMapRoute = new MapRoute(MyRoute);
                map_RiderMap.AddRoute(MyMapRoute);

                List<string> RouteList = new List<string>();
                foreach (RouteLeg leg in MyRoute.Legs)
                {
                    foreach (RouteManeuver maneuver in leg.Maneuvers)
                    {
                        RouteList.Add(maneuver.InstructionText);
                    }
                }

                MyQuery.Dispose();
                //get Distance
                distanceMeter = Math.Round(GetTotalDistance(MyCoordinates), 0); //Round double in zero decimal places
            }
        }


        //Get Distance from Your Position to Distance
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


        private void map_RiderMap_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "5fcbf5e6-e6d0-48d7-a69d-8699df1b5318";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "I5nG-B7z5bxyTGww1PApXA";
        }

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
    }
    public static class GeoCoordinateConvert
    {
        public static GeoCoordinate ConvertGeocoordinate(Geocoordinate geocoordinate)
        {
            return new GeoCoordinate
                (
                geocoordinate.Latitude,
                geocoordinate.Longitude,
                geocoordinate.Altitude ?? Double.NaN,
                geocoordinate.Accuracy,
                geocoordinate.AltitudeAccuracy ?? Double.NaN,
                geocoordinate.Speed ?? Double.NaN,
                geocoordinate.Heading ?? Double.NaN
                );
        }

    }
}