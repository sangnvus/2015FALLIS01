using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Controls;

namespace FT_Rider.Classes
{

    class GetCurrentPosition
    {
        public static async Task<GeoCoordinate> GetGeoCoordinate()
        {
            Geolocator myGeolocator = new Geolocator();
            Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
            Geocoordinate myGeocoordinate = myGeoposition.Coordinate;
            GeoCoordinate myGeoCoordinate = ConvertData.ConvertGeocoordinate(myGeocoordinate);

            return myGeoCoordinate;
        }

        //How to uses?
        //GeoCoordinate newGeo = new GeoCoordinate();
        //newGeo = await GetCurrentPosition.GetGeoCoordinate();
    }

}
