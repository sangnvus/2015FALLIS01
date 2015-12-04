using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    public class List
    {
        public string fid { get; set; }
        public string fName { get; set; }
        public string lName { get; set; }
        public string mobile { get; set; }
        public string firm { get; set; }
        public Uri img { get; set; }
    }

    public class Content
    {
        public IList<List> list { get; set; }
        public int totalResult { get; set; }
    }

    public class RiderGetListFavoriteDriver
    {
        public string status { get; set; }
        public int lmd { get; set; }
        public Content content { get; set; }
    }
}
