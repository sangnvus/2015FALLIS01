using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Driver.Classes
{
    class DriverCompleteTrip
    {
        public string uid { get; set; }
        public string pw { get; set; }
        public string tid { get; set; }
        public string eAdd { get; set; }
        public string eCityName { set; get; }
        public double eLat { get; set; }
        public double eLng { get; set; }
        public double dis { get; set; }
        public double fare { get; set; }
        public long lmd { get; set; }
        public int eCityId { get; set; }       

    }
}
