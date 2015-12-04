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

namespace FT_Rider.Pages
{
    public partial class RiderMyFavoriteDriver : PhoneApplicationPage
    {
        //Cái này để lấy dữ liệu làm việc của User sau khi login
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        RiderLogin userData = null;
        RiderGetListFavoriteDriver myFavorite = null;

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

                        MessageBox.Show(ConstantVariable.errHasErrInProcess);
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
        /// SỰ KIỆN NÀY ĐỂ GỌI XE
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lls_MyFavoriteDriver_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedDriver = ((RiderListFavoriteDriverLLSObj)(sender as LongListSelector).SelectedItem);
            // If selected item is null, do nothing
            if (lls_MyFavoriteDriver.SelectedItem == null)
                return;

            //Nếu có dữ liệu thì thực hiện con này
            RiderFunctions.CallToNumber(selectedDriver.FullName, selectedDriver.Mobile);
        }

        
    }
    
}