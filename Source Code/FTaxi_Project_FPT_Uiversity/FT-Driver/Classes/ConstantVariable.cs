using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class ConstantVariable
    {
        public static const string googleAPIQueryAutoCompleteRequestsBaseURI = "https://maps.googleapis.com/maps/api/place/queryautocomplete/json?key=";
        public static const string googleAPIGeocodingRequestsBaseURI = "https://maps.googleapis.com/maps/api/geocode/json?address=";
        //public static string googleGeolocationAPIkey = "AIzaSyAOi7TswVYRlkqvZcQ88Qf9SUHODK67TR0";
        public static const string googleGeolocationAPIkey = "AIzaSyD4Y-OfQiAfs9hS_kyrfmzUSs5jY9gEKiY";
        public static const string destiationAddressDescription = "Địa chỉ đón";

        //Messenger
        public static const string errInvalidAddress = "Địa chỉ không hợp lệ, vui lòng thử lại!";
        public static const string errServiceIsOff = "Dịch vụ định vị đang tắt, vui lòng bật lên hoặc kiểm tra lại các thiết đặt.";
        public static const string errHasErrInProcess = "Đã có lỗi xảy ra trong quá trình hoạt động, vui lòng trở lại!";
        public static const string errNoCarYet = "Không có taxi nào gần đây, vui lòng thử lại sau!";


        public static const string tNetWsParameterName = "json";

        //Rider Controller
        public static const string tNetDriverLoginAddress = "http://123.30.236.109:8088/TN/restServices/DriverController/Login";
        //public static const string tNetRiderGetNerDriverAddress = "http://123.30.236.109:8088/TN/restServices/RiderController/GetNearDriver";
        

        //Login Page
        public static const string strLoginSucess = "Đăng nhập thành công.";

        //Card type
        public static const String DIRECT = "DI";
		public static const String BANK_ACCOUNT = "BA";
		public static const String CREDIT_CARD = "CR";
		public static const String PAYPAL = "PP";
    }
}
