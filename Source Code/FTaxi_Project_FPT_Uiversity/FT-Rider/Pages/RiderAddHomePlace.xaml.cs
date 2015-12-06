using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using FT_Rider.Classes;
using FT_Rider.Resources;
using Newtonsoft.Json;
using System.Device.Location;
using System.Collections.ObjectModel;
using Microsoft.Devices;
using System.IO.IsolatedStorage;
using System.Diagnostics;

namespace FT_Rider.Pages
{
    public partial class RiderAddHomePlace : PhoneApplicationPage
    {
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        RiderLogin userData = null;
        string pwmd5 = string.Empty;
        long preOlmd;

        IDictionary<string, RiderGetCityList> cityNamesDB = null;


        //For Update
        double myLat;
        double myLng;
        int myCityId;
        string myCntry;
        

        VibrateController vibrateController = VibrateController.Default;
        public RiderAddHomePlace()
        {
            InitializeComponent();

            //Input lat & lng Parameter in this function to load address


            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userData = new RiderLogin();
                cityNamesDB = new Dictionary<string, RiderGetCityList>();
                userData = (RiderLogin)tNetUserLoginData["UserLoginData"];
                pwmd5 = (string)tNetUserLoginData["PasswordMd5"];
                preOlmd = (long)tNetUserLoginData["UserLmd"];
                cityNamesDB = (IDictionary<string, RiderGetCityList>)tNetUserLoginData["CityNamesDB"];

            }

            this.LoadLocationOnMap(21.038472, 105.8014108);



            //Set status of control
            //this.lls_AutoComplete.IsEnabled = false;
            //txt_City.IsEnabled = false;

        }

        private void isLlsOff()
        {
            this.lls_AutoComplete.Visibility = Visibility.Collapsed;
            this.lls_AutoComplete.IsEnabled = false;
        }

        private void isLlsOn()
        {
            this.lls_AutoComplete.Visibility = Visibility.Visible;
            this.lls_AutoComplete.IsEnabled = true;
        }


        private async void LoadLocationOnMap(double lat, double lng)
        {
            //Show marker
            MapShowMarker myMarker = new MapShowMarker();
            myMarker.ShowPointOnMap(lat, lng, map_RiderHome, 16);

            //Convert Lat & Lng to Address String
            string jsonString = await GoogleAPIFunction.ConvertLatLngToAddress(lat, lng); //Get Json String
            GoogleAPILatLngObj address = new GoogleAPILatLngObj();
            address = JsonConvert.DeserializeObject<GoogleAPILatLngObj>(jsonString); //Convert to Obj

            //Load address to screen
            txt_Address.Text = address.results[0].address_components[0].long_name.ToString() + ", " + address.results[0].address_components[1].long_name.ToString();
            txt_City.Text = address.results[0].address_components[3].long_name.ToString();

        }

