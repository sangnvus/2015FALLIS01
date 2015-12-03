using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    public class RiderMyTripList
    {
        public string tid { get; set; }
        public string did { get; set; }
        public string dName { get; set; }
        public string dMobile { get; set; }
        public string plate { get; set; }
        public string cLvl { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public string sTime { get; set; }
        public string eTime { get; set; }
        public Double? distance { get; set; }
        public Double? fare { get; set; }
        public Double? payment { get; set; }
        public string currency { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public string img { get; set; }
        public Double? rate { get; set; }
        public string interCode { get; set; }
        public bool favorite { get; set; }
    }

    public class RiderMyTripContent
    {
        public IList<RiderMyTripList> list { get; set; }
        public int totalResult { get; set; }
    }

    public class RiderMyTrip
    {
        public string status { get; set; }
        public long lmd { get; set; }
        public RiderMyTripContent content { get; set; }
    }

}
