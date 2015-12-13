using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Driver.Classes
{
    class AutoCompletePlaceLLSObj
    {
        private string _placeName;

        public string Name
        {
            get { return _placeName; }
            set { _placeName = value; }
        }

        public AutoCompletePlaceLLSObj(string placeName)
        {
            this.Name = placeName;
        }
    }
}
