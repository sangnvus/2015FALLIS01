using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class TaxiItemObj
    {
        private string _id;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private string _phone;

        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        public TaxiItemObj(string id, string name, string phone)
        {
            this.Id = id;
            this.Name = name;
            this.Phone = phone;
        }

    }
}
