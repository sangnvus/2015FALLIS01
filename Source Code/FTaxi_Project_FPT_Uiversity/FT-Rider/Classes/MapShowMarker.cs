using Microsoft.Phone.Maps.Controls;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace FT_Rider.Classes
{
    class MapShowMarker
    {
        public void ShowPointOnMap(double lat, double lng, Map map, int zoomLevel)
        {
            GeoCoordinate myCoordinate = new GeoCoordinate(lat, lng);
            map.Center = myCoordinate;
            map.ZoomLevel = zoomLevel;

            //Create icon on map
            Image addressIcon = new Image();
            addressIcon.Source = new BitmapImage(new Uri("/Images/Icons/img_MyPositionIcon.png", UriKind.Relative));

            // Create a MapOverlay to contain the circle.
            MapOverlay myOvelay = new MapOverlay();
            myOvelay.Content = addressIcon;
            myOvelay.PositionOrigin = new Point(0.5, 0.5);
            myOvelay.GeoCoordinate = myCoordinate;

            //Add to Map's Layer
            MapLayer myMaplayer = new MapLayer();
            myMaplayer.Add(myOvelay);
            map.Layers.Add(myMaplayer);
            
        }
    }
}
