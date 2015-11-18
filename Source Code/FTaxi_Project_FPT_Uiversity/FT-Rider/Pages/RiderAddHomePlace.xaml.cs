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
using FT_Rider.Classes;
using FT_Rider.Resources;

namespace FT_Rider.Pages
{
    public partial class RiderAddHomePlace : PhoneApplicationPage
    {
        public RiderAddHomePlace()
        {
            InitializeComponent();

            this.LoadLocationOnMap();
        }

        private void txt_Address_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            txt_Address.Text = String.Empty;
            txt_Address.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void txt_State_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            txt_State.Text = String.Empty;
            txt_State.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void txt_City_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            txt_City.Text = String.Empty;
            txt_City.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void LoadLocationOnMap()
        {
            MapShowMarker myMarker = new MapShowMarker();
            myMarker.ShowPointOnMap(21.038472, 105.8014108,map_RiderHome);
        }
        
    }
}