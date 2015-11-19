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
            string addressObjString;

            //GoogleAPIGeocoding URL
            string URL = ConstantVariable.googleAPIGeocodingLatLngBaseURI 
                + lat.ToString().Replace(',', '.') + "," 
                + lng.ToString().Replace(',', '.') + "&key=" 
                + ConstantVariable.googleGeolocationAPIkey;

            addressObjString = await ReqAndRes.GetJsonString(URL);
            return addressObjString;
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
      
    }
}
