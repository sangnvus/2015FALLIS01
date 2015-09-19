using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Demo_App_15092015v4
{
    public partial class Maps : PhoneApplicationPage
    {
        public Maps()
        {
            InitializeComponent();
        }
    }

    public enum GoogleTileTypes
    {
        street
    }

    public class GoogleTile : Microsoft.Phone.Controls.Maps.TileSource
    {
        private GoogleTileTypes _tiletypes;

        public GoogleTileTypes TileTypes
        {
            get { return _tiletypes; }
            set { _tiletypes = value; }
        }

        public GoogleTile()
        {
            UriFormat = @"http://mt{0}.google.com/vt/lyrs={1}&z={2}&x={3}&y={4}";
        }

        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            if (zoomLevel > 0)
            {
                var Url = string.Format(UriFormat, 0, "m", zoomLevel, x, y);
                return new Uri(Url);
            }
            return null;
        }
    }
}