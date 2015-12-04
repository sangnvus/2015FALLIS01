using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class RiderListFavoriteDriverLLSObj
    {
        private string _fid;

        public string Fid
        {
            get { return _fid; }
            set { _fid = value; }
        }
        private string _fName;

        public string FName
        {
            get { return _fName; }
            set { _fName = value; }
        }
        private string _lName;

        public string LName
        {
            get { return _lName; }
            set { _lName = value; }
        }
        private string _mobile;

        public string Mobile
        {
            get { return _mobile; }
            set { _mobile = value; }
        }
        private string _firm;

        public string Firm
        {
            get { return _firm; }
            set { _firm = value; }
        }
        private Uri _img;

        public Uri Img
        {
            get { return _img; }
            set { _img = value; }
        }

        public RiderListFavoriteDriverLLSObj(string fid, string fName, string lName, string mobile, string firm, Uri img)
        {
            this.Fid = fid;
            this.FName = fName;
            this.LName = lName;
            this.Mobile = mobile;
            this.Firm = firm;
            this.Img = img;
        }
    }
}
