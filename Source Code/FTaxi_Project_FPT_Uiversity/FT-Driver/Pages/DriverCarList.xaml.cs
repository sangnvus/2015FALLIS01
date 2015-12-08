using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Net.Http;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using FT_Driver.Classes;
using Microsoft.Devices;
using System.Threading;
using System.Diagnostics;


namespace FT_Driver.Pages
{
    public partial class DriverCarList : PhoneApplicationPage
    {
        //USER DATA
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        DriverLogin userData = new DriverLogin();
        string userId = "";
        IsolatedStorageSettings tNetAppSetting = IsolatedStorageSettings.ApplicationSettings;

        //Vibrate
        VibrateController vibrateController = VibrateController.Default;

        VehicleInfo myVehicle;
        string pushChannelURI = string.Empty;

        public DriverCarList()
        {
            InitializeComponent();
            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userData = (DriverLogin)tNetUserLoginData["UserLoginData"];
                userId = (string)tNetUserLoginData["UserId"];
            }


            if (tNetUserLoginData.Contains("PushChannelURI"))
            {
                pushChannelURI = (string)tNetUserLoginData["PushChannelURI"];
            }

            GetCarListToLLS();

        }

        private void GetCarListToLLS()
        {
            string carLelvel = "";
            Uri imgUrl = new Uri("Images/CarList/img_CarItemSAV.png", UriKind.Relative);
            ///{\"uid\":\"driver2@gmail.com\",\"pw\":\"b65bd772c3b0dfebf0a189efd420352d\",\"mid\":\"123\",\"mType\":\"iOS\"}
            try
            {
                //1. Parse Object and Load to LLS
                ObservableCollection<DriverVehiceInfoObj> carListDataSource = new ObservableCollection<DriverVehiceInfoObj>();
                lls_CarList.ItemsSource = carListDataSource;

                //2. Loop to list all item in object
                foreach (var obj in userData.content.vehicleInfos)
                {
                    switch (obj.carLevel)
                    {
                        case "ECO":
                            carLelvel = "Kinh tế";
                            imgUrl = new Uri("/Images/CarList/img_CarItemECO.png", UriKind.Relative);
                            break;
                        case "SAV":
                            carLelvel = "Tiết kiệm";
                            imgUrl = new Uri("/Images/CarList/img_CarItemSAV.png", UriKind.Relative);
                            break;
                        case "LUX":
                            imgUrl = new Uri("/Images/CarList/img_CarItemLUX.png", UriKind.Relative);
                            carLelvel = "Sang trọng";
                            break;
                    }

                    //3. Add to List
                    carListDataSource.Add(new DriverVehiceInfoObj(obj.plate, obj.carTitle, carLelvel, obj.vehicleId, imgUrl));
                }
            }
            catch (Exception)
            {

                MessageBox.Show(ConstantVariable.errHasErrInProcess);
            }
        }

        private void lls_CarList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            grv_ProcessScreen.Visibility = Visibility.Visible;
            tNetAppSetting["isSelectedCar"] = "SelectedCar"; //Cái này để đánh dấu rằng, sẽ bỏ qua bước chọn xe //Thêm vào sau tiến trình chọn xe

            var selectedCar = ((DriverVehiceInfoObj)(sender as LongListSelector).SelectedItem);
            // If selected item is null, do nothing
            if (lls_CarList.SelectedItem == null)
                return;


            //Else Update Vehicle Id to Server
            SelectVehicle(selectedCar.VehicleId);

            //vibrate phone
            TouchFeedback();
        }

        private async void SelectVehicle(int myVehicleId)
        {
            var did = userData.content.driverInfo.did.ToString();
            var uid = userId;
            var mid = pushChannelURI;
            var mType = ConstantVariable.mTypeWIN; //Chuyển did thành id
            var input = string.Format("{{\"id\":\"{0}\",\"uid\":\"{1}\",\"vehicleId\":\"{2}\",\"mid\":\"{3}\",\"mType\":\"{4}\"}}", did, uid, myVehicleId, mid, mType);     
       
            try
            {
                //Thử xem có lấy được thông tin về ko, nếu không thì do lỗi máy chủ
                var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverSelectVehicle, input);                
                var selectVehicle = JsonConvert.DeserializeObject<DriverLogin>(output);
                if (selectVehicle.status.Equals("0000")) //0000
                {
                    //Lấy thông tin  giá xe                        
                    foreach (var car in userData.content.vehicleInfos)
                    {
                        if (car.vehicleId == myVehicleId)
                        {
                            myVehicle = new VehicleInfo
                            {
                                status = car.status,
                                lmd = car.lmd,
                                content = car.content,
                                oPrice = car.oPrice,
                                oKm = car.oKm,
                                f1Price = car.f1Price,
                                f1Km = car.f1Km,
                                f2Price = car.f2Price,
                                f2Km = car.f2Km,
                                f3Price = car.f3Price,
                                f3Km = car.f3Km,
                                f4Price = car.f4Price,
                                f4Km = car.f4Km,
                                vehicleId = car.vehicleId,
                                plate = car.plate,
                                carTitle = car.carTitle,
                                carLevel = car.carLevel,
                                vRegDate = car.vRegDate,
                                manuYear = car.manuYear,
                                cap = car.cap
                            };

                            //
                            tNetUserLoginData["MySelectedVehicle"] = myVehicle;
                        }
                    }

                    NavigationService.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
                    Thread.Sleep(1000);
                    grv_ProcessScreen.Visibility = Visibility.Collapsed;//Disable Process bar
                }
                else
                {
                    MessageBox.Show("(Mã lỗi 1802) " + ConstantVariable.errServerError);
                    grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable Process bar
                    Debug.WriteLine("Mã lối ght67 ở SelectVehicle");
                }

            }
            catch (Exception)
            {

                //máy chủ lỗi
                MessageBox.Show("(Mã lỗi 1801) " + ConstantVariable.errServerError);
                grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable Process bar
                Debug.WriteLine("Mã lối 7hgtr3 ở SelectVehicle");
            }


        }



        private void TouchFeedback()
        {
            vibrateController.Start((TimeSpan.FromSeconds(0.1)));
        }
    }
}