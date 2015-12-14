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
using FT_Driver.Classes;
using ListPickerDemo;
using Microsoft.Devices;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Device.Location;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Media.Animation;
using System.Diagnostics;



namespace FT_Driver.Pages
{
    public partial class DriverProfile : PhoneApplicationPage
    {
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        DriverLogin userData = null;
        string pwmd5 = string.Empty;
        long preOlmd;
        string userId = string.Empty;
        IDictionary<string, DriverGetCityList> cityNamesDB = null;

        //For home Address Update
        double homeLat = 0;
        double homeLng = 0;
        int homeCityId = 0;
        string homeCntry = String.Empty;
        string homeCity = string.Empty;
        bool isMovableHomeMap = false;

        VibrateController vibrateController = VibrateController.Default;

        public DriverProfile()
        {
            InitializeComponent();

            //Load drver profile
            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userData = new DriverLogin();
                cityNamesDB = new Dictionary<string, DriverGetCityList>();
                userData = (DriverLogin)tNetUserLoginData["UserLoginData"];
                pwmd5 = (string)tNetUserLoginData["PasswordMd5"];
                preOlmd = (long)tNetUserLoginData["UserLmd"];
                cityNamesDB = (IDictionary<string, DriverGetCityList>)tNetUserLoginData["CityNamesDB"];
                userId = (string)tNetUserLoginData["UserId"];
            }

            tbl_OfficeAddress_Show.Text = tbl_OfficeAddress.Text;
            txt_HomeAddress.Text = tbl_HomeAddress.Text;

            LoadDriverProfile();
        }

        private void LoadDriverProfile()
        {
            if (userData.content.driverInfo.fName != null)
            {
                txt_FirstName.Text = userData.content.driverInfo.fName;
            }
            else
            {
                txt_FirstName.Text = string.Empty;
            }

            if (userData.content.driverInfo.lName != null)
            {
                txt_LastName.Text = userData.content.driverInfo.lName;
            }
            else
            {
                txt_LastName.Text = string.Empty;
            }


            if (userData.content.driverInfo.email != null)
            {
                txt_Email.Text = userData.content.driverInfo.email;
            }
            else
            {
                txt_Email.Text = string.Empty;
            }

            if (userData.content.driverInfo.balance != null)
            {
                txt_Balance.Text = userData.content.driverInfo.balance.ToString() + "VNĐ";
            }
            else
            {
                txt_Balance.Text = string.Empty;
            }

            if (userData.content.driverInfo.mobile != null)
            {
                txt_Mobile.Text = userData.content.driverInfo.mobile;
            }
            else
            {
                txt_Mobile.Text = string.Empty;
            }

            if (userData.content.driverInfo.hAdd != null)
            {
                tbl_HomeAddress.Text = userData.content.driverInfo.hAdd;
            }
            else
            {
                tbl_HomeAddress.Text = string.Empty;
            }


            if (userData.content.companyInfo.add != null)
            {
                tbl_OfficeAddress.Text = userData.content.companyInfo.add;
            }
            else
            {
                tbl_OfficeAddress.Text = string.Empty;
            }
        }


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


