using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class BaseResponse
    {
        public string status { get; set; }
        public long lmd { get; set; }
        public Object content { get; set; }
    }

}
