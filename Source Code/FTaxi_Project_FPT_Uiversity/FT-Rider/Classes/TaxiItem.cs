using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    public class Brand
    {
        public string id { get; set; }
        public string name { get; set; }
        public string phone { get; set; }
    }

    public class TaxiItem
    {
        public IList<Brand> brands { get; set; }
    }
}
