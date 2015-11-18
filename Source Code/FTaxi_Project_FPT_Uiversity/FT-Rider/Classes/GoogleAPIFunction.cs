using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class GoogleAPIFunction
    {

        public static async Task<string> ConvertLatLngToAddress(double lat, double lng)
        {
            string address;

            //GoogleAPIGeocoding URL
            string URL = ConstantVariable.googleAPIGeocodingLatLngBaseURI 
                + lat.ToString().Replace(',', '.') + "," 
                + lng.ToString().Replace(',', '.') + "&key=" 
                + ConstantVariable.googleGeolocationAPIkey;

            address = await ReqAndRes.GetJsonString(URL);
            return address;
        }
      
    }
}
