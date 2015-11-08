using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class BaseCoordinate
    {
        double lat;
        double lng;

        public double Lat
        {
            get { return lat; }
            set { lat = value; }
        }
        
        public double Lng
        {
            get { return lng; }
            set { lng = value; }
        }

        public BaseCoordinate(double a, double b)
        {
            this.lat = a;
            this.lng = b;
        }
    }
}
