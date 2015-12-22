using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace FT_Driver.Classes
{
    [DataContract]
    class UserData
    {
        [DataMember]
        public string uid { get; set; }
        [DataMember]
        public string password { get; set; }
    }
}
