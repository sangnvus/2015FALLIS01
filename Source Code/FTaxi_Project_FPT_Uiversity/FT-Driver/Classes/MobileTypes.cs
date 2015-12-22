using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Driver.Classes
{
    class MobileTypes
    {
        public enum Type
        {
            IOS,
            WIN,
            AND
        }
        public Type TaxiType { get; set; }
    }
}
