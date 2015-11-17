using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    public class TaxiTypes
    {
        public enum Type
        {
            ECO,
            SAV,
            LUX
        }
        public Type TaxiType { get; set; }
    }

  
}
