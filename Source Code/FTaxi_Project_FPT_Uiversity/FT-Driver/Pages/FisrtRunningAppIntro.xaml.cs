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
using System.IO.IsolatedStorage;

namespace FT_Driver.Pages
{
    public partial class FisrtRunningAppIntro : PhoneApplicationPage
    {
        //varible
        SolidColorBrush mySolidColorBrush = new SolidColorBrush();
        PivotItem pivot = null;
        IsolatedStorageSettings tNetAppSetting = IsolatedStorageSettings.ApplicationSettings;


        public FisrtRunningAppIntro()
        {
            InitializeComponent();
        }

        private void btn_Go_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ///Nếu người dùng login thành công và chọn xe thành công thì sẽ vào thằng màn hình Home
            ///Nếu người dùng chỉ mới login nhưng sau đó không chọn xe mà loại thoát app thì lần sau vẫn vào màn hình chọn xe
            ///Nếu người dùng chưa login thành công thì lần truy cập triếp theo vẫn vào màn hình Login
            if (tNetAppSetting.Contains("isLogin") && tNetAppSetting.Contains("isSelectedCar")) //Check if user was logined and Selected Car
            {
                NavigationService.Navigate(new Uri("/Pages/Page2.xaml", UriKind.Relative));
            }
            else if (tNetAppSetting.Contains("isSelectedCar"))
            {
                NavigationService.Navigate(new Uri("/Pages/DriverCarList.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService.Navigate(new Uri("/Pages/Login.xaml", UriKind.Relative));
            }
           
        }

        /// Cái này để thay đổi nút số trang ở màn hình First Start App
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