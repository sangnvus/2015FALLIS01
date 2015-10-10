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
using System.Windows.Media;

namespace FT_Rider.Pages
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

        private void txt_CardNumber_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            txt_CardNumber.Text = String.Empty;
            txt_CardNumber.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void txt_Postal_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            txt_Postal.Text = String.Empty;
            txt_Postal.Foreground = new SolidColorBrush(Colors.Black);
        }

     

     

       
        
    }
}