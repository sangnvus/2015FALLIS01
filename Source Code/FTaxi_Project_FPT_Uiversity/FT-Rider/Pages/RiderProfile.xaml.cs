using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using FT_Rider.Classes;
using Newtonsoft.Json;
using System.Diagnostics;
using Telerik.Windows.Controls.PhoneTextBox;
using System.ComponentModel;
using System.Windows.Media.Animation;
using Microsoft.Devices;
using System.Threading.Tasks;
using System.Device.Location;
using Microsoft.Phone.Maps.Controls;
using System.Collections.ObjectModel;



namespace FT_Rider.Pages
{
    public partial class RiderProfile : PhoneApplicationPage
    {
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        RiderLogin userData = null;
        string pwmd5 = string.Empty;
        long preOlmd;
        IDictionary<string, RiderGetCityList> cityNamesDB = null;

        //For home Address Update
        double homeLat = 0;
        double homeLng = 0;
        int homeCityId = 0;
        string homeCntry = String.Empty;
        string homeCity = string.Empty;
        bool isMovableHomeMap = false;

        //For Offfice Address Update
        double officeLat = 0;
        double officeLng = 0;
        int officeCityId = 0;
        string officeCntry = String.Empty;
        string officeCity = string.Empty;
        bool isMovableOfficeMap = false;

        VibrateController vibrateController = VibrateController.Default;


        public RiderProfile()
        {
            InitializeComponent();


            //Load rider profile
            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userData = new RiderLogin();
                cityNamesDB = new Dictionary<string, RiderGetCityList>();
                userData = (RiderLogin)tNetUserLoginData["UserLoginData"];
                pwmd5 = (string)tNetUserLoginData["PasswordMd5"];
                preOlmd = (long)tNetUserLoginData["UserLmd"];
                cityNamesDB = (IDictionary<string, RiderGetCityList>)tNetUserLoginData["CityNamesDB"];

            }

            LoadRiderProfile();


            txt_HomeAddress.Text = tbl_HomeAddress.Text;
            txt_OfficeAddress.Text = tbl_OfficeAddress.Text;

        }


        private void LoadRiderProfile()
        {
            if (userData.content.fName != null)
            {
                txt_FirstName.Text = userData.content.fName;
            }
            else
            {
                txt_FirstName.Text = string.Empty;
            }

            if (userData.content.lName != null)
            {
                txt_LastName.Text = userData.content.lName;
            }
            else
            {
                txt_LastName.Text = string.Empty;
            }

            if (userData.content.mobile != null)
            {
                txt_Mobile.Text = userData.content.mobile;
            }
            else
            {
                txt_Mobile.Text = string.Empty;
            }

            if (userData.content.hAdd != null)
            {
                tbl_HomeAddress.Text = userData.content.hAdd;
            }
            else
            {
                tbl_HomeAddress.Text = string.Empty;
            }


            if (userData.content.oAdd != null)
            {
                tbl_OfficeAddress.Text = userData.content.oAdd;
            }
            else
            {
                tbl_OfficeAddress.Text = string.Empty;
            }
            if (userData.content.email != null)
            {
                txt_Email.Text = userData.content.email;
            }
            else
            {
                txt_Email.Text = string.Empty;
            }
        }

        private void tbl_Tap_ChangePassword(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void img_EditIcon_HomeAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowAddHomeAddressGrid();
        }

