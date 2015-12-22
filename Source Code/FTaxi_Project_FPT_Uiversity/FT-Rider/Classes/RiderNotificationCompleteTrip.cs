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
        public double eLat { get; set; }
        public double eLng { get; set; }
        public double fare { get; set; }
        public double dis { get; set; }
        public long lmd { get; set; }
        public string notiType { get; set; }
        public string tStatus { get; set; }
    }
}
