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
        public object rate { get; set; }
        public int oPrice { get; set; }
        public double oKm { get; set; }
        public int f1Price { get; set; }
        public int f1Km { get; set; }
        public int f2Price { get; set; }
        public int f2Km { get; set; }
        public object f3Price { get; set; }
        public object f3Km { get; set; }
        public object f4Price { get; set; }
        public object f4Km { get; set; }
        public string img { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Content
    {
        public IList<ListDriverDTO> listDriverDTO { get; set; }
    }

    public class RiderGetNearCar
    {
        public string status { get; set; }
        public int lmd { get; set; }
        public Content content { get; set; }
    }
}
