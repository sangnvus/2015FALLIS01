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
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Animation;

namespace FT_Rider.Pages
{
    public partial class RiderMyFavoriteDriver : PhoneApplicationPage
    {
        //Cái này để lấy dữ liệu làm việc của User sau khi login
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        RiderLogin userData = null;
        RiderGetListFavoriteDriver myFavorite = null;


        //For call and delete favorite Driver
        string driverName = string.Empty;
        string driverMobile = string.Empty;
        string driverId = string.Empty;

        public RiderMyFavoriteDriver()
        {
            InitializeComponent();

            //Lấy dữ liệu user
            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userData = new RiderLogin();
                userData = (RiderLogin)tNetUserLoginData["UserLoginData"];
            }

            //Load Farvorite
            GetMyFarvoriteDriver();
        }

        private async void GetMyFarvoriteDriver()
        {
            ShowLoadingScreen();//hàm này để hiện thông tin

            var id = userData.content.rid; //Cái này chính là Rider ID (Rider Unique Identify Number)
            var page = 0; //Cái này là số trang trả về Hiện tại để là 0
            var input = string.Format("{{\"id\":\"{0}\",\"page\":\"{1}\"}}", id, page);
            try
            {
                //Thử xem có lấy đc JSON về ko, nếu ko thì bắn ra Lối kết nối / lỗi server
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderGetMyFarvoriteDriver, input);
                myFavorite = new RiderGetListFavoriteDriver();
                myFavorite = JsonConvert.DeserializeObject<RiderGetListFavoriteDriver>(output);


                if (myFavorite.status.Equals(ConstantVariable.responseCodeSuccess)) //0000 Code tra về ok
                {
                    //Nếu có thông tin Taxi farvorite
                    try
                    {
                        //1. Parse Object and Load to LLS
                        ObservableCollection<RiderListFavoriteDriverLLSObj> myFavoriteDataSource = new ObservableCollection<RiderListFavoriteDriverLLSObj>();
                        lls_MyFavoriteDriver.ItemsSource = myFavoriteDataSource;


                        //2. Loop to list all item in object
                        foreach (var favoriteItem in myFavorite.content.list)
                        {
                            myFavoriteDataSource.Add(new RiderListFavoriteDriverLLSObj(favoriteItem.fid, favoriteItem.fName, favoriteItem.lName, favoriteItem.mobile, favoriteItem.firm, favoriteItem.img));
                        }
                        //Tắt loading data
                        HideLoadingScreen();
                    }
                    catch (Exception)
                    {

                        MessageBox.Show("(Mã lỗi 1306) " + ConstantVariable.errHasErrInProcess);
                        Debug.WriteLine("Có lỗi 09dtgh ở Get Favorite Info");
                    }
                }
                else
                {
                    txt_MyTripStatus.Text = ConstantVariable.strNoTripInfo;
                    Debug.WriteLine("Có lỗi 4tge87t ở Get Favorite Info");
                }
            }
            catch (Exception)
            {

                MessageBox.Show("(Mã lỗi 1301) " + ConstantVariable.errConnectingError);
                Debug.WriteLine("Có lỗi 4dwdw ở Get Favorite Info");
            }
        }


        /// <summary>
        /// SỰ KIỆN NÀY ĐỂ GỌI XE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lls_MyFavoriteDriver_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ///1. Hiện Menu lựa chọn chức năng
            ///2. Truyền tham số vào
            ///3. ?

            //1. Show menu
            (this.Resources["showMenu"] as Storyboard).Begin();
            ShowBottomMenu();

            //2. Truyền tham số, đúng hơn là set tham số
            var selectedDriver = ((RiderListFavoriteDriverLLSObj)(sender as LongListSelector).SelectedItem);
            // If selected item is null, do nothing
            if (lls_MyFavoriteDriver.SelectedItem == null)
                return;
            driverName = selectedDriver.FullName;
            driverMobile = selectedDriver.Mobile;
            //driverId = selectedDriver.

            // Reset selected item to null
            lls_MyFavoriteDriver.SelectedItem = null;
        }


        /// <summary>
        /// HÀM NÀY ĐỂ XÓA 1 DRIVER RA KHỎI DANH SÁCH YÊU THÍCH
        /// </summary>
        private async void DeleteFovariteDriver(string did)
        {
            ShowButtonLoadingScreen();

            var uid = userData.content.uid;
            var rid = userData.content.rid;
            var input = string.Format("{{\"uid\":\"{0}\",\"rid\":\"{1}\",\"did\":\"{1}\"}}", uid, rid, did);
            try
            {
                //Thử xem có lấy đc JSON về ko, nếu ko thì bắn ra Lối kết nối / lỗi server
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetRiderDeleteMyFarvoriteDriver, input);
                BaseResponse deleteFavorite = new BaseResponse();
                deleteFavorite = JsonConvert.DeserializeObject<BaseResponse>(output);

                //Nếu như xóa thành công
                if (deleteFavorite.status.Equals(ConstantVariable.responseCodeSuccess)) //0000 Code tra về ok
                {
                    //Tắt loading ở menu
                    HideButtonLoadingScreen();

                    //Và load lại longlist                    
                    GetMyFarvoriteDriver();
                    
                }
                else
                {
                    txt_MyTripStatus.Text = ConstantVariable.errHasErrInProcess;
                    Debug.WriteLine("Có lỗi 43fdfd ở Delete Favorite Info");
                }
            }
            catch (Exception)
            {

                MessageBox.Show("(Mã lỗi 1307) " + ConstantVariable.errConnectingError);
                Debug.WriteLine("Có lỗi 43sdug ở Delete Favorite Info");
            }

        }


        private void btn_CallToDriver_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ///Sau khi chọn 1 item trong Longlist, hệ thống sẽ gán số điện thoại và tên cho 2 biến driverName và driverMobile
            ///Sau đó gọi điện qua hàm CallToNumber
            RiderFunctions.CallToNumber(driverName, driverMobile);
        }

        private void btn_Remove_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

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


    }

}