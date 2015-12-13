using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Driver.Classes
{
    public class DriverGetCityList
    {
        public int cityId { get; set; }
        public string lan { get; set; }
        public string cityName { get; set; }
        public string googleName { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class DriverGetCityNamesContent
    {
        public IList<DriverGetCityList> list { get; set; }
        public int totalResult { get; set; }
    }

    public class DriverGetCityNames
    {
        public string status { get; set; }
        public long lmd { get; set; }
        public DriverGetCityNamesContent content { get; set; }
    }
}
