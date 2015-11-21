using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class RiderCreateTrip
    {
        private String uid;
        private String rid;
        private List<String> did;
        private String sAddr;
        private String eAddr;
        private Double? sLat;
        private Double? sLng;
        private Double? eLat;
        private Double? eLng;
        private Double? sCity;
        private Double? eCity;
        private String sCityName;
        private String eCityName;
        private String cntry;
        private String proCode;
        private String rType; // request type : 1 - many, many
    }
}
