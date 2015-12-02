using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Driver.Classes
{
    class DriverNewtripNotification
    {
        public string tid { get; set; }
        //public string sAdd { get; set; }
        //public string eAdd { get; set; }
        public double sLat { get; set; }
        public double sLng { get; set; }
        public double eLat { get; set; }
        public double eLng { get; set; }
        public string rName { get; set; }
        public string mobile { get; set; }
        public long lmd { get; set; }
        public string notiType { get; set; }
    }
}
