using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using FT_Rider.Classes;

namespace FT_Rider.Pages
{
    public partial class TaxiList : PhoneApplicationPage
    {
        string listTaxi = "{\"brands\" : [{\"id\":\"1\",\"name\":\"Mỹ Đình\",\"phone\":\"0438333888\"},{\"id\":\"2\",\"name\":\"Taxi Group\",\"phone\":\"0438535353\"},{\"id\":\"3\",\"name\":\"Mai Linh\",\"phone\":\"0438222666\"},{\"id\":\"4\",\"name\":\"CP\",\"phone\":\"0438262626\"},{\"id\":\"5\",\"name\":\"Thăng Long\",\"phone\":\"0439717171\"},{\"id\":\"6\",\"name\":\"Airport\",\"phone\":\"0438733333\"},{\"id\":\"7\",\"name\":\"Nội bài\",\"phone\":\"0438868888\"},{\"id\":\"8\",\"name\":\"Thanh Nga\",\"phone\":\"0438215215\"},{\"id\":\"9\",\"name\":\"VIC\",\"phone\":\"0438434343\"},{\"id\":\"10\",\"name\":\"Thủ đô\",\"phone\":\"0438333333\"},{\"id\":\"11\",\"name\":\"Ba Sao\",\"phone\":\"0432202020\"},{\"id\":\"12\",\"name\":\"ABC\",\"phone\":\"0437191919\"},{\"id\":\"13\",\"name\":\"Vạn Xuân\",\"phone\":\"0438222888\"},{\"id\":\"14\",\"name\":\"Sao Việt\",\"phone\":\"0432626262\"},{\"id\":\"15\",\"name\":\"Thu Hương\",\"phone\":\"0438363636\"},{\"id\":\"16\",\"name\":\"Hương Lúa\",\"phone\":\"0438252525\"},{\"id\":\"17\",\"name\":\"Thế Kỷ Mới\",\"phone\":\"0438734734\"},{\"id\":\"18\",\"name\":\"Đông Đô\",\"phone\":\"0438574574\"},{\"id\":\"19\",\"name\":\"Việt Hương\",\"phone\":\"0438282828\"},{\"id\":\"20\",\"name\":\"Thăng Long\",\"phone\":\"0439717171\"},{\"id\":\"21\",\"name\":\"Phú Hưng\",\"phone\":\"0432262626\"},{\"id\":\"22\",\"name\":\"Phú Gia\",\"phone\":\"0438767676\"},{\"id\":\"23\",\"name\":\"Thành Lợi\",\"phone\":\"0438551551\"},{\"id\":\"24\",\"name\":\"Thành Công\",\"phone\":\"0432575757\"}]}";
        TaxiItem taxiList = new TaxiItem();
        public TaxiList()
        {
            InitializeComponent();
            taxiList = JsonConvert.DeserializeObject<TaxiItem>(listTaxi);                
            LoadTaxiList("");
        }

        private void LoadTaxiList(string str)
        {
            ObservableCollection<TaxiItemObj> taxiListDataSource = new ObservableCollection<TaxiItemObj>();
            if (str.Equals(""))
            {
                foreach (var item in taxiList.brands)
                {
                    taxiListDataSource.Add(new TaxiItemObj(item.id, item.name, item.phone));
                }
            }
            else
            {
                foreach (var item in taxiList.brands)
                {
                    if (ConvertData.ConvertVietnamCharacter(item.name.ToLower()).Contains(str.ToLower()))
                    {
                        taxiListDataSource.Add(new TaxiItemObj(item.id, item.name, item.phone));
                    }
                }
            }

            lls_TaxiList.ItemsSource = taxiListDataSource;
        }

        private void lls_TaxiList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedTaxiCenter = ((TaxiItemObj)(sender as LongListSelector).SelectedItem);
            // If selected item is null, do nothing
            if (lls_TaxiList.SelectedItem == null)
                return;

            RiderFunctions.CallToNumber(selectedTaxiCenter.Name, selectedTaxiCenter.Phone);
        }

        private void txt_SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txt_SearchBox.Text.Equals("Tìm kiếm"))
            {
                txt_SearchBox.Text = string.Empty;
            }

        }

        private void txt_SearchBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            LoadTaxiList(txt_SearchBox.Text);
        }


    }
}