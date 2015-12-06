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
        public const string tNetRiderUpdateAddress = "http://123.30.236.109:8088/TN/restServices/RiderController/UpdateAddress";
        public const string tNetRiderResetPassword = "http://123.30.236.109:8088/TN/restServices/RiderController/ResetPassword";
        public const string tNetRiderChangePassword = "http://123.30.236.109:8088/TN/restServices/RiderController/ChangePassword";

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
        public const string responseCodeSuccess = "0000";
        public const string responseCodeTaken = "013";

        //Other String
        public const string phoneCallDriverSubject = "Tài xế";

        //Confirmboz
        public const string cfbLogOut = "Bạn có chắc là bạn muốn thoát tài khoản không?";
        public const string cfbYes = "Có";
        public const string cfbNo = "Không";


        //Address Type
        public const string addrTypeHOME = "home";
        public const string addrTypeOFFICE = "office";

    }
}
