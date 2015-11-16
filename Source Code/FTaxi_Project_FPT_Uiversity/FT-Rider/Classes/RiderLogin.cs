using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    public class Pmt
    {
        public string pid { get; set; }
        public string cNO { get; set; }
        public string cvv { get; set; }
        public string mon { get; set; }
        public string year { get; set; }
        public string postal { get; set; }
        public string cate { get; set; }
    }

    public class RiderLoginContent
    {
        public string fName { get; set; }
        public string lName { get; set; }
        public string interCode { get; set; }
        public string img { get; set; }
        public string mobile { get; set; }
        public string hAdd { get; set; }
        public string lan { get; set; }
        public string cntry { get; set; }
        public string email { get; set; }
        public string rid { get; set; }
        public string name { get; set; }
        public string oAdd { get; set; }
        public Double? hAddLat { get; set; }
        public Double? hAddLng { get; set; }
        public Double? oAddLat { get; set; }
        public Double? oAddLng { get; set; }
        public IList<Pmt> pmt { get; set; }
        public long olmd { get; set; }
        public int nlmd { get; set; }
        public string status { get; set; }
        public string uid { get; set; }
        public string pw { get; set; }
    }

    public class RiderLogin
    {
        public int status { get; set; }
        public long lmd { get; set; }
        public RiderLoginContent content { get; set; }
    }

}