        private void txt_Address_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Enable LLS
            isLlsOn();
            //Enable 
            img_ClearText.Visibility = Visibility.Visible;

        }

        private void img_ClearText_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (txt_Address.Text == null)
            {
                isLlsOff();
                img_ClearText.Visibility = Visibility.Collapsed;
            }
            else
            {
                txt_Address.Text = string.Empty;
                txt_Address.Focus();
            }
        }

        private void txt_Address_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string inputAddress = txt_Address.Text;
            AutoCompletePlace(inputAddress);
        }

        private async void AutoCompletePlace(string inputAddress)
        {
            GoogleAPIQueryAutoCompleteObj placesObj = new GoogleAPIQueryAutoCompleteObj();

            try
            {
                placesObj = await GoogleAPIFunction.ConvertAutoCompleteToLLS(inputAddress);

                //2. Create Place list
                ObservableCollection<AutoCompletePlaceLLSObj> autoCompleteDataSource = new ObservableCollection<AutoCompletePlaceLLSObj>();
                lls_AutoComplete.ItemsSource = autoCompleteDataSource;
                //3. Loop to list all item in object
                foreach (var obj in placesObj.predictions)
                {
                    autoCompleteDataSource.Add(new AutoCompletePlaceLLSObj(obj.description.ToString()));
                }
            }
            catch (Exception)
            {

                txt_Address.Focus();
            }
        }

        private async void lls_AutoComplete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedPlace = ((AutoCompletePlaceLLSObj)(sender as LongListSelector).SelectedItem);
            // If selected item is null, do nothing
            if (lls_AutoComplete.SelectedItem == null)
            {
                return;
            }

            //show address on textbox
            GoogleAPIAddressObj address = new GoogleAPIAddressObj();
            address = await GoogleAPIFunction.ConvertAddressToLatLng(selectedPlace.Name.ToString());
            txt_Address.Text = address.results[0].address_components[0].long_name.ToString()
                                + ", " + address.results[0].address_components[1].long_name.ToString()
                                + ", " + address.results[0].address_components[2].long_name.ToString();
            txt_City.Text = address.results[0].address_components[address.results[0].address_components.Count - 2].long_name.ToString();

            //Return Lat, Lng, some paramenter here
            //Return Lat, Lng, some paramenter here
            //Return Lat, Lng, some paramenter here

            setCursorAtLast(txt_Address);

            //vibrate phone
            vibrateController.Start(TimeSpan.FromSeconds(0.1));

            isLlsOff();
        }

        private void setCursorAtLast(TextBox txtBox)
        {
            txtBox.SelectionStart = txtBox.Text.Length; // add some logic if length is 0
            txtBox.SelectionLength = 0;
        }

        private void txt_Address_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox addressTextbox = (TextBox)sender;
            addressTextbox.Background = new SolidColorBrush(Colors.Transparent);
            addressTextbox.BorderBrush = new SolidColorBrush(Colors.Transparent);
        }

        private async void txt_Address_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //check if input is "Enter" key
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                //Show Loading
                ShowLoadingScreen();

                //Lấy thông tin tọa độ
                var input = ConstantVariable.googleAPIGeocodingAddressBaseURI + txt_Address.Text + "&key=" + ConstantVariable.googleGeolocationAPIkey;
                try
                {
                    var output = await ReqAndRes.GetJsonString(input);
                    var myLocation = JsonConvert.DeserializeObject<GoogleAPIAddressObj>(output);
                    if (myLocation.status.Equals(ConstantVariable.googleResponseStatusOK)) //OK
                    {
                       myCntry = myLocation.results[0].address_components[myLocation.results[0].address_components.Count-1].short_name.ToString();

                        //Trả về cityCode
                       myCityId = (GetCityCodeFromCityName(myLocation.results[0].address_components[myLocation.results[0].address_components.Count - 2].long_name.ToString()));

                        //Tra ve lat long
                       myLat = myLocation.results[0].geometry.location.lat;
                       myLng = myLocation.results[0].geometry.location.lng;
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


                //Lay xong thong tin thi:...
                //Chạy hàm cập nhật địa chỉ nhả
                UpdateHomeAddress();

                //Update xong thi tat
            }
        }

        private async void UpdateHomeAddress()
        {
            var id = userData.content.rid;
            var nCity = txt_City.Text;
            var nAdd = txt_Address.Text;
            double lat = myLat;
            double lng = myLng;
            var addrType = ConstantVariable.addrTypeHOME;
            long lmd = preOlmd;
            var role = ConstantVariable.dRole;
            var cityId = myCityId;
            var cntry = myCntry;
            var uid = userData.content.uid;
            var pw = pwmd5;

            var input = string.Format("{{\"id\":\"{0}\",\"nCity\":\"{1}\",\"nAdd\":\"{2}\",\"lat\":\"{3}\",\"lng\":\"{4}\",\"addrType\":\"{5}\",\"olmd\":\"{6}\",\"role\":\"{7}\",\"cityId\":\"{8}\",\"cntry\":\"{9}\",\"uid\":\"{10}\",\"pw\":\"{11}\"}}", id, nCity, nAdd, lat, lng, addrType, lmd, role, cityId, cntry, uid, pw);
            try
            {
                //Thủ xem có lấy đc gì k
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderUpdateAddress, input);
                var updateStatus = JsonConvert.DeserializeObject<BaseResponse>(output);
                if (updateStatus.content.Equals(ConstantVariable.responseCodeSuccess)) //ok 0000
                {
                    //Neu ok thi se
                    ///2. cap nhat lmd
                    ///3. tat loading
                    ///4. mess
                    ///

                    //2.
                    tNetUserLoginData["UserLmd"] = lmd;

                    //3.
                    HideLoadingScreen();

                    //4.
                    MessageBox.Show(ConstantVariable.strRiderUpdateSuccess); //ok
                }
            }
            catch (Exception)
            {
                HideLoadingScreen();
                MessageBox.Show("(Mã lỗi 2605) " + ConstantVariable.errServerErr); //Co loi may chu
                Debug.WriteLine("Có lỗi 58565dfg ở update home address");    
            }

        }

        //This function to get City Code From City Name in City Dictionary
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


        private void ShowLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Visible;
        }
        private void HideLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Collapsed;
        }
    }
}