using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class RiderUpdateAddressRequest
    {
        private String id { get; set; }
        private String nCity { get; set; }
        private String nAdd { get; set; }
        private Double? lat { get; set; }
        private Double? lng { get; set; }
        private String addrtype { get; set; }
        private long lmd { get; set; }
        private String role { get; set; }
        private int cityId { get; set; }
        private String cntry { get; set; }
        private String uid { get; set; }

        //addrType ở đây là "home" và "office"
        //role gồm có DR và RD
        //DR là driver
        //RD là rider
    }
}
