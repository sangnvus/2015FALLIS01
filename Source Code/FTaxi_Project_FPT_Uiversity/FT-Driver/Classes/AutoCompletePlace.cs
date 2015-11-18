using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class AutoCompletePlace
    {
        private string _placeName;

        public string Name
        {
            get { return _placeName; }
            set { _placeName = value; }
        }

        public AutoCompletePlace(string placeName)
        {
            this.Name = placeName;
        }
    }
}
