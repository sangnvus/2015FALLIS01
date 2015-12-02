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
        public const string googleAPIGeocodingLatLngBaseURI = "https://maps.googleapis.com/maps/api/geocode/json?latlng=";
        //public static string googleGeolocationAPIkey = "AIzaSyAOi7TswVYRlkqvZcQ88Qf9SUHODK67TR0";
        public const string googleGeolocationAPIkey = "AIzaSyD4Y-OfQiAfs9hS_kyrfmzUSs5jY9gEKiY";
        public const string destiationAddressDescription = "Địa chỉ đón";

        public const string googleAPIDistanceMatrixBaseURI1 = "https://maps.googleapis.com/maps/api/distancematrix/json?origins=";
        public const string googleAPIDistanceMatrixBaseURI2 = "&destinations=";
        public const string googleAPIDistanceMatrixBaseURI3 = "&mode=driving&language=vi-VI";

        //Messenger
        public const string errInvalidAddress = "Địa chỉ không hợp lệ. Vui lòng thử lại sau!";
        public const string errServiceIsOff = "Dịch vụ định vị đang tắt. Vui lòng bật lên hoặc kiểm tra lại các thiết đặt!";
        public const string errHasErrInProcess = "Đã có lỗi xảy ra trong quá trình hoạt động. Vui lòng trở lại!";
        public const string errNoCarYet = "Không có taxi nào gần đây. Vui lòng thử lại sau!";
        public const string errLoginFailed = "Đăng nhập không thành công. Vui lòng kiểm tra lại thông tin!";
        public const string errConnectingError = "Xảy ra lỗi khi kết nối đến máy chủ. Vui lòng thử lại sau!";
        public const string errNotEmpty = "Vui lòng điền đầy đủ thông tin!";
        public const string errServerError = "Đang có lỗi máy chủ, vui lòng thử lại sau!";


        public const string tNetWsParameterName = "json";

        //Driver Controller
        public const string tNetDriverLoginAddress = "http://123.30.236.109:8088/TN/restServices/DriverController/Login";
        public const string tNetDriverUpdateStatus = "http://123.30.236.109:8088/TN/restServices/DriverController/UpdateDriverStatus";
        public const string tNetDriverUpdateCurrentLocation = "http://123.30.236.109:8088/TN/restServices/DriverController/UpdateCurrentLocation";
        public const string tNetDriverSelectVehicle = "http://123.30.236.109:8088/TN/restServices/DriverController/selectVehicle";
        public const string tNetDriverUpdateRegId = "http://123.30.236.109:8088/TN/restServices/CommonController/updateRegId";
        public const string tNetDriverAcceptTrip = "http://123.30.236.109:8088/TN/restServices/TripController/AcceptTrip";
        public const string tNetDriverRejectTrip = "http://123.30.236.109:8088/TN/restServices/TripController/RejectTrip";
        public const string tNetDriverStartTrip = "http://123.30.236.109:8088/TN/restServices/TripController/StartTrip";
        public const string tNetDriverCancelTrip = "http://123.30.236.109:8088/TN/restServices/TripController/CancelTrip";
        public const string tNetDriverCompleteTrip = "http://123.30.236.109:8088/TN/restServices/TripController/CompleteTrip";

        

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

        //Driver Type
        public const string dRole = "DR";

        //Trip type
        public const string notiTypeNewTrip = "NT";
        public const string notiTypeUpdateTrip = "UT";
        public const string notiTypePromotionTeip = "PT";

        //Response code
        public const string responseCodeSuccess = "0000";
        public const string responseCodeTaken = "013";

        //Start trip status
        public const string tripStatusNewTrip = "NT";
        public const string tripStatusReject = "RJ";
        public const string tripStatusPicked = "PD";
        public const string tripStatusPicking = "PI";
        public const string tripStatusCancelled = "CA";
        public const string tripStatusComplete = "TC";
    }
}
