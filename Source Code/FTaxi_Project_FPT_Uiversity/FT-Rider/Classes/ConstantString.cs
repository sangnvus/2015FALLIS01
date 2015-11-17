using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class ConstantString
    {
        public static string googleAPIQueryAutoCompleteRequestsBaseURI = "https://maps.googleapis.com/maps/api/place/queryautocomplete/json?key=";
        public static string googleAPIGeocodingRequestsBaseURI = "https://maps.googleapis.com/maps/api/geocode/json?address=";
        //public static string googleGeolocationAPIkey = "AIzaSyAOi7TswVYRlkqvZcQ88Qf9SUHODK67TR0";
        public static string googleGeolocationAPIkey = "AIzaSyD4Y-OfQiAfs9hS_kyrfmzUSs5jY9gEKiY";
        public static string destiationAddressDescription = "Địa chỉ đón";

        //Messenger
        public static string errInvalidAddress = "Địa chỉ không hợp lệ, vui lòng thử lại!";
        public static string errServiceIsOff = "Dịch vụ định vị đang tắt, vui lòng bật lên hoặc kiểm tra lại các thiết đặt.";
        public static string errHasErrInProcess = "Đã có lỗi xảy ra trong quá trình hoạt động, vui lòng trở lại!";
        public static string errNoCarYet = "Không có taxi nào gần đây, vui lòng thử lại sau!";


        public static string tNetWsParameterName = "json";

        //Rider Controller
        public static string tNetRiderLoginAddress = "http://123.30.236.109:8088/TN/restServices/RiderController/LoginRider";
        public static string tNetRiderGetNerDriverAddress = "http://123.30.236.109:8088/TN/restServices/RiderController/GetNearDriver";
        

        //Login Page
        public static string strLoginSucess = "Đăng nhập thành công.";
    }
}
