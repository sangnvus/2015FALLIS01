using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Driver.Classes
{
    class GoogleAPIFunctions
    {
        public static async Task<string> ConvertLatLngToAddress(double lat, double lng)
        {
            string addressObjString;

            //GoogleAPIGeocoding URL
            string URL = ConstantVariable.googleAPIGeocodingLatLngBaseURI
                + lat.ToString().Replace(',', '.') + ","
                + lng.ToString().Replace(',', '.') + "&key="
                + ConstantVariable.googleGeolocationAPIkey;

            addressObjString = await ReqAndRes.GetJsonString(URL);
            return addressObjString;
        }



        public static async Task<double> GetDistance(Double sLat, Double sLng, Double eLat, Double eLng)
        {
            var URL = ConstantVariable.googleAPIDistanceMatrixBaseURI1
                + sLat + "," + sLng
                + "&destinations="
                + eLat + "," + eLng
                + ConstantVariable.googleAPIDistanceMatrixBaseURI3
                + ConstantVariable.googleGeolocationAPIkey;

            var returnString = await ReqAndRes.GetJsonString(URL);
            var distance = JsonConvert.DeserializeObject<GoogleAPIDistanceMatrixObj>(returnString);
            var rawDistance = double.Parse(distance.rows[0].elements[0].distance.value.ToString()) / 1000;
            return rawDistance;
            //{
            //"destination_addresses" : [ "143 Kim Mã, Kim Mã, Ba Đình, Hà Nội, Việt Nam" ],
            //"origin_addresses" : [ "50 Liễu Giai, Ngọc Khánh, Ba Đình, Hà Nội, Việt Nam" ],
            //"rows" : [
            //   {
            //      "elements" : [
            //         {
            //            "distance" : {
            //               "text" : "1,7 km",
            //               "value" : 1707 <<<=====
            //            },
            //            "duration" : {
            //               "text" : "5 phút",
            //               "value" : 327
            //            },
            //            "status" : "OK"
            //         }
            //      ]
            //   }
            //],
            //"status" : "OK"
            //}
        }
    }
}
