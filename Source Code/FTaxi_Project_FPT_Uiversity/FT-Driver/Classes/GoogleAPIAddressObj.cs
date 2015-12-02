using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Driver.Classes
{
    public class AOJAddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public IList<string> types { get; set; }
    }

    public class AOJLocation
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class AOJNortheast
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class AOJSouthwest
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class AOJViewport
    {
        public AOJNortheast northeast { get; set; }
        public AOJSouthwest southwest { get; set; }
    }



    public class AOJGeometry
    {
        public AOJLocation location { get; set; }
        public string location_type { get; set; }
        public AOJViewport viewport { get; set; }
    }

    public class AOJResult
    {
        public IList<AOJAddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public AOJGeometry geometry { get; set; }
        public string place_id { get; set; }
        public IList<string> types { get; set; }
    }

    public class AOJGoogleAPIAddressObj
    {
        public IList<AOJResult> results { get; set; }
        public string status { get; set; }
    }
}
