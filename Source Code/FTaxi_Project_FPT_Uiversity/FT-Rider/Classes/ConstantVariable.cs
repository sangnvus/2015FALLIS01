using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class ConstantVariable
    {

        //Google Maps API String
        public const string googleAPIQueryAutoCompleteRequestsBaseURI = "https://maps.googleapis.com/maps/api/place/queryautocomplete/json?key=";
        public const string googleAPIGeocodingAddressBaseURI = "https://maps.googleapis.com/maps/api/geocode/json?address=";
        //public static string googleGeolocationAPIkey = "AIzaSyAOi7TswVYRlkqvZcQ88Qf9SUHODK67TR0";
        public const string googleGeolocationAPIkey = "AIzaSyD4Y-OfQiAfs9hS_kyrfmzUSs5jY9gEKiY";
        public const string googleAPIGeocodingLatLngBaseURI = "https://maps.googleapis.com/maps/api/geocode/json?latlng=";
        public const string googleResponseStatusOK = "OK";


        //https://maps.googleapis.com/maps/api/distancematrix/json?
        //origins=21.032585,105.813623
        //&destinations=21.031849,105.826399
        //&mode=driving&language=vi-VI
        //&key=AIzaSyD4Y-OfQiAfs9hS_kyrfmzUSs5jY9gEKiY
        public const string googleAPIDistanceMatrixBaseURI1 = "https://maps.googleapis.com/maps/api/distancematrix/json?origins=";
        public const string googleAPIDistanceMatrixBaseURI2 = "&destinations=";
        public const string googleAPIDistanceMatrixBaseURI3 = "&mode=driving&language=vi-VI";



        //String
        public const string destiationAddressDescription = "Địa chỉ đón";
        public const string errInvalidAddress = "Địa chỉ không hợp lệ, vui lòng thử lại!";
        public const string errServiceIsOff = "Dịch vụ định vị đang tắt, vui lòng bật lên hoặc kiểm tra lại các thiết đặt.";
        public const string errHasErrInProcess = "Đã có lỗi xảy ra trong quá trình hoạt động, vui lòng trở lại!";
        public const string errNoCarYet = "Không có taxi nào gần đây, vui lòng thử lại sau!";
        public const string errLoginFailed = "Đăng nhập không thành công!";
        public const string errRegisterFailed = "Đăng ký không thành công!";
        public const string strLoginSucess = "Đăng nhập thành công.";
        public const string strLoginSuccessed = "Đăng ký thành công!";
        public const string errNotEmpty = "Vui lòng điền đầy đủ thông tin!";
        public const string errConnectingError = "Xảy ra lỗi khi kết nối đến máy chủ. Vui lòng thử lại sau!";
        public const string errServerErr = "Có lỗi xảy ra ở máy chủ. Vui lòng thử lại sau!";
        public const string strNoTripInfo = "Bạn chưa có chuyến đi nào cả!";
        public const string strNoFavoriteInfo = "Chưa có thông tin!";
        public const string strNoDriverNumber = "Tài xế không có số điện thoại!";
        public const string strRiderUpdateSuccess = "Cập nhật thông tin cá nhân thành công!";
        public const string strResetPasswordSuccess = "Mật khẩu đã được gửi về hôm thư của bạn. Vui lòng kiếm tra và đăng nhập lại!";
        public const string strOldPassNotCorrect = "Mật khẩu cũ không chính xác. Vui lòng kiếm tra lại!";
        public const string strPassNotValid = "Mật khẩu phải lớn hơn 6 ký tự!";
        public const string strPassNotLike = "Mật khẩu không giống nhau!";
        public const string strChangePassSuccess = "Đổi mật khẩu thành công!";
        public const string strAddFavoriteSuccess = "Bạn đã thêm thành công lái xe yêu thích!";


        //valid
        public const string validMobile = "Số điện phải có 10 hoặc 11 số!";
        public const string validName = "Vui lòng nhập tên!";
        public const string validEmail = "Vui lòng Kiểm tra lại email!";

        //Rider Controller
        public const string tNetWsParameterName = "json";
        public const string tNetRiderLoginAddress = "http://123.30.236.109:8088/TN/restServices/RiderController/LoginRider";
        public const string tNetRiderGetNerDriverAddress = "http://123.30.236.109:8088/TN/restServices/RiderController/GetNearDriver";
        public const string tNetRiderRegisterAddress = "http://123.30.236.109:8088/TN/restServices/TestController/RegisterRider";
        public const string tNetRiderGetCityName = "http://123.30.236.109:8088/TN/restServices/TestController/getCityName";
        public const string tNetRiderCreateTrip = "http://123.30.236.109:8088/TN/restServices/TripController/CreateTrip";
        public const string tNetRiderUpdateRegId = "http://123.30.236.109:8088/TN/restServices/CommonController/updateRegId";
        public const string tNetRiderCancelTrip = "http://123.30.236.109:8088/TN/restServices/CommonController/CancelTrip";
        public const string tNetRiderGetMyTrip = "http://123.30.236.109:8088/TN/restServices/TripController/getCompletedTripRider";
        public const string tNetRiderGetMyFarvoriteDriver = "http://123.30.236.109:8088/TN/restServices/FavoriteController/getListFavoriteDriver";
        public const string tNetRiderDeleteMyFarvoriteDriver = "http://123.30.236.109:8088/TN/restServices/FavoriteController/deleteFavoriteDriver";
        public const string tNetRiderUpdateProfile = "http://123.30.236.109:8088/TN/restServices/RiderController/UpdateRider";
        public const string tNetRiderUpdateAddress = "http://123.30.236.109:8088/TN/restServices/CommonController/UpdateAddress";
        public const string tNetRiderResetPassword = "http://123.30.236.109:8088/TN/restServices/CommonController/resetPassword";
        public const string tNetRiderChangePassword = "http://123.30.236.109:8088/TN/restServices/RiderController/ChangePassword";
        public const string tNetRiderAddMyFarvoriteDriver = "http://123.30.236.109:8088/TN/restServices/FavoriteController/addFavoriteDriver";

        //Card type
        public const String DIRECT = "DI";
        public const String BANK_ACCOUNT = "BA";
        public const String CREDIT_CARD = "CR";
        public const String PAYPAL = "PP";

        //Trip Type
        public const String ONE_MANY = "OM";
        public const String MANY = "MN";

        //Mobile Type
        public const string mTypeWIN = "WIN";

        //Noti Type
        public const string dRole = "RD";

        //Trip type
        public const string notiTypeNewTrip = "NT";
        public const string notiTypeUpdateTrip = "UT";
        public const string notiTypePromotionTeip = "PT";

        //Start trip status
        public const string tripStatusNewTrip = "NT";
        public const string tripStatusReject = "RJ";
        public const string tripStatusPicked = "PD";
        public const string tripStatusPicking = "PI";
        public const string tripStatusCancelled = "CA";
        public const string tripStatusComplete = "TC";


        //Trip message status
        public const string strPleseWait = "Vui lòng đợi...";
        public const string strCarAreComming = "Xe đang tới...";
        public const string strCarRejected = "Yêu cầu bị hủy bỏ!";
        public const string strCarCanceled = "Chuyến đi bị hủy bỏ!";
        public const string strCarAreStarting = "Chúc bạn một chuyến đi vui vẻ";

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



        //Other String
        public const string phoneCallDriverSubject = "Tài xế";

        //Confirmboz
        public const string cfbLogOut = "Bạn có chắc là bạn muốn thoát tài khoản không?";
        public const string cfbYes = "Có";
        public const string cfbCancel = "Hủy chuyến";
        public const string cfbNo = "Không";
        public const string cfbCancelTaxi = "Bạn sẽ phải trả tiền gọi xe theo giá mở cửa nếu hủy chuyến.";


        //Address Type
        public const string addrTypeHOME = "home";
        public const string addrTypeOFFICE = "office";

    }
}
