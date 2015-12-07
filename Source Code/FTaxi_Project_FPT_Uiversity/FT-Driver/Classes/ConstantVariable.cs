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
        public const string RESPONSECODE_SUCCESS = "0000";
        public const string RESPONSECODE_FAIL = "1111";
        public const string RESPONSECODE_ACCOUNT_NOT_ACTIVE = "101";
        public const string RESPONSECODE_USERNAME_NOT_CORRECT = "102";
        public const string RESPONSECODE_PASSWORD_NOT_CORRECT = "103";

        public const string RESPONSECODE_LMD_NOT_EQUAL = "201";
        public const string RESPONSECODE_OLD_PASSWORD_WRONG = "301";
        public const string RESPONSECODE_ERR_SYSTEM = "001";
        public const string RESPONSECODE_DUPLICATED = "002";
        public const string RESPONSECODE_INVALID_NUMBER = "003";
        public const string RESPONSECODE_INVALID_CARD_NO = "004";
        public const string RESPONSECODE_DUPPLICATED_PHONE_NUMBER = "005";
        public const string RESPONSECODE_USERNAME_NOT_FOUND = "006";
        public const string RESPONSECODE_INVALID_USER_GROUP = "007";
        public const string RESPONSECODE_INVALID_PASSWORD = "008";

        public const string RESPONSECODE_INVALID_DATA = "-1";
        public const string RESPONSECODE_PROMOTION_CODE_NOT_FOUND = "009";
        public const string RESPONSECODE_PROGRAM_OUT_OF_DATE = "010";
        public const string RESPONSECODE_INVALID_PROMOTION_CODE = "011";
        public const string RESPONSECODE_CANT_SEND_APNS = "012";
        public const string RESPONSECODE_TRIP_TAKEN = "013";
        public const string RESPONSECODE_CANT_SEND_GCM = "014";
        public const string RESPONSECODE_NOT_ENOUGH_SEATS = "015";
        public const string RESPONSECODE_UNREGISTERED_FACEBOOK_ACCOUNT = "016";
        public const string RESPONSECODE_CAR_ALREADY_SELECTED = "017";
        public const string RESPONSECODE_INVALID_VERIFY_CODE = "018";
        public const string RESPONSECODE_INVALID_EMAIL = "019";
        public const string RESPONSECODE_CAR_BUSY = "MS036";

        public const string RESPONSECODE_TRIP_CANCELLED = "901";
        public const string RESPONSECODE_TRIP_COMPLETED = "902";
        public const string RESPONSECODE_TRIP_NO_RESPONSE = "903";

        //Start trip status
        public const string tripStatusNewTrip = "NT";
        public const string tripStatusReject = "RJ";
        public const string tripStatusPicked = "PD";
        public const string tripStatusPicking = "PI";
        public const string tripStatusCancelled = "CA";
        public const string tripStatusComplete = "TC";
    }
}
