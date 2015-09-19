using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace updatemaps
{
    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
    }

    public class Photo
    {
        public int height { get; set; }
        public IList<string> html_attributions { get; set; }
        public string photo_reference { get; set; }
        public int width { get; set; }
    }

    public class OpeningHours
    {
        public bool open_now { get; set; }
        public IList<object> weekday_text { get; set; }
    }

    public class Result
    {
        public Geometry geometry { get; set; }
        public string icon { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string place_id { get; set; }
        public string reference { get; set; }
        public string scope { get; set; }
        public IList<string> types { get; set; }
        public string vicinity { get; set; }
        public IList<Photo> photos { get; set; }
        public OpeningHours opening_hours { get; set; }
    }

    public class JSON_NearbySearch
    {
        public IList<string> html_attributions { get; set; }
        public IList<Result> results { get; set; }
        public string status { get; set; }


    }
}
