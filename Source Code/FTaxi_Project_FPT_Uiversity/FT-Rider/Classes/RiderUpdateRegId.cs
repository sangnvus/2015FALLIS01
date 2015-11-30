using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class RiderUpdateRegId
    {
        private String _mid;
        private String _mType;
        private String _role;
        private String _id;

        public String Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public String Role
        {
            get { return _role; }
            set { _role = value; }
        }
        
        public String MType
        {
            get { return _mType; }
            set { _mType = value; }
        }
        
        public String Mid
        {
            get { return _mid; }
            set { _mid = value; }
        }

        public RiderUpdateRegId(String mid, String mType, String role, String id)
        {
            this.Mid = mid;
            this.MType = mType;
            this.Role = role;
            this.Id = id;
        }
       
    }
}
