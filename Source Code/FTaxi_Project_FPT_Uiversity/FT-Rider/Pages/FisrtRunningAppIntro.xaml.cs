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
        //varible
        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
        PivotItem pivot = null;


        public FisrtRunningAppIntro()
        {
            InitializeComponent();
        }

        private void btn_Go_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Login.xaml", UriKind.Relative));
        }

        private void pvi_Tut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //TUT page            
            mySolidColorBrush.Color = Color.FromArgb(255, 46, 159, 255);
            pivot = (PivotItem)(sender as Pivot).SelectedItem;

            switch (pivot.Name.ToString())
            {
                case "pvi_Tut1":
                    el_Page1.Fill = mySolidColorBrush;
                    el_Page2.Fill = el_Page3.Fill = el_Page4.Fill = el_Page5.Fill = new SolidColorBrush(Colors.White);
                    break;
                case "pvi_Tut2":
                    el_Page2.Fill = mySolidColorBrush;
                    el_Page1.Fill = el_Page3.Fill = el_Page4.Fill = el_Page5.Fill = new SolidColorBrush(Colors.White);
                    break;
                case "pvi_Tut3":
                    el_Page3.Fill = mySolidColorBrush;
                    el_Page1.Fill = el_Page2.Fill = el_Page4.Fill = el_Page5.Fill = new SolidColorBrush(Colors.White);
                    break;
                case "pvi_Tut4":
                    el_Page4.Fill = mySolidColorBrush;
                    el_Page1.Fill = el_Page2.Fill = el_Page3.Fill = el_Page5.Fill = new SolidColorBrush(Colors.White);
                    break;
                case "pvi_Tut5":
                    el_Page5.Fill = mySolidColorBrush;
                    el_Page1.Fill = el_Page2.Fill = el_Page3.Fill = el_Page4.Fill = new SolidColorBrush(Colors.White);
                    break;

            }
        }

    }
}