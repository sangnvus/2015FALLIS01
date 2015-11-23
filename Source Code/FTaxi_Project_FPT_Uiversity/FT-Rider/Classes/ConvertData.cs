using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace FT_Rider.Classes
{
    class ConvertData
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


        //Convert String to Datetime
        public static DateTime ConvertStringToDateTime(string inputDateTime)
        {
            DateTime dt;
            dt = Convert.ToDateTime(inputDateTime);
            return dt;
        }

        //Convert Datetime to string
        public static string ConvertDateTimeToString(DateTime inputDateTime)
        {
            string dt;
            dt = Convert.ToString(inputDateTime);
            return dt;
        }

        //Format Datetime
        public static string FormatDateTime(DateTime inputDateTime)
        {
            string dt;
            dt = inputDateTime.ToString("ddMMyyyyHHmmss");
            return dt;
        }

        //Convert Json Obj to string
        public static string ConvertJsonObjToString(Object jSonObj)
        {
            return JsonConvert.SerializeObject(jSonObj);
        }

        public static string ConvertStringToMD5(string inputStr)
        {
            MD5.MD5 md = new MD5.MD5();
            md.Value = inputStr;
            return md.FingerPrint;
        }
    }
}
