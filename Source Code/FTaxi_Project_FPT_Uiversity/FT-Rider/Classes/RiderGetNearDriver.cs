using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{

    public class ListDriverDTO
    {
        public string did { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public string cName { get; set; }
        public string mobile { get; set; }
        public int rate { get; set; }
        public double oPrice { get; set; }
        public double oKm { get; set; }
        public Double f1Price { get; set; }
        public Double f1Km { get; set; }
        public Double f2Price { get; set; }
        public Double f2Km { get; set; }
        public Double f3Price { get; set; }
        public Double f3Km { get; set; }
        public Double f4Price { get; set; }
        public Double f4Km { get; set; }
        public string img { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class RiderGetNearDriverContent
    {
        public IList<ListDriverDTO> listDriverDTO { get; set; }
    }

    public class RiderGetNearDriver
    {
        public string status { get; set; }
        public int lmd { get; set; }
        public RiderGetNearDriver content { get; set; }
    }

}

