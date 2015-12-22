using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Driver.Classes
{
    public class DriverGetMyTripList
    {
        public string tid { get; set; }
        public string rid { get; set; }
        public string sAdd { get; set; }
        public string eAdd { get; set; }
        public string sTime { get; set; }
        public string eTime { get; set; }
        public string rName { get; set; }
        public string mobile { get; set; }
        public bool vip { get; set; }
        public double fare { get; set; }
        public double distance { get; set; }
        public string payment { get; set; }
        public string currency { get; set; }
        public Double? rate { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public string img { get; set; }
    }

    public class DriverGetMyTripContent
    {
        public IList<DriverGetMyTripList> list { get; set; }
        public int totalResult { get; set; }
    }

    public class DriverGetMyTrip
    {
        public string status { get; set; }
        public long lmd { get; set; }
        public DriverGetMyTripContent content { get; set; }
    }

}
