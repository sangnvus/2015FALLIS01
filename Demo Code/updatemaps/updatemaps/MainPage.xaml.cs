using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using updatemaps.Resources;
using System.Device.Location; // Provides the GeoCoordinate class.
using Windows.Devices.Geolocation; //Provides the Geocoordinate class.
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Phone.Maps.Controls;
using Newtonsoft.Json;
using Microsoft.Phone.Maps.Services;

namespace updatemaps
{
    public partial class MainPage : PhoneApplicationPage
    {
        //Khai báo đối tượng lưu lại tọa độ
        SaveCoordinate MasterPosition = new SaveCoordinate();

        //Khai báo biến dùng cho kẻ đường
        RouteQuery MyQuery = null;
        GeocodeQuery Mygeocodequery = null;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            getMyPosition();
        }

        public async void getMyPosition()
        {
            // Get my current location.
            Geolocator myGeolocator = new Geolocator();
            Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
            Geocoordinate myToado = myGeoposition.Coordinate;
            //Lấy tọa độ
            GeoCoordinate toaDoCuaToi = ChuyenDoiToaDo.ConvertGeocoordinate(myToado);
            myGeoposition = await myGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10)); //TimeSpan.FromMinutes(1) return 00:01:00
            this.mymaps.Center = toaDoCuaToi;
            this.mymaps.ZoomLevel = 15;
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
            myLocationOverlay.GeoCoordinate = toaDoCuaToi;
            // Create a MapLayer to contain the MapOverlay.
            MapLayer myLocationLayer = new MapLayer();
            myLocationLayer.Add(myLocationOverlay);
            // Add the MapLayer to the Map.
            mymaps.Layers.Add(myLocationLayer);


            //show lat and lng
            txtLat.Text = toaDoCuaToi.Latitude.ToString();
            txtLng.Text = toaDoCuaToi.Longitude.ToString();

            ShowGoogleMapPoint(toaDoCuaToi.Latitude, toaDoCuaToi.Longitude);
        }



        private void ShowGoogleMapPoint(double a, double b)
        {


            //GeoCoordinate toaDo2 = new GeoCoordinate { Latitude = (Double)21.0277644, Longitude = (Double)105.8341598 }; //Gán tọa độ cứng vào bản đồ để tạo maker
            string URL = string.Format(@"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0},{1}&radius=500&types=food&key=AIzaSyAOi7TswVYRlkqvZcQ88Qf9SUHODK67TR0", a.ToString().Replace(',', '.'), b.ToString().Replace(',', '.'));
            //get Json
            WebClient proxy = new WebClient();
            proxy.DownloadStringCompleted +=
            new DownloadStringCompletedEventHandler(proxy_DownloadStringCompleted);
            proxy.DownloadStringAsync(new Uri(URL));

        }


        private void proxy_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            //1. Convert Json String to an Object
            JSON_NearbySearch account = JsonConvert.DeserializeObject<JSON_NearbySearch>(e.Result);
            // Create a MapLayer to contain the MapOverlay.
            MapLayer myGoogleLayer = new MapLayer();
            //2. Loop to list all item in object
            foreach (var obj in account.results)
            {
                GeoCoordinate GoogleCoordinate = new GeoCoordinate(obj.geometry.location.lat, obj.geometry.location.lng);
                // Create a small circle to mark the current location.
                Ellipse myCircle = new Ellipse();
                myCircle.Fill = new SolidColorBrush(Colors.Red);
                myCircle.Height = 10;
                myCircle.Width = 10;
                myCircle.Opacity = 50;
                // Create a MapOverlay to contain the circle.
                MapOverlay myGoogleOvelay = new MapOverlay();
                myGoogleOvelay.Content = myCircle;
                myGoogleOvelay.PositionOrigin = new Point(0.5, 0.5);
                myGoogleOvelay.GeoCoordinate = GoogleCoordinate;

                myGoogleLayer.Add(myGoogleOvelay);

            }

            // Add the MapLayer to the Map.
            mymaps.Layers.Add(myGoogleLayer);
        }




        private void myMap_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "ApJsz76A5DIw8fg7oU2F1nI2wO2p7xssazIh1SdqA_LcZskorBXeV9YEYdcG3GqI";
        }
    }


    public static class ChuyenDoiToaDo
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