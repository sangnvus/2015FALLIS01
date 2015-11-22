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
        public DriverCarList()
        {
            InitializeComponent();
            this.GetCarListToLLS();
        }

        private async void GetCarListToLLS()
        {

            //Get JSON string when login
            DriverLoginResponse driverLogin = new DriverLoginResponse();
            string driverLoginUrl = ConstantVariable.tNetDriverLoginAddress;
            string driverLoginData = "{\"uid\":\"driver2@gmail.com\",\"pw\":\"b65bd772c3b0dfebf0a189efd420352d\",\"mid\":\"123\",\"mType\":\"iOS\"}";
            string driverLoginJsonReturn = await GetJsonFromPOSTMethod.GetJsonString(driverLoginUrl, driverLoginData);

            try
            {
                //2. Parse Object and Load to LLS
                driverLogin = JsonConvert.DeserializeObject<DriverLoginResponse>(driverLoginJsonReturn);
                ObservableCollection<DriverVehiceInfoObj> carListDataSource = new ObservableCollection<DriverVehiceInfoObj>();
                lls_CarList.ItemsSource = carListDataSource;

                //3. Loop to list all item in object
                foreach (var obj in driverLogin.content.vehicleInfos)
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