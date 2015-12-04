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

namespace FT_Rider.Pages
{
    public partial class RiderMyTrip : PhoneApplicationPage
    {
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        RiderLogin userData;
        string password = string.Empty;
        RiderGetMyTrip myTrip = null;

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

        /// <summary>
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
                

                if (myTrip.status.Equals(ConstantVariable.responseCodeSuccess)) //0000
                {
                    //Nếu có thông tin chuyến đi thì bắt đầu lấy về
                    try
                    {
                        //1. Parse Object and Load to LLS
                        ObservableCollection<MyTripObj> myTripDataSource = new ObservableCollection<MyTripObj>();
                        lls_MyTrip.ItemsSource = myTripDataSource;


                        //2. Loop to list all item in object
                        foreach (var trip in myTrip.content.list)
                        {
                            //switch (trip.carLevel)
                            //{
                            //    case "ECO":
                            //        carLelvel = "Kinh tế";
                            //        imgUrl = new Uri("/Images/CarList/img_CarItemECO.png", UriKind.Relative);
                            //        break;
                            //    case "SAV":
                            //        carLelvel = "Tiết kiệm";
                            //        imgUrl = new Uri("/Images/CarList/img_CarItemSAV.png", UriKind.Relative);
                            //        break;
                            //    case "LUX":
                            //        imgUrl = new Uri("/Images/CarList/img_CarItemLUX.png", UriKind.Relative);
                            //        carLelvel = "Sang trọng";
                            //        break;
                            //}
                            myTripDataSource.Add(new MyTripObj(trip.tid, trip.did, trip.dName, trip.dMobile, trip.plate, trip.cLvl, trip.from, trip.to, trip.sTime, trip.eTime, trip.distance, trip.fare, trip.payment, trip.currency, trip.fName, trip.lName, trip.rate, trip.interCode, trip.favorite));
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
    }
}