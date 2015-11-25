using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class RiderCreateTrip
    {
        public String uid { get; set; }
        public String rid { get; set; }
        public List<String> did { get; set; }
        public String sAddr { get; set; }
        public String eAddr { get; set; }
        public Double? sLat { get; set; }
        public Double? sLng { get; set; }
        public Double? eLat { get; set; }
        public Double? eLng { get; set; }
        public Double? sCity { get; set; }
        public Double? eCity { get; set; }
        public String sCityName { get; set; }
        public String eCityName { get; set; }
        public String cntry { get; set; }
        public String proCode { get; set; }
        public String rType { get; set; } // request type : 1 - many, many
    }
}
