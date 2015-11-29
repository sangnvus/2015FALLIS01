using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Driver.Classes
{
    class DriverVehiceInfoObj
    {
        private string _plate;
        private string _carTitle;
        private string _carLevel;
        private Uri _imgUrl;
        public System.Uri ImgUrl
        {
            get { return _imgUrl; }
            set { _imgUrl = value; }
        }
        public string Plate
        {
            get { return _plate; }
            set { _plate = value; }
        }

        public string CarTitle
        {
            get { return _carTitle; }
            set { _carTitle = value; }
        }

        public string CarLevel
        {
            get { return _carLevel; }
            set { _carLevel = value; }
        }

        public DriverVehiceInfoObj(string plate, string carTitle, string carLevel, Uri imgUrl)
        {
            this.Plate = plate;
            this.CarTitle = carTitle;
            this.CarLevel = carLevel;
            this.ImgUrl = imgUrl;
        }
    }
}
