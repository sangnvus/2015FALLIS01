using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace FT_Driver.Pages
{
    public partial class HomePayment : PhoneApplicationPage
    {
        public HomePayment()
        {
            InitializeComponent();
        }

        

        private void btn_Payment_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            MessageBox.Show("Thanh toán thành công!");
        }
    }
}