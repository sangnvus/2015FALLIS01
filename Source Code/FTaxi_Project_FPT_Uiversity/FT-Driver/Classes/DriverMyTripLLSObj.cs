using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Driver.Classes
{
    class DriverMyTripLLSObj
    {
        private string _tid;

        public string Tid
        {
            get { return _tid; }
            set { _tid = value; }
        }

        private string _sAdd;

        public string SAdd
        {
            get { return _sAdd; }
            set { _sAdd = value; }
        }
        private string _eAdd;

        public string EAdd
        {
            get { return _eAdd; }
            set { _eAdd = value; }
        }
        private string _eTime;

        public string ETime
        {
            get { return _eTime; }
            set { _eTime = value; }
        }
        private string _rName;

        public string RName
        {
            get { return _rName; }
            set { _rName = value; }
        }
        private string _mobile;

        public string Mobile
        {
            get { return _mobile; }
            set { _mobile = value; }
        }
        private double _fare;

        public double Fare
        {
            get { return _fare; }
            set { _fare = value; }
        }
        private Double? _rate;

        public Double? Rate
        {
            get { return _rate; }
            set { _rate = value; }
        }

        public DriverMyTripLLSObj(string tid,string sAdd, string eAdd, string eTime, string rName, string mobile, double fare, Double? rate)
        {
            this.Tid = tid;
            this.SAdd = sAdd;
            this.EAdd = eAdd;
            this.ETime = eTime;
            this.RName = rName;
            this.Mobile = mobile;
            this.Fare = fare;
            this.Rate = rate;
        }
    }
}
