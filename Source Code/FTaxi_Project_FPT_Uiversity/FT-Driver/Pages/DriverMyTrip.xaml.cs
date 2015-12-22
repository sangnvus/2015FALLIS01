using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;
using System.Diagnostics;
using FT_Driver.Classes;
using System.IO.IsolatedStorage;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace FT_Driver.Pages
{
    public partial class DriverMyTrip : PhoneApplicationPage
    {
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        DriverLogin userData;
        string password = string.Empty;
        string userId = string.Empty;
        string selectedTripId = string.Empty;
        string selectedRiderId = string.Empty;
        DriverGetMyTrip myTrip = null;
        //For store trip data
        IDictionary<string, DriverTripItemObj> myTripCollection = new Dictionary<string, DriverTripItemObj>();
        public DriverMyTrip()
        {
            InitializeComponent();
            //Get User data from login
            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userData = new DriverLogin();
                userData = (DriverLogin)tNetUserLoginData["UserLoginData"];
                password = (string)tNetUserLoginData["PasswordMd5"];
                userId = (string)tNetUserLoginData["UserId"];
            }
            GetMyTripData();
            //Load My trip
        }

        private void ShowInforOnDeteilPanel()
        {
            txt_DriverName.Text = myTripCollection[selectedTripId].rName;
            txt_Mobile.Text = myTripCollection[selectedTripId].mobile;
            txt_From.Text = myTripCollection[selectedTripId].sAdd;
            txt_To.Text = myTripCollection[selectedTripId].eAdd;

            txt_CarLevel.Text = myTripCollection[selectedTripId].eTime;
            txt_Distance.Text = myTripCollection[selectedTripId].distance.ToString();
            txt_TotalCost.Text = myTripCollection[selectedTripId].fare.ToString();
        }

        private async void GetMyTripData()
        {
            ShowLoadingScreen();
            //Hiện Loading


            var uid = userId;
            var pw = password;
            var page = "0";

            var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"page\":\"{2}\"}}", uid, pw, page);
            try
            {
                //Thử xem có lấy đc JSON về ko, nếu ko thì bắn ra Lối kết nối / lỗi server
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverGetMyTrip, input);
                myTrip = new DriverGetMyTrip();
                myTrip = JsonConvert.DeserializeObject<DriverGetMyTrip>(output);


                if (myTrip.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS)) //0000
                {
                    //Nếu có thông tin chuyến đi thì bắt đầu lấy về
                    try
                    {
                        //1. Parse Object and Load to LLS
                        ObservableCollection<DriverMyTripLLSObj> myTripDataSource = new ObservableCollection<DriverMyTripLLSObj>();
                        lls_MyTrip.ItemsSource = myTripDataSource;


                        //2. Loop to list all item in object
                        foreach (var trip in myTrip.content.list)
                        {
                            myTripDataSource.Add(new DriverMyTripLLSObj(trip.tid, trip.sAdd, trip.eAdd, trip.eTime, trip.rName, trip.mobile, trip.fare, trip.rate));
                            myTripCollection[trip.tid.ToString()] = new DriverTripItemObj
                            {
                                tid = trip.tid,
                                rid = trip.rid,
                                sAdd = trip.sAdd,
                                eAdd = trip.eAdd,
                                sTime = trip.sTime,
                                eTime = trip.eTime,
                                rName = trip.rName,
                                mobile = trip.mobile,
                                vip = trip.vip,
                                fare = trip.fare,
                                distance = trip.distance,
                                payment = trip.payment,
                                currency = trip.currency,
                                rate = trip.rate,
                                fName = trip.fName,
                                lName = trip.lName,
                                img = trip.img
                            };
                        }
                        //Tắt loading data
                        HideLoadingScreen();
                    }
                    catch (Exception)
                    {

                        MessageBox.Show("(Mã lỗi 36958) " + ConstantVariable.errHasErrInProcess);
                        Debug.WriteLine("Có lỗi 2365 ở Get Trip Info");
                    }
                }
                else
                {
                    txt_MyTripStatus.Text = ConstantVariable.strNoTripInfo;
                    Debug.WriteLine("Có lỗi 558ki ở Get Trip Info");
                }
            }
            catch (Exception)
            {

                MessageBox.Show("(Mã lỗi 6325) " + ConstantVariable.errConnectingError);
                Debug.WriteLine("Có lỗi 125kji ở Get Trip Info");
            }
        }

        private void ShowDriverLostAssets()
        {
            (this.Resources["showDriverLostAssets"] as Storyboard).Begin();
            grv_DriverAssetsLost.Visibility = Visibility.Visible;
        }
        private void HideDriverLostAssets()
        {
            grv_DriverAssetsLost.Visibility = Visibility.Collapsed;
        }
        private void btn_ViewTripDetail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Ẩn menu bên dưới
            HideBottomMenu();

            //hiện Grid Detail Trip
            ShowTripDetailScreen();

            //Show infor
            ShowInforOnDeteilPanel();
        }

        private void btn_AlertAssets_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowDriverLostAssets();
            HideBottomMenu();            
        }

        private async void SendFeedback()
        {
            ShowLoadingScreen();
            var did = userData.content.driverInfo.did;
            var title = userData.content.driverInfo.fName + " " + userData.content.driverInfo.lName + " Báo mất đồ";
            var content = txt_Content.Text;
            var uid = userId;
            var pw = password;
            var input = string.Format("{{\"did\":\"{0}\",\"title\":\"{1}\",\"content\":\"{2}\",\"uid\":\"{3}\",\"pw\":\"{4}\"}}", did, title, content, uid, pw);
            try
            {
                //Thử xem có lấy đc JSON về ko, nếu ko thì bắn ra Lối kết nối / lỗi server
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverSendFeedback, input);
                var lostStatus = JsonConvert.DeserializeObject<BaseResponse>(output);

                if (lostStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS)) //0000
                {
                    //Nếu OK thi
                    HideLoadingScreen();
                    HideDriverLostAssets();
                    MessageBox.Show("Thông báo của bạn đã được gửi lên thành công."); //Thêm lái xe thành công

                }
                else
                {

                    Debug.WriteLine("Có lỗi 265gyut ở driver send lost");
                }
            }
            catch (Exception)
            {
                HideLoadingScreen();
                MessageBox.Show("(Mã lỗi 5689) " + ConstantVariable.errConnectingError);
                Debug.WriteLine("Có lỗi ghjtr54 ở Lost Asset");
            }
        }

        private void HideLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Collapsed;
        }
        private void ShowLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Visible;
        }

        private void btn_Cancel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HideBottomMenu();
        }


        private void btn_Close_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Hide Detail Trip Screen
            HideTripDetailScreen();
        }

        private void lls_MyTrip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ///1. Hiện Menu lựa chọn chức năng
            ///2. Truyền tham số vào
            ///3. ?
            ///

            // Hiện menu
            ShowBottomMenu();
            var selectedTrip = ((DriverMyTripLLSObj)(sender as LongListSelector).SelectedItem);
            // If selected item is null, do nothing
            if (lls_MyTrip.SelectedItem == null)
                return;

            selectedTripId = selectedTrip.Tid; // Khai báo rằng con Trip Id này đã được chọn

            // Reset selected item to null
            lls_MyTrip.SelectedItem = null;
        }

        /// <summary>
        /// HIEN MENU
        /// </summary>
        private void ShowBottomMenu()
        {
            (this.Resources["showMenu"] as Storyboard).Begin();
            grv_FunctionMenu.Visibility = Visibility.Visible;
        }

        private void HideBottomMenu()
        {
            grv_FunctionMenu.Visibility = Visibility.Collapsed;
        }

        private void ShowButtonLoadingScreen()
        {
            grv_ButtonLoadingScreen.Visibility = Visibility.Visible;
        }

        private void HideButtonLoadingScreen()
        {
            grv_ButtonLoadingScreen.Visibility = Visibility.Collapsed;
        }

        private void img_CloseDriverLostAsset_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HideDriverLostAssets();
            HideBottomMenu();
        }

        private void btn_SendLostAsset_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (txt_Content.Text.Equals(string.Empty))
            {
                MessageBox.Show("Vui lòng cung cấp thêm một vài thông tin. Xin cảm ơn.");
                txt_Content.Focus();
            }
            else
            {
                SendFeedback();
            }
        }

        private void ShowTripDetailScreen()
        {
            (this.Resources["showDetail"] as Storyboard).Begin();
            grv_TripDetail.Visibility = Visibility.Visible;
        }


        private void HideTripDetailScreen()
        {
            grv_TripDetail.Visibility = Visibility.Collapsed;
        }
    }
}