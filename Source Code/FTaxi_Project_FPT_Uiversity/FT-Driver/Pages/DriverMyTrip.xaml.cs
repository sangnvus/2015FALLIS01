using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Animation;

namespace FT_Driver.Pages
{
    public partial class DriverMyTrip : PhoneApplicationPage
    {
        public DriverMyTrip()
        {
            InitializeComponent();
        }

        private void ShowDriverLostAssets()
        {
            (this.Resources["showDriverLostAssets"] as Storyboard).Begin();
            grv_DriverAssetsLost.Visibility = Visibility.Visible;
        }
        private void HideDriverLostAssets()
        {
            grv_DriverAssetsLost.Visibility = Visibility.Collapsed;
        }
        private void btn_ViewTripDetail_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void btn_AlertAssets_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void btn_Cancel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void btn_AddFavorite_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void btn_Close_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

        }

        private void lls_MyTrip_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void img_CloseDriverLostAsset_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            HideDriverLostAssets();
        }

        private void btn_SendLostAsset_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ShowDriverLostAssets();
        }
    }
}