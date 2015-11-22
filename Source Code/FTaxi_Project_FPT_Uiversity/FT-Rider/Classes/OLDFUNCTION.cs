using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class OLDFUNCTION
    {
        //private async void GetCurrentCoordinate()
        //{
            //riderFirstGeolocator = new Geolocator();
            //riderFirstGeolocator.DesiredAccuracy = PositionAccuracy.High;
            //riderFirstGeolocator.MovementThreshold = 20;
            //riderFirstGeolocator.ReportInterval = 100;
            //riderFirstGeoposition = await riderFirstGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));

            //riderFirstGeolocator.PositionChanged += geolocator_PositionChanged;

            //Geocoordinate riderFirstGeocoordinate = riderFirstGeoposition.Coordinate;
            //GeoCoordinate riderFirstGeoCoordinate = ConvertData.ConvertGeocoordinate(riderFirstGeocoordinate);


            ////Adjust map on the phone screen - 0.001500 to move up the map
            //this.map_RiderMap.Center = new GeoCoordinate(riderFirstGeoposition.Coordinate.Latitude - 0.001500, riderFirstGeoposition.Coordinate.Longitude);
            //this.map_RiderMap.ZoomLevel = 16;

            ////Show maker

            //// Create a small circle to mark the current location.
            //Ellipse riderFirstPositionIcon = new Ellipse();
            //riderFirstPositionIcon.Fill = new SolidColorBrush(Color.FromArgb(255, (byte)42, (byte)165, (byte)255)); //RGB of #2aa5ff
            //riderFirstPositionIcon.Height = 15;
            //riderFirstPositionIcon.Width = 15;
            //riderFirstPositionIcon.Opacity = 100;

            //// Create a MapOverlay to contain the circle.
            //MapOverlay firstRiderLocationOverlay = new MapOverlay();
            //firstRiderLocationOverlay.Content = riderFirstPositionIcon;

            ////MapOverlay PositionOrigin to 0.9, 0. MapOverlay will align it's center towards the GeoCoordinate
            //firstRiderLocationOverlay.PositionOrigin = new Point(0.5, 0.5);
            //firstRiderLocationOverlay.GeoCoordinate = riderFirstGeoCoordinate;

            //// Create a MapLayer to contain the MapOverlay.
            //riderMapLayer = new MapLayer();
            //riderMapLayer.Add(firstRiderLocationOverlay);

            //// Add the MapLayer to the Map.
            //map_RiderMap.Layers.Add(riderMapLayer);

        //}
    }
}
