using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
     public class LatLngAddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public IList<string> types { get; set; }
    }

    public class LatLngNortheast
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class LatLngSouthwest
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class LatLngBounds
    {
        public LatLngNortheast northeast { get; set; }
        public LatLngSouthwest southwest { get; set; }
    }

    public class LatLngLocation
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class LatLngViewport
    {
        public LatLngNortheast northeast { get; set; }
        public LatLngSouthwest southwest { get; set; }
    }

    public class LatLngGeometry
    {
        public LatLngBounds bounds { get; set; }
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
    }

    public class LatLngResult
    {
        public IList<AddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public LatLngGeometry geometry { get; set; }
        public string place_id { get; set; }
        public IList<string> types { get; set; }
    }

    public class GoogleAPILatLngObj
    {
        public IList<LatLngResult> results { get; set; }
        public string status { get; set; }
    }

}
