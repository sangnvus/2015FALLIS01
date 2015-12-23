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
using FT_Rider.Classes;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Windows.Media.Animation;

namespace FT_Rider.Pages
{
    public partial class RiderMyTrip : PhoneApplicationPage
    {
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        RiderLogin userData;
        string password = string.Empty;
        RiderGetMyTrip myTrip = null;

        //For store trip data
        IDictionary<string, RiderMyTripItemObj> myTripCollection = new Dictionary<string, RiderMyTripItemObj>();

        //For view trip detail
        string selectedTripId = string.Empty;
        string selectedDriverId = string.Empty;

        public RiderMyTrip()
        {
            InitializeComponent();

            //Get User data from login
            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userData = new RiderLogin();
                userData = (RiderLogin)tNetUserLoginData["UserLoginData"];
                password = (string)tNetUserLoginData["PasswordMd5"];
            }

            //Load My trip
            GetMyTripData();
        }

        private async void GetMyTripData()
        {
            ShowLoadingScreen();
            //Hiện Loading


            var uid = userData.content.uid;
            var pw = password;
            var page = "0";

            var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"page\":\"{2}\"}}", uid, pw, page);
            try
            {
                //Thử xem có lấy đc JSON về ko, nếu ko thì bắn ra Lối kết nối / lỗi server
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderGetMyTrip, input);
                myTrip = new RiderGetMyTrip();
                myTrip = JsonConvert.DeserializeObject<RiderGetMyTrip>(output);


