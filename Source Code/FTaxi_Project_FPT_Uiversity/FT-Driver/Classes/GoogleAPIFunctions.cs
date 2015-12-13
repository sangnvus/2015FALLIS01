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


        public async static Task<string> GetCountryNameFromCoordinate(double lat, double lng)
        {
            string cntryName;
            var str = await GoogleAPIFunctions.ConvertLatLngToAddress(lat, lng);
            var address = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(str);
            cntryName = address.results[0].address_components[address.results[0].address_components.Count - 1].short_name;

            return cntryName;
        }


        public async static Task<string> GetCityNameFromCoordinate(double lat, double lng)
        {
            string cityName;
            var str = await GoogleAPIFunctions.ConvertLatLngToAddress(lat, lng);
            var address = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(str);
            cityName = address.results[0].address_components[address.results[0].address_components.Count - 2].long_name;

            return cityName;
        }



        public static async Task<GoogleAPIAddressObj> ConvertAddressToLatLng(string address)
        {
            GoogleAPIAddressObj latLngObj = new GoogleAPIAddressObj();

            //GoogleAPIGeocoding URL
            string URL = ConstantVariable.googleAPIGeocodingAddressBaseURI
                + address
                + "&key="
                + ConstantVariable.googleGeolocationAPIkey;

            string json = await ReqAndRes.GetJsonString(URL);
            latLngObj = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(json);

            return latLngObj;
        }







        //Input Address (ex: 18 Pham Hung)
        //Output a json incluted detail of address 18 Pham Hung
        public static async Task<GoogleAPIQueryAutoCompleteObj> ConvertAutoCompleteToLLS(string address)
        {
            string URL = ConstantVariable.googleAPIQueryAutoCompleteRequestsBaseURI
                + ConstantVariable.googleGeolocationAPIkey
                + "&input="
                + address;

            //Get Json string
            string addressObjString;
            addressObjString = await ReqAndRes.GetJsonString(URL);

            GoogleAPIQueryAutoCompleteObj addressObj = new GoogleAPIQueryAutoCompleteObj();
            addressObj = JsonConvert.DeserializeObject<GoogleAPIQueryAutoCompleteObj>(addressObjString);

            return addressObj;
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
            var rawDistance = distance.rows[0].elements[0].distance.text.ToString();
            rawDistance = rawDistance.ToLower();
            rawDistance.Replace("km", "");
            rawDistance.Replace(",", ".");
            var kilo = double.Parse(rawDistance);
            return kilo;
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
