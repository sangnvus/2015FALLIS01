using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FT_Driver.Classes;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace FT_Driver.Pages
{
    public partial class DriverCarList : PhoneApplicationPage
    {
        //USER DATA
        DriverLogin userData = PhoneApplicationService.Current.State["UserInfo"] as DriverLogin;
        string userId = PhoneApplicationService.Current.State["UserId"] as string;
        string pwmd5 = PhoneApplicationService.Current.State["PasswordMd5"] as string;

        public DriverCarList()
        {
            InitializeComponent();
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

                //3. Loop to list all item in object
                foreach (var obj in userData.content.vehicleInfos)
                {
                    carListDataSource.Add(new DriverVehiceInfoObj(obj.plate, obj.carTitle, obj.carLevel));
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