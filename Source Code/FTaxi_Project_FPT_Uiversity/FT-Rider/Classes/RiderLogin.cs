using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    public class Pmt
    {
        public object pid { get; set; }
        public string cNO { get; set; }
        public string cvv { get; set; }
        public string mon { get; set; }
        public string year { get; set; }
        public object postal { get; set; }
        public string cate { get; set; }
    }

    public class RiderLoginContent
    {
        public string fName { get; set; }
        public string lName { get; set; }
        public object interCode { get; set; }
        public string img { get; set; }
        public string mobile { get; set; }
        public string hAdd { get; set; }
        public string lan { get; set; }
        public object cntry { get; set; }
        public string email { get; set; }
        public string rid { get; set; }
        public object name { get; set; }
        public string oAdd { get; set; }
        public double hAddLat { get; set; }
        public double hAddLng { get; set; }
        public double oAddLat { get; set; }
        public double oAddLng { get; set; }
        public IList<Pmt> pmt { get; set; }
        public long olmd { get; set; }
        public int nlmd { get; set; }
        public string status { get; set; }
        public string uid { get; set; }
        public object pw { get; set; }
    }

    public class RiderLogin
    {
        public string status { get; set; }
        public int lmd { get; set; }
        public RiderGetNearDriverContent content { get; set; }
    }
}