        private void img_EditIcon_HomeAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowAddHomeAddressGrid();
        }




        private void tbl_Tap_ChangePassword(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/DriverChangePassword.xaml", UriKind.Relative));
        }

        private void tbl_OfficeAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowOfficeAddressGrid();
        }

        private void img_CloseHome_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HideAddHomeAddressGrid();
        }



        private void txt_HomeAddress_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (txt_HomeAddress.Text != null)
            {
                ShowCloseClearHomeIcon();
                ShowSaveHomeButton();
            }
            HomeAddressAutoComplete(txt_HomeAddress.Text);
        }


        private async void HomeAddressAutoComplete(string inputAddress)
        {
            GoogleAPIQueryAutoCompleteObj placesObj = new GoogleAPIQueryAutoCompleteObj();

            try
            {
                placesObj = await GoogleAPIFunctions.ConvertAutoCompleteToLLS(inputAddress);

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




        private void txt_HomeAddress_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

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
                    var id = userData.content.driverInfo.did;
                    var nCity = homeCity;
                    var nAdd = txt_HomeAddress.Text;
                    double lat = homeLat;
                    double lng = homeLng;
                    var addrType = type;
                    long lmd = preOlmd;
                    var role = ConstantVariable.dRole;
                    var cityId = homeCityId;
                    var cntry = homeCntry;
                    var uid = userId;
                    var pw = pwmd5;

                    var input2 = string.Format("{{\"id\":\"{0}\",\"nCity\":\"{1}\",\"nAdd\":\"{2}\",\"lat\":\"{3}\",\"lng\":\"{4}\",\"addrType\":\"{5}\",\"olmd\":\"{6}\",\"role\":\"{7}\",\"cityId\":\"{8}\",\"cntry\":\"{9}\",\"uid\":\"{10}\",\"pw\":\"{11}\"}}", id, nCity, nAdd, lat, lng, addrType, lmd, role, cityId, cntry, uid, pw);
                    try
                    {
                        //Thủ xem có lấy đc gì k
                        var output2 = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverUpdateAddress, input2);
                        var updateStatus = JsonConvert.DeserializeObject<BaseResponse>(output2);
                        if (updateStatus.status != null)
                        {
                            if (updateStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS))//ok 0000
                            {
                                tNetUserLoginData["UserLmd"] = updateStatus.lmd;
                                preOlmd = updateStatus.lmd;

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
                            MessageBox.Show("(Mã lỗi 6201) " + ConstantVariable.errServerError); //Co loi may chu
                            Debug.WriteLine("Có lỗi 356fgh ở update home address");
                        }
                    }
                    catch (Exception)
                    {
                        HideLoadingScreen();
                        MessageBox.Show("(Mã lỗi 6202) " + ConstantVariable.errServerError); //Co loi may chu
                        Debug.WriteLine("Có lỗi 652rt ở update home address");
                    }
                }
                else
                {
                    MessageBox.Show("(Mã lỗi 6203) " + ConstantVariable.errServerError);
                    Debug.WriteLine("Có lỗi 26644dd ở get Google String");
                }
            }
            catch (Exception)
            {

                MessageBox.Show("(Mã lỗi 6204) " + ConstantVariable.errServerError);
                Debug.WriteLine("Có lỗi 214dw ở get Google String");
            }

        }



        private void txt_HomeAddress_LostFocus(object sender, RoutedEventArgs e)
        {
            setCursorAtFirst(txt_HomeAddress);
            HideCloseClearHomeIcon();
        }





        private void btn_SaveHome_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UpdateRiderHomeAddress(txt_HomeAddress.Text, ConstantVariable.addrTypeHOME);
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

        private void map_HomeAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            setCursorAtFirst(txt_HomeAddress);
        }

        private void map_HomeAddress_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void map_HomeAddress_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            isMovableHomeMap = true;
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
                var str = await GoogleAPIFunctions.ConvertLatLngToAddress(lat, lng);
                var address = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(str);
                txt_HomeAddress.Text = address.results[0].formatted_address.ToString();
            }
            catch (Exception)
            {

                MessageBox.Show("(Mã lỗi 25652) " + ConstantVariable.errConnectingError);
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
            address = await GoogleAPIFunctions.ConvertAddressToLatLng(selectedPlace.Name.ToString());
            txt_HomeAddress.Text = address.results[0].address_components[0].long_name.ToString()
                                + ", " + address.results[0].address_components[1].long_name.ToString()
                                + ", " + address.results[0].address_components[2].long_name.ToString();

            setCursorAtFirst(txt_HomeAddress);

            SetPositionFromAddress(address.results[0].formatted_address.ToString());

            //vibrate phone
            vibrateController.Start(TimeSpan.FromSeconds(0.1));

            HideLLSHomeAddress();
        }

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

        private void ShowSaveHomeButton()
        {
            btn_SaveHome.Visibility = Visibility.Visible;
        }
        private void HideAddHomeAddressGrid()
        {
            grv_AddHomeAddress.Visibility = Visibility.Collapsed;
        }

        private void txt_HomeAddress_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txt_HomeAddress.Text != null)
            {
                ShowCloseClearHomeIcon();
            }
            setCursorAtLast(txt_HomeAddress);
        }
        private void ShowLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Visible;
        }

        private void HideLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Collapsed;
        }
        private void ShowAddHomeAddressGrid()
        {
            (this.Resources["showAddHomeAddressGrid"] as Storyboard).Begin();
            grv_AddHomeAddress.Visibility = Visibility.Visible;
        }
        private void HideCloseClearHomeIcon()
        {
            img_CloseClearHomeAddress.Visibility = Visibility.Collapsed;
        }
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
        private void ShowCloseClearHomeIcon()
        {
            img_CloseClearHomeAddress.Visibility = Visibility.Visible;
        }
        private void HideSaveHomeButton()
        {
            btn_SaveHome.Visibility = Visibility.Visible;
        }

        private void img_CloseOffice_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HideOfficeAddressGrid();
        }

        private void ShowOfficeAddressGrid()
        {
            (this.Resources["showOfficeAddressGrid"] as Storyboard).Begin();
            grv_OfficeAddress.Visibility = Visibility.Visible;
            if (tbl_OfficeAddress.Text != null)
            {
                LoadOfficeMapToScreen(tbl_OfficeAddress.Text);
                tbl_OfficeAddress_Show.Text = tbl_OfficeAddress.Text;
            }            
        }

        private async void LoadOfficeMapToScreen(string inputAddress)
        {
            GoogleAPIAddressObj address = new GoogleAPIAddressObj();
            address = await GoogleAPIFunctions.ConvertAddressToLatLng(inputAddress);

            map_OfficeAddress.SetView(new GeoCoordinate(address.results[0].geometry.location.lat, address.results[0].geometry.location.lng), 16, MapAnimationKind.Linear);
        }
        private void HideOfficeAddressGrid()
        {
            grv_OfficeAddress.Visibility = Visibility.Collapsed;
        }
    }
}