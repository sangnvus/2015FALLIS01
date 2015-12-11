using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    public class DriverStatusList
    {
        public string did { get; set; }
        public string status { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class RiderUpdateDriverStatusObjContent
    {
        public IList<DriverStatusList> driverStatusList { get; set; }
    }

    public class RiderUpdateDriverStatusObj
    {
        public string status { get; set; }
        public long lmd { get; set; }
        public RiderUpdateDriverStatusObjContent content { get; set; }
    }

}
