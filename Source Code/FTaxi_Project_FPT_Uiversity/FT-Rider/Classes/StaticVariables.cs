using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class StaticVariables
    {
        public static string googleAPIQueryAutoCompleteRequestsBaseURI = "https://maps.googleapis.com/maps/api/place/queryautocomplete/json?key=";
        public static string googleAPIGeocodingRequestsBaseURI = "https://maps.googleapis.com/maps/api/geocode/json?address=";
        //public static string googleGeolocationAPIkey = "AIzaSyAOi7TswVYRlkqvZcQ88Qf9SUHODK67TR0";
        public static string googleGeolocationAPIkey = "AIzaSyD4Y-OfQiAfs9hS_kyrfmzUSs5jY9gEKiY";
        public static string destiationAddressDescription = "Địa chỉ đón";
        public static string errInvalidAddress = "Địa chỉ không hợp lệ, vui lòng thử lại!";
        public static string errServiceIsOff = "Dịch vụ định vị đang tắt, vui lòng bật lên hoặc kiểm tra lại các thiết đặt.";
        public static string errHasErrInProcess = "Đã có lỗi xảy ra trong quá trình hoạt động, vui lòng trở lại!";

        //Rider Controller
        public static string tNetServerAddress = "123.30.236.109:8088/";
        public static string tNetRestServices = "TN/restServices/";
        public static string tNetRider = "RiderController/";
        public static string tNetRequestParameter = "?json";
        public static string tNetRiderController = tNetServerAddress + tNetRestServices + tNetRider + tNetRequestParameter;
    }
}
