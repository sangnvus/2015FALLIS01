using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Demo_App_15092015v4.Views
{
    public partial class Demo_Call_WebService1 : PhoneApplicationPage
    {
        public Demo_Call_WebService1()
        {
            InitializeComponent();
        }

        private void CallServiceButton_Click(object sender, RoutedEventArgs e)
        {
            ServiceReference3.Service1Client callsv = new ServiceReference3.Service1Client();
            
        }

    }
}