        private void img_EditIcon_OfficeAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowAddOfficeAddressGrid();
        }

        private void txt_LastName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowHomeSaveButton();
        }

        private void txt_FirstName_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowHomeSaveButton();

        }

        private void txt_Email_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowHomeSaveButton();

        }

        private void txt_Mobile_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowHomeSaveButton();

        }




        private void txt_LastName_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox addressTextbox = (TextBox)sender;
            addressTextbox.Background = new SolidColorBrush(Colors.Transparent);
            addressTextbox.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txt_FirstName_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox addressTextbox = (TextBox)sender;
            addressTextbox.Background = new SolidColorBrush(Colors.Transparent);
            addressTextbox.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txt_Email_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox addressTextbox = (TextBox)sender;
            addressTextbox.Background = new SolidColorBrush(Colors.Transparent);
            addressTextbox.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private void txt_Mobile_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox addressTextbox = (TextBox)sender;
            addressTextbox.Background = new SolidColorBrush(Colors.Transparent);
            addressTextbox.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private async void btn_Save_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Show loading screen
            ShowLoadingScreen();

            var rid = userData.content.rid;
            var email = txt_Email.Text;
            var fName = txt_FirstName.Text;
            var lName = txt_LastName.Text;
            var mobile = txt_Mobile.Text;
            var uid = userData.content.uid;
            var olmd = preOlmd; ; //Cái này là dùng lmd của Login
            var pw = pwmd5;
            var input = string.Format("{{\"rid\":\"{0}\",\"email\":\"{1}\",\"fName\":\"{2}\",\"lName\":\"{3}\",\"mobile\":\"{4}\",\"uid\":\"{5}\",\"pw\":\"{6}\",\"lmd\":\"{7}\"}}", rid, email, fName, lName, mobile, uid, pw, olmd);
            try
            {
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderUpdateProfile, input);
                var updateStatus = JsonConvert.DeserializeObject<BaseResponse>(output);
                if (updateStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS)) //Neu tra ve 0000
                {
                    ///1. Cập nhật lmd
                    ///2. show messenger box
                    ///tắt loading

                    //1
                    tNetUserLoginData["UserLmd"] = updateStatus.lmd;

                    //3
                    HideLoadingScreen();
                    ShowHomeEditButton();

                    //2
                    MessageBox.Show(ConstantVariable.strRiderUpdateSuccess); //Cập nhật thành công
                }
                else
                {
                    HideLoadingScreen();
                    ShowHomeEditButton();
                    MessageBox.Show("(Mã lỗi 22206) " + ConstantVariable.errServerErr); //Co loi may chu
                    Debug.WriteLine("Có lỗi 263gghf ở update profile");
                }
            }
            catch (Exception)
            {
                HideLoadingScreen();
                ShowHomeEditButton();
                MessageBox.Show("(Mã lỗi 2201) " + ConstantVariable.errServerErr); //Co loi may chu
                Debug.WriteLine("Có lỗi 7hsgt54 ở update profile");
            }
        }



        private bool ValidateEmail()
        {
            if (Regex.IsMatch(txt_Email.Text.Trim(), @"^([a-zA-Z_])([a-zA-Z0-9_\-\.]*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\])$"))
            {
                MessageBox.Show(ConstantVariable.validEmail);
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool ValidateName()
        {
            var NameEmpty = string.IsNullOrEmpty(txt_FirstName.Text);
            if (NameEmpty)
            {
                MessageBox.Show(ConstantVariable.validName);
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool ValidateLastName()
        {
            var FirstAndMiddleNameEmpty = string.IsNullOrEmpty(txt_LastName.Text);
            if (FirstAndMiddleNameEmpty)
            {
                MessageBox.Show(ConstantVariable.validName);
                return false;
            }
            else
            {
                return true;
            }
        }


        private bool ValidatePhoneNumber()
        {
            var PhoneNumberEmpty = string.IsNullOrEmpty(txt_Mobile.Text);
            if (PhoneNumberEmpty || txt_Mobile.Text.Length != 10 || txt_Mobile.Text.Length != 11)
            {
                MessageBox.Show(ConstantVariable.validMobile);
                txt_Mobile.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void txt_LastName_LostFocus(object sender, RoutedEventArgs e)
        {
            //ValidateLastName();
        }

        private void txt_FirstName_LostFocus(object sender, RoutedEventArgs e)
        {
            //ValidateName();
        }

        private void txt_Email_LostFocus(object sender, RoutedEventArgs e)
        {
            //ValidateEmail();
        }

        private void txt_Mobile_LostFocus(object sender, RoutedEventArgs e)
        {
            //ValidatePhoneNumber();
        }


        private void btn_Edit_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowHomeSaveButton();
            txt_LastName.Focus();
        }


        private void txt_LastName_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowHomeSaveButton();
        }

        private void txt_FirstName_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowHomeSaveButton();
        }

        private void txt_Email_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowHomeSaveButton();
        }

        private void txt_Mobile_Tap_1(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowHomeSaveButton();

        }


        private async void UpdateRiderOfficeAddress(string inputAddress, string type)
        {

            ShowLoadingScreen();
            //Lấy thông tin tọa độ
            var input = ConstantVariable.googleAPIGeocodingAddressBaseURI + inputAddress + "&key=" + ConstantVariable.googleGeolocationAPIkey;
            try
            {
                var output = await ReqAndRes.GetJsonString(input);
                var myLocation = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(output);
                if (myLocation.status.Equals(ConstantVariable.googleResponseStatusOK)) //OK
                {
                    officeCntry = myLocation.results[0].address_components[myLocation.results[0].address_components.Count - 1].short_name.ToString();

                    //Trả về cityCode
                    officeCity = myLocation.results[0].address_components[myLocation.results[0].address_components.Count - 2].long_name.ToString();

                    //Tra ve lat long
                    officeLat = myLocation.results[0].geometry.location.lat;
                    officeLng = myLocation.results[0].geometry.location.lng;


                    //Update
                    var id = userData.content.rid;
                    var nCity = officeCity;
                    var nAdd = txt_OfficeAddress.Text;
                    double lat = officeLat;
                    double lng = officeLng;
                    var addrType = type;
                    long lmd = preOlmd;
                    var role = ConstantVariable.dRole;
                    var cityId = officeCityId;
                    var cntry = officeCntry;
                    var uid = userData.content.uid;
                    var pw = pwmd5;

                    var input2 = string.Format("{{\"id\":\"{0}\",\"nCity\":\"{1}\",\"nAdd\":\"{2}\",\"lat\":\"{3}\",\"lng\":\"{4}\",\"addrType\":\"{5}\",\"olmd\":\"{6}\",\"role\":\"{7}\",\"cityId\":\"{8}\",\"cntry\":\"{9}\",\"uid\":\"{10}\",\"pw\":\"{11}\"}}", id, nCity, nAdd, lat, lng, addrType, lmd, role, cityId, cntry, uid, pw);
                    try
                    {
                        //Thủ xem có lấy đc gì k
                        var output2 = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderUpdateAddress, input2);
                        var updateStatus = JsonConvert.DeserializeObject<BaseResponse>(output2);
                        if (updateStatus.status != null)
                        {
                            if (updateStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS))//ok 0000
                            {
                                tNetUserLoginData["UserLmd"] = updateStatus.lmd;

                                if (officeLng != 0 && officeLat != 0)
                                {
                                    map_OfficeAddress.SetView(new GeoCoordinate(officeLat, officeLng), 16, MapAnimationKind.Linear);
                                }

                                HideSaveOfficeButton();
                                HideLoadingScreen();
                                MessageBox.Show(ConstantVariable.strRiderUpdateSuccess); //ok
                            }

                        }
                        else
                        {
                            HideLoadingScreen();
                            MessageBox.Show("(Mã lỗi 2650) " + ConstantVariable.errServerErr); //Co loi may chu
                            Debug.WriteLine("Có lỗi gg5d2tr ở update ofice address");
                        }
                    }
                    catch (Exception)
                    {
                        HideLoadingScreen();
                        MessageBox.Show("(Mã lỗi 2651) " + ConstantVariable.errServerErr); //Co loi may chu
                        Debug.WriteLine("Có lỗi sf5ds ở update ofice address");
                    }
                }
                else
                {
                    MessageBox.Show("(Mã lỗi 2652) " + ConstantVariable.errServerErr);
                    Debug.WriteLine("Có lỗi fefsafd ở get Google String");
                }
            }
            catch (Exception)
            {

                MessageBox.Show("(Mã lỗi 2653) " + ConstantVariable.errServerErr);
                Debug.WriteLine("Có lỗi jyj76 ở get Google String");
            }

        }


        private async void UpdateRiderHomeAddress(string inputAddress, string type)
        {

            ShowLoadingScreen();
            //Lấy thông tin tọa độ
            var input = ConstantVariable.googleAPIGeocodingAddressBaseURI + inputAddress + "&key=" + ConstantVariable.googleGeolocationAPIkey;
            try
            {
                var output = await ReqAndRes.GetJsonString(input);
                var myLocation = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(output);
                if (myLocation.status.Equals(ConstantVariable.googleResponseStatusOK)) //OK
                {
                    homeCntry = myLocation.results[0].address_components[myLocation.results[0].address_components.Count - 1].short_name.ToString();

                    //Trả về cityCode
                    homeCity = myLocation.results[0].address_components[myLocation.results[0].address_components.Count - 2].long_name.ToString();

                    //Tra ve lat long
                    homeLat = myLocation.results[0].geometry.location.lat;
                    homeLng = myLocation.results[0].geometry.location.lng;


                    //Update
                    var id = userData.content.rid;
                    var nCity = homeCity;
                    var nAdd = txt_HomeAddress.Text;
                    double lat = homeLat;
                    double lng = homeLng;
                    var addrType = type;
                    long lmd = preOlmd;
                    var role = ConstantVariable.dRole;
                    var cityId = homeCityId;
                    var cntry = homeCntry;
                    var uid = userData.content.uid;
                    var pw = pwmd5;

                    var input2 = string.Format("{{\"id\":\"{0}\",\"nCity\":\"{1}\",\"nAdd\":\"{2}\",\"lat\":\"{3}\",\"lng\":\"{4}\",\"addrType\":\"{5}\",\"olmd\":\"{6}\",\"role\":\"{7}\",\"cityId\":\"{8}\",\"cntry\":\"{9}\",\"uid\":\"{10}\",\"pw\":\"{11}\"}}", id, nCity, nAdd, lat, lng, addrType, lmd, role, cityId, cntry, uid, pw);
                    try
                    {
                        //Thủ xem có lấy đc gì k
                        var output2 = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderUpdateAddress, input2);
                        var updateStatus = JsonConvert.DeserializeObject<BaseResponse>(output2);
                        if (updateStatus.status != null)
                        {
                            if (updateStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS))//ok 0000
                            {
                                tNetUserLoginData["UserLmd"] = updateStatus.lmd;

                                if (homeLng != 0 && homeLat != 0)
                                {
                                    map_HomeAddress.SetView(new GeoCoordinate(homeLat, homeLng), 16, MapAnimationKind.Linear);
                                }

                                HideSaveHomeButton();
                                HideLoadingScreen();
                                MessageBox.Show(ConstantVariable.strRiderUpdateSuccess); //ok
                            }

                        }
                        else
                        {
                            HideLoadingScreen();
                            MessageBox.Show("(Mã lỗi 2689) " + ConstantVariable.errServerErr); //Co loi may chu
                            Debug.WriteLine("Có lỗi 2565568 ở update home address");
                        }
                    }
                    catch (Exception)
                    {
                        HideLoadingScreen();
                        MessageBox.Show("(Mã lỗi 2605) " + ConstantVariable.errServerErr); //Co loi may chu
                        Debug.WriteLine("Có lỗi 58565dfg ở update home address");
                    }
                }
                else
                {
                    MessageBox.Show("(Mã lỗi 2603) " + ConstantVariable.errServerErr);
                    Debug.WriteLine("Có lỗi 56ght ở get Google String");
                }
            }
            catch (Exception)
            {

                MessageBox.Show("(Mã lỗi 2602) " + ConstantVariable.errServerErr);
                Debug.WriteLine("Có lỗi 256ftgh ở get Google String");
            }

        }




        private void txt_HomeAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txt_HomeAddress.Text != null)
            {
                ShowCloseClearHomeIcon();
            }
            setCursorAtLast(txt_HomeAddress);
        }

        private void img_CloseClearHomeAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (txt_HomeAddress.Text != null)
            {
                txt_HomeAddress.Text = string.Empty;
            }
            if (txt_HomeAddress.Text.Equals(null))
            {
                map_HomeAddress.Focus();
            }
        }

        private void ShowCloseClearHomeIcon()
        {
            img_CloseClearHomeAddress.Visibility = Visibility.Visible;
        }

        private void HideCloseClearHomeIcon()
        {
            img_CloseClearHomeAddress.Visibility = Visibility.Collapsed;
        }

        private void txt_HomeAddress_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (txt_HomeAddress.Text != null)
            {
                ShowCloseClearHomeIcon();
                ShowSaveHomeButton();
            }
            HomeAddressAutoComplete(txt_HomeAddress.Text);
            //GetHomeCityPlusInfo(txt_HomeAddress.Text);

        }

        private async void GetHomeCityPlusInfo(string inputAddress)
        {

            //Lấy thông tin tọa độ
            var input = ConstantVariable.googleAPIGeocodingAddressBaseURI + inputAddress + "&key=" + ConstantVariable.googleGeolocationAPIkey;
            try
            {
                var output = await ReqAndRes.GetJsonString(input);
                var myLocation = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(output);
                if (myLocation.status.Equals(ConstantVariable.googleResponseStatusOK)) //OK
                {
                    homeCntry = myLocation.results[0].address_components[myLocation.results[0].address_components.Count - 1].short_name.ToString();

                    //Trả về cityCode
                    homeCity = myLocation.results[0].address_components[myLocation.results[0].address_components.Count - 2].long_name.ToString();

                    //Tra ve lat long
                    homeLat = myLocation.results[0].geometry.location.lat;
                    homeLng = myLocation.results[0].geometry.location.lng;
                }
                else
                {
                    MessageBox.Show("(Mã lỗi 2603) " + ConstantVariable.errServerErr);
                    Debug.WriteLine("Có lỗi 56ght ở get Google String");
                }
            }
            catch (Exception)
            {

                MessageBox.Show("(Mã lỗi 2602) " + ConstantVariable.errServerErr);
                Debug.WriteLine("Có lỗi 256ftgh ở get Google String");
            }
        }

        //------ BEGIN set Pickup Address from address search ------//
        /// <summary>
        /// CÁI NÀY ĐỂ SAU KHI CHỌN ĐIỂM ĐÓN THÌ MAP SẼ CHẠY LẠI ĐỊA CHỈ ĐÓ
        /// </summary>
        /// <param name="inputAddress"></param>
        private void SetPositionFromAddress(string inputAddress)
        {
            //GoogleAPIGeocoding URL
            string URL = ConstantVariable.googleAPIGeocodingAddressBaseURI + inputAddress + "&key=" + ConstantVariable.googleGeolocationAPIkey;

            //Query Autocomplete Responses to a JSON String
            WebClient proxy = new WebClient();
            proxy.DownloadStringCompleted +=
            new DownloadStringCompletedEventHandler(proxy_SetPositionFromAddress);
            proxy.DownloadStringAsync(new Uri(URL));
        }
        private void proxy_SetPositionFromAddress(object sender, DownloadStringCompletedEventArgs e)
        {
            //1. Convert chuối json lấy về thành object
            GoogleAPIAddressObj places = new GoogleAPIAddressObj();
            places = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(e.Result);
            try
            {
                //Lấy tọa độ của điểm mới tìm được
                double lat = places.results[0].geometry.location.lat;
                double lng = places.results[0].geometry.location.lng;

                //Và dời vị trí map về đó
                map_HomeAddress.SetView(new GeoCoordinate(lat, lng), 16, MapAnimationKind.Linear);

            }
            catch (Exception)
            {

                MessageBox.Show(ConstantVariable.errInvalidAddress);
            }
        }
        //------ END set Pickup Address from address search  ------//

        private int GetCityCodeFromCityName(string cityName)
        {
            int cityCode = 0;
            try
            {
                cityCode = cityNamesDB[cityName].cityId;
            }
            catch (Exception)
            {

                MessageBox.Show("(Mã lỗi 2601) " + ConstantVariable.errHasErrInProcess);
            }
            return cityCode;
        }

        private void txt_HomeAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //ShowSaveHomeButton();
        }


        private void btn_SaveHome_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UpdateRiderHomeAddress(txt_HomeAddress.Text, ConstantVariable.addrTypeHOME);
        }



        private async void HomeAddressAutoComplete(string inputAddress)
        {
            GoogleAPIQueryAutoCompleteObj placesObj = new GoogleAPIQueryAutoCompleteObj();

            try
            {
                placesObj = await GoogleAPIFunction.ConvertAutoCompleteToLLS(inputAddress);

                //2. Create Place list
                ObservableCollection<AutoCompletePlaceLLSObj> autoCompleteDataSource = new ObservableCollection<AutoCompletePlaceLLSObj>();
                lls_HomeAddress.ItemsSource = autoCompleteDataSource;
                //3. Loop to list all item in object
                if (placesObj.status != "ZERO_RESULTS")
                {
                    ShowLLSHomeAddress();

                    foreach (var obj in placesObj.predictions)
                    {
                        autoCompleteDataSource.Add(new AutoCompletePlaceLLSObj(obj.description.ToString()));
                    }
                }
                else
                {
                    HideLLSHomeAddress();
                }


            }
            catch (Exception)
            {

                txt_HomeAddress.Focus();
            }
        }

        private async void lls_HomeAddress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedPlace = ((AutoCompletePlaceLLSObj)(sender as LongListSelector).SelectedItem);
            // If selected item is null, do nothing
            if (lls_HomeAddress.SelectedItem == null)
            {
                return;
            }

            //show address on textbox
            GoogleAPIAddressObj address = new GoogleAPIAddressObj();
            address = await GoogleAPIFunction.ConvertAddressToLatLng(selectedPlace.Name.ToString());
            txt_HomeAddress.Text = address.results[0].address_components[0].long_name.ToString()
                                + ", " + address.results[0].address_components[1].long_name.ToString()
                                + ", " + address.results[0].address_components[2].long_name.ToString();
            //txt_City.Text = address.results[0].address_components[address.results[0].address_components.Count - 2].long_name.ToString();

            //Return Lat, Lng, some paramenter here
            //Return Lat, Lng, some paramenter here
            //Return Lat, Lng, some paramenter here

            setCursorAtFirst(txt_HomeAddress);

            SetPositionFromAddress(address.results[0].formatted_address.ToString());

            //vibrate phone
            vibrateController.Start(TimeSpan.FromSeconds(0.1));

            HideLLSHomeAddress();
        }

        private void txt_HomeAddress_LostFocus(object sender, RoutedEventArgs e)
        {
            setCursorAtFirst(txt_HomeAddress);
            HideCloseClearHomeIcon();
        }

        private void map_HomeAddress_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void map_HomeAddress_ResolveCompleted(object sender, MapResolveCompletedEventArgs e)
        {
            if (isMovableHomeMap == true)
            {
                homeLat = map_HomeAddress.Center.Latitude;
                homeLng = map_HomeAddress.Center.Longitude;
                ShowHomeAddressFromCoordinate(homeLat, homeLng);
            }
        }

        private async void ShowHomeAddressFromCoordinate(double lat, double lng)
        {
            try
            {
                var str = await GoogleAPIFunction.ConvertLatLngToAddress(lat, lng);
                var address = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(str);
                txt_HomeAddress.Text = address.results[0].formatted_address.ToString();
            }
            catch (Exception)
            {

                MessageBox.Show("(Mã lỗi 32658) " + ConstantVariable.errConnectingError);
            }
        }

        private async void ShowAddressFromCoordinate(double lat, double lng)
        {
            try
            {
                var str = await GoogleAPIFunction.ConvertLatLngToAddress(lat, lng);
                var address = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(str);
                txt_OfficeAddress.Text = address.results[0].formatted_address.ToString();
            }
            catch (Exception)
            {

                MessageBox.Show("(Mã lỗi 32658) " + ConstantVariable.errConnectingError);
            }
        }
        private void map_HomeAddress_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isMovableHomeMap = true;
        }

        private void map_HomeAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            setCursorAtFirst(txt_HomeAddress);
        }

        private void img_CloseHome_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HideAddHomeAddressGrid();
        }




        #region Update Office Function
        //------ BEGIN set Pickup Address from address search ------//
        /// <summary>
        /// CÁI NÀY ĐỂ SAU KHI CHỌN ĐIỂM ĐÓN THÌ MAP SẼ CHẠY LẠI ĐỊA CHỈ ĐÓ
        /// </summary>
        /// <param name="inputAddress"></param>
        private void SetOfficePositionFromAddress(string inputAddress)
        {
            //GoogleAPIGeocoding URL
            string URL = ConstantVariable.googleAPIGeocodingAddressBaseURI + inputAddress + "&key=" + ConstantVariable.googleGeolocationAPIkey;

            //Query Autocomplete Responses to a JSON String
            WebClient proxy = new WebClient();
            proxy.DownloadStringCompleted +=
            new DownloadStringCompletedEventHandler(proxy_SetOfficePositionFromAddress);
            proxy.DownloadStringAsync(new Uri(URL));
        }
        private void proxy_SetOfficePositionFromAddress(object sender, DownloadStringCompletedEventArgs e)
        {
            //1. Convert chuối json lấy về thành object
            GoogleAPIAddressObj places = new GoogleAPIAddressObj();
            places = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(e.Result);
            try
            {
                //Lấy tọa độ của điểm mới tìm được
                double lat = places.results[0].geometry.location.lat;
                double lng = places.results[0].geometry.location.lng;

                //Và dời vị trí map về đó
                map_OfficeAddress.SetView(new GeoCoordinate(lat, lng), 16, MapAnimationKind.Linear);

            }
            catch (Exception)
            {

                MessageBox.Show(ConstantVariable.errInvalidAddress);
            }
        }
        //------ END set Pickup Address from address search  ------//

        private async void OfficeAddressAutoComplete(string inputAddress)
        {
            GoogleAPIQueryAutoCompleteObj placesObj = new GoogleAPIQueryAutoCompleteObj();

            try
            {
                placesObj = await GoogleAPIFunction.ConvertAutoCompleteToLLS(inputAddress);

                //2. Create Place list
                ObservableCollection<AutoCompletePlaceLLSObj> autoCompleteDataSource = new ObservableCollection<AutoCompletePlaceLLSObj>();
                lls_OfficeAddress.ItemsSource = autoCompleteDataSource;
                //3. Loop to list all item in object
                if (placesObj.status != "ZERO_RESULTS")
                {
                    ShowLLSOfficeAddress();

                    foreach (var obj in placesObj.predictions)
                    {
                        autoCompleteDataSource.Add(new AutoCompletePlaceLLSObj(obj.description.ToString()));
                    }
                }
                else
                {
                    HideLLSOfficeAddress();
                }


            }
            catch (Exception)
            {

                txt_OfficeAddress.Focus();
            }
        }

        private void img_CloseOffice_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HideAddOfficeAddressGrid();
        }

        private void txt_OfficeAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txt_OfficeAddress.Text != null)
            {
                ShowCloseClearOfficeIcon();
            }
            setCursorAtLast(txt_OfficeAddress);
        }

        private void txt_OfficeAddress_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (txt_OfficeAddress.Text != null)
            {
                ShowCloseClearOfficeIcon();
                ShowSaveOfficeButton();
            }
            OfficeAddressAutoComplete(txt_OfficeAddress.Text);
            //GetHomeCityPlusInfo(txt_HomeAddress.Text);
        }

        private void txt_OfficeAddress_LostFocus(object sender, RoutedEventArgs e)
        {
            setCursorAtFirst(txt_OfficeAddress);
            HideCloseClearOfficeIcon();
        }

        private void txt_OfficeAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void btn_SaveOffice_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UpdateRiderOfficeAddress(txt_OfficeAddress.Text, ConstantVariable.addrTypeOFFICE);
        }

        private void img_CloseClearOfficeAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (txt_OfficeAddress.Text != null)
            {
                txt_OfficeAddress.Text = string.Empty;
            }
            if (txt_OfficeAddress.Text.Equals(null))
            {
                txt_OfficeAddress.Focus();
            }
        }

        private void map_OfficeAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            setCursorAtFirst(txt_OfficeAddress);
        }

        private void map_OfficeAddress_ResolveCompleted(object sender, MapResolveCompletedEventArgs e)
        {
            if (isMovableOfficeMap == true)
            {
                officeLat = map_OfficeAddress.Center.Latitude;
                officeLng = map_OfficeAddress.Center.Longitude;
                ShowAddressFromCoordinate(officeLat, officeLng);
            }
        }

        private async void lls_OfficeAddress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedPlace = ((AutoCompletePlaceLLSObj)(sender as LongListSelector).SelectedItem);
            // If selected item is null, do nothing
            if (lls_OfficeAddress.SelectedItem == null)
            {
                return;
            }

            //show address on textbox
            GoogleAPIAddressObj address = new GoogleAPIAddressObj();
            address = await GoogleAPIFunction.ConvertAddressToLatLng(selectedPlace.Name.ToString());
            txt_OfficeAddress.Text = address.results[0].address_components[0].long_name.ToString()
                                + ", " + address.results[0].address_components[1].long_name.ToString()
                                + ", " + address.results[0].address_components[2].long_name.ToString();
            //txt_City.Text = address.results[0].address_components[address.results[0].address_components.Count - 2].long_name.ToString();

            //Return Lat, Lng, some paramenter here
            //Return Lat, Lng, some paramenter here
            //Return Lat, Lng, some paramenter here

            setCursorAtFirst(txt_OfficeAddress);

            SetOfficePositionFromAddress(address.results[0].formatted_address.ToString());

            //vibrate phone
            vibrateController.Start(TimeSpan.FromSeconds(0.1));

            HideLLSOfficeAddress();
        }
        #endregion



        /// <summary>
        /// HAI HÀM NÀY ĐỂ ĐĂT TRỎ CHUỘT VÀO ĐẦU VÀ CUỐI DÒNG
        /// </summary>
        /// <param name="txtBox"></param>
        private void setCursorAtLast(TextBox txtBox)
        {
            txtBox.SelectionStart = txtBox.Text.Length;
            txtBox.SelectionLength = 0;
        }
        private void setCursorAtFirst(TextBox txtBox)
        {
            txtBox.SelectionStart = 0;
            txtBox.SelectionLength = 0;
        }




        #region Update Home Grid State View
        private void ShowLLSHomeAddress()
        {
            lls_HomeAddress.Visibility = Visibility.Visible;
            lls_HomeAddress.IsEnabled = true;
        }

        private void HideLLSHomeAddress()
        {
            lls_HomeAddress.Visibility = Visibility.Collapsed;
            lls_HomeAddress.IsEnabled = false;
        }

        private void ShowSaveHomeButton()
        {
            btn_SaveHome.Visibility = Visibility.Visible;
        }

        private void HideSaveHomeButton()
        {
            btn_SaveHome.Visibility = Visibility.Visible;
        }

        private void ShowAddHomeAddressGrid()
        {
            (this.Resources["showAddHomeAddressGrid"] as Storyboard).Begin();
            grv_AddHomeAddress.Visibility = Visibility.Visible;
        }

        private void HideAddHomeAddressGrid()
        {
            grv_AddHomeAddress.Visibility = Visibility.Collapsed;
        }
        private void ShowLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Visible;
        }

        private void HideLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Collapsed;
        }


        private void ShowHomeSaveButton()
        {
            btn_Save.Visibility = Visibility.Visible;
            btn_Edit.Visibility = Visibility.Collapsed;
        }

        private void ShowHomeEditButton()
        {
            btn_Edit.Visibility = Visibility.Visible;
            btn_Save.Visibility = Visibility.Collapsed;
        }
        #endregion





        #region Update Office Grid State View
        private void ShowLLSOfficeAddress()
        {
            lls_OfficeAddress.Visibility = Visibility.Visible;
            lls_OfficeAddress.IsEnabled = true;
        }

        private void HideLLSOfficeAddress()
        {
            lls_OfficeAddress.Visibility = Visibility.Collapsed;
            lls_OfficeAddress.IsEnabled = false;
        }

        private void ShowSaveOfficeButton()
        {
            btn_SaveOffice.Visibility = Visibility.Visible;
        }

        private void HideSaveOfficeButton()
        {
            btn_SaveOffice.Visibility = Visibility.Visible;
        }

        private void ShowAddOfficeAddressGrid()
        {
            (this.Resources["showAddOfficeAddressGrid"] as Storyboard).Begin();
            grv_AddOfficeAddress.Visibility = Visibility.Visible;
        }

        private void HideAddOfficeAddressGrid()
        {
            grv_AddOfficeAddress.Visibility = Visibility.Collapsed;
        }

        private void ShowOfficeSaveButton()
        {
            btn_Save.Visibility = Visibility.Visible;
            btn_Edit.Visibility = Visibility.Collapsed;
        }

        private void ShowOfficeEditButton()
        {
            btn_Edit.Visibility = Visibility.Visible;
            btn_Save.Visibility = Visibility.Collapsed;
        }

        private void ShowCloseClearOfficeIcon()
        {
            img_CloseClearOfficeAddress.Visibility = Visibility.Visible;
        }
        private void HideCloseClearOfficeIcon()
        {
            img_CloseClearOfficeAddress.Visibility = Visibility.Visible;
        }

        #endregion



    }
}