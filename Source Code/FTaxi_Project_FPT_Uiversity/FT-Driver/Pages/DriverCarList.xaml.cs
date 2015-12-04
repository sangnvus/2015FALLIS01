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


        public DriverCarList()
        {
            InitializeComponent();
            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userData = (DriverLogin)tNetUserLoginData["UserLoginData"];
                userId = (string)tNetUserLoginData["UserId"];
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

            var input = string.Format("{{\"did\":\"{0}\",\"uid\":\"{1}\",\"vehicleId\":\"{2}\"}}", did, uid, myVehicleId);
            var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverSelectVehicle, input);
            if (output != null)
            {
                try
                {

                    var selectVehicle = JsonConvert.DeserializeObject<DriverLogin>(output);
                    if (selectVehicle != null)
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
                        MessageBox.Show(ConstantVariable.errServerError);
                        grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable Process bar
                    }

                }
                catch (Exception)
                {

                    //Login Failed | Đăng nhập không thành công
                    MessageBox.Show(ConstantVariable.errServerError);
                    grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable Process bar
                }
            }
            else
            {
                //Có lỗi kết nối với Server
                MessageBox.Show(ConstantVariable.errConnectingError);
                grv_ProcessScreen.Visibility = Visibility.Collapsed; //Disable Process bar
            }

        }



        private void TouchFeedback()
        {
            vibrateController.Start((TimeSpan.FromSeconds(0.1)));
        }
    }
}