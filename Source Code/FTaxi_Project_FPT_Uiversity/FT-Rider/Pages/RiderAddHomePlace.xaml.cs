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
using System.Device.Location;
using System.Collections.ObjectModel;
using Microsoft.Devices;

namespace FT_Rider.Pages
{
    public partial class RiderAddHomePlace : PhoneApplicationPage
    {
        VibrateController vibrateController = VibrateController.Default;
        public RiderAddHomePlace()
        {
            InitializeComponent();

            //Input lat & lng Parameter in this function to load address
            this.LoadLocationOnMap(21.038472, 105.8014108);



            //Set status of control
            //this.lls_AutoComplete.IsEnabled = false;
            //txt_City.IsEnabled = false;
            isLlsOff();
            this.img_ClearText.Visibility = Visibility.Collapsed;

        }

        private void isLlsOff()
        {
            this.lls_AutoComplete.Visibility = Visibility.Collapsed;
            this.lls_AutoComplete.IsEnabled = false;
        }

        private void isLlsOn()
        {
            this.lls_AutoComplete.Visibility = Visibility.Visible;
            this.lls_AutoComplete.IsEnabled = true;
        }


        private async void LoadLocationOnMap(double lat, double lng)
        {
            //Show marker
            MapShowMarker myMarker = new MapShowMarker();
            myMarker.ShowPointOnMap(lat, lng, map_RiderHome, 16);

            //Convert Lat & Lng to Address String
            string jsonString = await GoogleAPIFunction.ConvertLatLngToAddress(lat, lng); //Get Json String
            GoogleAPILatLngObj address = new GoogleAPILatLngObj();
            address = JsonConvert.DeserializeObject<GoogleAPILatLngObj>(jsonString); //Convert to Obj

            //Load address to screen
            txt_Address.Text = address.results[0].address_components[0].long_name.ToString() + ", " + address.results[0].address_components[1].long_name.ToString();
            txt_City.Text = address.results[0].address_components[3].long_name.ToString();

        }

        private void txt_Address_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            //Enable LLS
            isLlsOn();
            //Enable 
            img_ClearText.Visibility = Visibility.Visible;

        }

        private void img_ClearText_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (txt_Address.Text == null)
            {
                isLlsOff();
                img_ClearText.Visibility = Visibility.Collapsed;
            }
            else
            {
                txt_Address.Text = string.Empty;
                txt_Address.Focus();
            }
        }

        private void txt_Address_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string inputAddress = txt_Address.Text;
            AutoCompletePlace(inputAddress);
        }

        private async void AutoCompletePlace(string inputAddress)
        {
            GoogleAPIQueryAutoCompleteObj placesObj = new GoogleAPIQueryAutoCompleteObj();

            try
            {
                placesObj = await GoogleAPIFunction.ConvertAutoCompleteToLLS(inputAddress);

                //2. Create Place list
                ObservableCollection<AutoCompletePlace> autoCompleteDataSource = new ObservableCollection<AutoCompletePlace>();
                lls_AutoComplete.ItemsSource = autoCompleteDataSource;
                //3. Loop to list all item in object
                foreach (var obj in placesObj.predictions)
                {
                    autoCompleteDataSource.Add(new AutoCompletePlace(obj.description.ToString()));
                }
            }
            catch (Exception)
            {

                txt_Address.Focus();
            }
        }

        private async void lls_AutoComplete_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedPlace = ((AutoCompletePlace)(sender as LongListSelector).SelectedItem);
            // If selected item is null, do nothing
            if (lls_AutoComplete.SelectedItem == null)
            {
                return;
            }

            //show address on textbox
            GoogleAPIAddressObj address = new GoogleAPIAddressObj();
            address = await GoogleAPIFunction.ConvertAddressToLatLng(selectedPlace.Name.ToString());
            txt_Address.Text = address.results[0].address_components[0].long_name.ToString()
                                + ", " + address.results[0].address_components[1].long_name.ToString()
                                + ", " + address.results[0].address_components[2].long_name.ToString();
            txt_City.Text = address.results[0].address_components[address.results[0].address_components.Count - 2].long_name.ToString();

            //Return Lat, Lng, some paramenter here
            //Return Lat, Lng, some paramenter here
            //Return Lat, Lng, some paramenter here

            setCursorAtLast(txt_Address);

            //vibrate phone
            vibrateController.Start(TimeSpan.FromSeconds(0.1));

            isLlsOff();
        }

        private void setCursorAtLast(TextBox txtBox)
        {
            txtBox.SelectionStart = txtBox.Text.Length; // add some logic if length is 0
            txtBox.SelectionLength = 0;
        }

    }
}