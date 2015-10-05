using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Data;
using System.Windows.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace TaxiApp
{
    public partial class RiderRegisterPayment : PhoneApplicationPage
    {
        String[] Month = { "Tháng", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
        String[] Year = { "Năm", "2015", "2016", "2017", "2018", "2019", "2020" };

        public RiderRegisterPayment()
        {
            InitializeComponent();

            this.lpk_Month.ItemsSource = Month;
            this.lpk_Year.ItemsSource = Year;
            
        }

     

        private void btn__Tap_Return(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.GoBack();
        }

        
    }
}