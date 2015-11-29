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


namespace FT_Driver.Pages
{
    public partial class DriverCarList : PhoneApplicationPage
    {
        //USER DATA
        //DriverLogin userData = PhoneApplicationService.Current.State["UserInfo"] as DriverLogin;

        //string userId = PhoneApplicationService.Current.State["UserId"] as string;
        //string pwmd5 = PhoneApplicationService.Current.State["PasswordMd5"] as string;
        IsolatedStorageSettings tNetUserLoginData = IsolatedStorageSettings.ApplicationSettings;
        DriverLogin userData = new DriverLogin();



        public DriverCarList()
        {
            InitializeComponent();
            if (tNetUserLoginData.Contains("UserLoginData"))
            {
                userData = (DriverLogin)tNetUserLoginData["UserLoginData"];
            }


            this.GetCarListToLLS();

        }

        private void GetCarListToLLS()
        {

            ///{\"uid\":\"driver2@gmail.com\",\"pw\":\"b65bd772c3b0dfebf0a189efd420352d\",\"mid\":\"123\",\"mType\":\"iOS\"}
            try
            {
                //2. Parse Object and Load to LLS
                ObservableCollection<DriverVehiceInfoObj> carListDataSource = new ObservableCollection<DriverVehiceInfoObj>();
                lls_CarList.ItemsSource = carListDataSource;
                string carLelvel = "";
                Uri imgUrl =  new Uri("Images/CarList/img_CarItemSAV.png", UriKind.Relative);;
                //3. Loop to list all item in object
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

                    carListDataSource.Add(new DriverVehiceInfoObj(obj.plate, obj.carTitle, carLelvel, imgUrl));
                }
            }
            catch (Exception)
            {

                MessageBox.Show(ConstantVariable.errHasErrInProcess);
            }
        }

        private void lls_CarList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}