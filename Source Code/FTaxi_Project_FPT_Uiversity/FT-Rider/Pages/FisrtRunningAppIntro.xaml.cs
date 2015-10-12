using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Input;

namespace FT_Rider.Pages
{
    public partial class FisrtRunningAppIntro : PhoneApplicationPage
    {
        

        public FisrtRunningAppIntro()
        {
            InitializeComponent();

            //TUT page
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 46, 159, 255);

            if(pvi_Tut1.IsEnabled)
            {
                el_Page1.Fill = mySolidColorBrush;
                el_Page2.Fill = el_Page3.Fill = el_Page4.Fill = el_Page5.Fill = new SolidColorBrush(Colors.White);
            }
            if (pvi_Tut2.IsEnabled)
            {
                el_Page2.Fill = mySolidColorBrush;
                el_Page1.Fill = el_Page3.Fill = el_Page4.Fill = el_Page5.Fill = new SolidColorBrush(Colors.White);
            }
            if (pvi_Tut3.IsEnabled)
            {
                el_Page3.Fill = mySolidColorBrush;
                el_Page1.Fill = el_Page2.Fill = el_Page4.Fill = el_Page5.Fill = new SolidColorBrush(Colors.White);
            }
            if (pvi_Tut4.IsEnabled)
            {
                el_Page4.Fill = mySolidColorBrush;
                el_Page1.Fill = el_Page2.Fill = el_Page3.Fill = el_Page5.Fill = new SolidColorBrush(Colors.White);
            }
            if (pvi_Tut5.IsEnabled)
            {
                el_Page5.Fill = mySolidColorBrush;
                el_Page1.Fill = el_Page2.Fill = el_Page3.Fill = el_Page4.Fill = new SolidColorBrush(Colors.White);
            }
        }

        private void btn_Go_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Login.xaml", UriKind.Relative));
        }
    }
}