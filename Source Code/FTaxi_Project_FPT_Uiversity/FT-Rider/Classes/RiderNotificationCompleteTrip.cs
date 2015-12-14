using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class RiderNotificationCompleteTrip
    {
        public string tid { get; set;}
        public string eAdd { get; set; }
        public Double? eLat { get; set; }
        public Double? eLng { get; set; }
        public Double? fare { get; set; }
        public Double? dis { get; set; }
        public long lmd { get; set; }
        public string notiType { get; set; }
        public string tStatus { get; set; }
    }
}