                if (myTrip.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS)) //0000
                {
                    //Nếu có thông tin chuyến đi thì bắt đầu lấy về
                    try
                    {
                        //1. Parse Object and Load to LLS
                        ObservableCollection<RiderMyTripLLSObj> myTripDataSource = new ObservableCollection<RiderMyTripLLSObj>();
                        lls_MyTrip.ItemsSource = myTripDataSource;


                        //2. Loop to list all item in object
                        foreach (var trip in myTrip.content.list)
                        {
                            myTripDataSource.Add(new RiderMyTripLLSObj(trip.tid, trip.did, trip.dName, trip.dMobile, trip.plate, trip.cLvl, trip.from, trip.to, trip.sTime, trip.eTime, trip.distance, RiderFunctions.RoundMoney((double)trip.fare, -3), trip.payment, trip.currency, trip.fName, trip.lName, trip.rate, trip.interCode, trip.favorite));
                            myTripCollection[trip.tid.ToString()] = new RiderMyTripItemObj
                            {
                                tid = trip.tid,
                                did = trip.did,
                                dName = trip.dName,
                                dMobile = trip.dMobile,
                                plate = trip.plate,
                                cLvl = trip.cLvl,
                                from = trip.from,
                                to = trip.to,
                                sTime = trip.sTime,
                                eTime = trip.eTime,
                                distance = trip.distance,
                                fare = trip.fare,
                                payment = trip.payment,
                                currency = trip.currency,
                                fName = trip.fName,
                                lName = trip.lName,
                                rate = trip.rate,
                                interCode = trip.interCode,
                                favorite = trip.favorite
                            };
                        }
                        //Tắt loading data
                        HideLoadingScreen();
                    }
                    catch (Exception)
                    {

                        MessageBox.Show(ConstantVariable.errHasErrInProcess);
                        Debug.WriteLine("Có lỗi 4fe44t4 ở Get Trip Info");
                    }
                }
                else
                {
                    txt_MyTripStatus.Text = ConstantVariable.strNoTripInfo;
                    Debug.WriteLine("Có lỗi 4h87sf ở Get Trip Info");
                }
            }
            catch (Exception)
            {

                MessageBox.Show("(Mã lỗi 1201) " + ConstantVariable.errConnectingError);
                Debug.WriteLine("Có lỗi 4hfffsf ở Get Trip Info");
            }

        }


        /// <summary>
        /// Khi nhấn vào 1 trip
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lls_MyTrip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ///1. Hiện Menu lựa chọn chức năng
            ///2. Truyền tham số vào
            ///3. ?
            ///

            // Hiện menu
            ShowBottomMenu();
            var selectedTrip = ((RiderMyTripLLSObj)(sender as LongListSelector).SelectedItem);
            // If selected item is null, do nothing
            if (lls_MyTrip.SelectedItem == null)
                return;

            selectedTripId = selectedTrip.Tid; // Khai báo rằng con Trip Id này đã được chọn
            selectedDriverId = selectedTrip.Did;

            // Reset selected item to null
            lls_MyTrip.SelectedItem = null;
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

        private void ShowInforOnDeteilPanel()
        {
            var carLevel = "";
            switch (myTripCollection[selectedTripId].cLvl)
            {
                case "ECO":
                    carLevel = "Kinh tế";
                    break;
                case "SAV":
                    carLevel = "Tiết kiệm";
                    break;
                case "LUX":
                    carLevel = "Sang trọng";
                    break;
            }

            txt_DriverName.Text = myTripCollection[selectedTripId].dName;
            txt_Mobile.Text = myTripCollection[selectedTripId].dMobile;
            txt_From.Text = myTripCollection[selectedTripId].from;
            txt_To.Text = myTripCollection[selectedTripId].to;

            txt_Date.Text = carLevel;
            txt_Distance.Text = myTripCollection[selectedTripId].distance.ToString();
            txt_TotalCost.Text = string.Format("{0:#,##0}", RiderFunctions.RoundMoney((double)myTripCollection[selectedTripId].fare, -3)); //làm tròn tiền
        }

        private void btn_CallToDriver_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Quay số cho tài xế
            if (myTripCollection[selectedTripId].dMobile != null)
            {
                RiderFunctions.CallToNumber(myTripCollection[selectedTripId].dName, myTripCollection[selectedTripId].dMobile);
            }
            else
            {
                MessageBox.Show("(Mã lỗi 1202) " + ConstantVariable.strNoDriverNumber);
                Debug.WriteLine("Mã lỗi dfef444 ở btn_CallToDriver_Tap");
            }

        }


        private void btn_Cancel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HideBottomMenu();
        }

        /// HÀM NÀY ĐỂ HIỆN LOADING
        /// </summary>
        private void ShowLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// HÀM NÀY ĐỂ TẮT LOADING
        /// </summary>
        private void HideLoadingScreen()
        {
            grv_ProcessScreen.Visibility = Visibility.Collapsed;
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


        private void ShowTripDetailScreen()
        {
            (this.Resources["showDetail"] as Storyboard).Begin();
            grv_TripDetail.Visibility = Visibility.Visible;
        }


        private void HideTripDetailScreen()
        {
            grv_TripDetail.Visibility = Visibility.Collapsed;
        }

        private void btn_Close_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Hide Detail Trip Screen
            HideTripDetailScreen();
        }

        private void btn_AlertAssets_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowLostAssetGrid();
            HideBottomMenu();
        }

        private async void btn_AddFavorite_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowLoadingScreen();

            //Code thêm vào yêu thích
            var uid = userData.content.uid;
            var rid = userData.content.rid;
            var did = selectedDriverId;

            var input = string.Format("{{\"uid\":\"{0}\",\"rid\":\"{1}\",\"did\":\"{2}\"}}", uid, rid, did);
            try
            {
                //Thử xem có lấy đc JSON về ko, nếu ko thì bắn ra Lối kết nối / lỗi server
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderAddMyFarvoriteDriver, input);
                var addStatus = JsonConvert.DeserializeObject<BaseResponse>(output);

                if (addStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS)) //0000
                {
                    //Nếu OK thi
                    HideLoadingScreen();
                    MessageBox.Show(ConstantVariable.strAddFavoriteSuccess); //Thêm lái xe thành công
                }
                else
                {
                    HideLoadingScreen();
                    Debug.WriteLine("Có lỗi 25696635ggg ở Add driver favorite");
                }
            }
            catch (Exception)
            {
                HideLoadingScreen();
                MessageBox.Show("(Mã lỗi 1229) " + ConstantVariable.errConnectingError);
                Debug.WriteLine("Có lỗi 65fgh2676 ở Add driver favorite");
            }

        }

        private void img_CloseRiderLostAsset_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HideLostAssetGrid();
            HideBottomMenu();
        }

        private void btn_SendLostAsset_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (txt_Content.Text.Equals(string.Empty))
            {
                MessageBox.Show("Vui lòng cung cấp thêm một vài thông tin. Xin cảm ơn.");
            }
            SendFeedback();
        }

        private async void SendFeedback()
        {
            var rid = userData.content.rid;
            var title = userData.content.fName + " " + userData.content.lName + " Báo mất đồ";
            var content = txt_Content.Text;
            var uid = userData.content.uid;
            var pw = password;
            var input = string.Format("{{\"rid\":\"{0}\",\"title\":\"{1}\",\"content\":\"{2}\",\"uid\":\"{3}\",\"pw\":\"{4}\"}}", rid, title, content, uid, pw);
            try
            {
                //Thử xem có lấy đc JSON về ko, nếu ko thì bắn ra Lối kết nối / lỗi server
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderSendFeedback, input);
                var lostStatus = JsonConvert.DeserializeObject<BaseResponse>(output);

                if (lostStatus.status.Equals(ConstantVariable.RESPONSECODE_SUCCESS)) //0000
                {
                    //Nếu OK thi
                    HideLoadingScreen();
                    HideBottomMenu();
                    MessageBox.Show("Thông báo của bạn đã được gửi lên thành công."); //Thêm lái xe thành công

                }
                else
                {

                    Debug.WriteLine("Có lỗi 26f86 ở Add driver favorite");
                }
            }
            catch (Exception)
            {

                MessageBox.Show("(Mã lỗi 1289) " + ConstantVariable.errConnectingError);
                Debug.WriteLine("Có lỗi ff5726 ở Lost Asset");
            }
        }


        private void ShowLostAssetGrid()
        {
            (this.Resources["showLostAssetGrid"] as Storyboard).Begin();
            grv_RiderLossAssets.Visibility = Visibility.Visible;
        }

        private void HideLostAssetGrid()
        {
            grv_RiderLossAssets.Visibility = Visibility.Collapsed;
        }
    }
}