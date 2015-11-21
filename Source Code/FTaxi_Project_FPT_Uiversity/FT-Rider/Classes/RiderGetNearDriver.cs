using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{

    public class ListDriverDTO
    {
        public String did { get; set; }
        public String fName { get; set; }
        public String lName { get; set; }
        public String cName { get; set; }
        public String mobile { get; set; }
        public String rate { get; set; }
        public double oPrice { get; set; }
        public double oKm { get; set; }
        public Double? f1Price { get; set; }
        public Double? f1Km { get; set; }
        public Double? f2Price { get; set; }
        public Double? f2Km { get; set; }
        public Double? f3Price { get; set; }
        public Double? f3Km { get; set; }
        public Double? f4Price { get; set; }
        public Double? f4Km { get; set; }
        public String img { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class RiderGetNearDriverContent
    {
        public IList<ListDriverDTO> listDriverDTO { get; set; }
    }

    public class RiderGetNearDriver
    {
        public int status { get; set; }
        public long lmd { get; set; }
        public RiderGetNearDriverContent content { get; set; }
    }
    
}

