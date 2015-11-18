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
using Newtonsoft.Json;

namespace FT_Rider.Pages
{
    public partial class RiderAddHomePlace : PhoneApplicationPage
    {
        public RiderAddHomePlace()
        {
            InitializeComponent();

            this.LoadLocationOnMap(21.038472, 105.8014108);
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

        private async void LoadLocationOnMap(double lat, double lng)
        {
            //Show marker
            MapShowMarker myMarker = new MapShowMarker();
            myMarker.ShowPointOnMap(lat, lng ,map_RiderHome);

            //Convert Lat & Lng to Address String
            string jsonString = await GoogleAPIFunction.ConvertLatLngToAddress(lat, lng); //Get Json String
            GoogleAPILatLngObj address = new GoogleAPILatLngObj(); 
            address = JsonConvert.DeserializeObject<GoogleAPILatLngObj>(jsonString); //Convert to Obj

            //Load address to screen
            txt_Address.Text = address.results[0].address_components[0].long_name.ToString();
            txt_State.Text = address.results[0].address_components[1].long_name.ToString();
            txt_City.Text = address.results[0].address_components[2].long_name.ToString();
        }
        
    }
}