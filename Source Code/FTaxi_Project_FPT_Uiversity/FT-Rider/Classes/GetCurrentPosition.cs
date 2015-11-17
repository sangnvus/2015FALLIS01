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
        public async Task<Geoposition> GetGeoCoordinate()
        {
            Geolocator myGeolocator = new Geolocator();
            Geoposition myGeoposition = await myGeolocator.GetGeopositionAsync();
            return myGeoposition;
        }
    }

}
