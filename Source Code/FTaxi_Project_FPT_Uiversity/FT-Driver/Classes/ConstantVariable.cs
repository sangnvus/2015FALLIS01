using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Driver.Classes
{
    class ConstantVariable
    {
        public const string googleAPIQueryAutoCompleteRequestsBaseURI = "https://maps.googleapis.com/maps/api/place/queryautocomplete/json?key=";
        public const string googleAPIGeocodingRequestsBaseURI = "https://maps.googleapis.com/maps/api/geocode/json?address=";
        //public static string googleGeolocationAPIkey = "AIzaSyAOi7TswVYRlkqvZcQ88Qf9SUHODK67TR0";
        public const string googleGeolocationAPIkey = "AIzaSyD4Y-OfQiAfs9hS_kyrfmzUSs5jY9gEKiY";
        public const string destiationAddressDescription = "Địa chỉ đón";

        //Messenger
        public const string errInvalidAddress = "Địa chỉ không hợp lệ. Vui lòng thử lại sau!";
        public const string errServiceIsOff = "Dịch vụ định vị đang tắt. Vui lòng bật lên hoặc kiểm tra lại các thiết đặt!";
        public const string errHasErrInProcess = "Đã có lỗi xảy ra trong quá trình hoạt động. Vui lòng trở lại!";
        public const string errNoCarYet = "Không có taxi nào gần đây. Vui lòng thử lại sau!";
        public const string errLoginFailed = "Đăng nhập không thành công. Vui lòng kiểm tra lại thông tin!";
        public const string errConnectingError = "Xảy ra lỗi khi kết nối đến máy chủ. Vui lòng thử lại sau!";


        public const string tNetWsParameterName = "json";

        //Driver Controller
        public const string tNetDriverLoginAddress = "http://123.30.236.109:8088/TN/restServices/DriverController/Login";
        public const string tNetDriverUpdateStatus = "http://123.30.236.109:8088/TN/restServices/DriverController/UpdateDriverStatus";
        public const string tNetDriverUpdateCurrentLocation = "http://123.30.236.109:8088/TN/restServices/DriverController/UpdateCurrentLocation";
        

        //Login Page
        public const string strLoginSucess = "Đăng nhập thành công!";

        //Card type
        public const String DIRECT = "DI";
		public const String BANK_ACCOUNT = "BA";
		public const String CREDIT_CARD = "CR";
		public const String PAYPAL = "PP";

        //Mobile Type
        public const string mTypeWIN = "WIN";

        //Driver status
        public const string dStatusAvailable = "AC";
        public const string dStatusNotAvailable = "NA";
        public const string dStatusBusy = "BU";
    }
}
