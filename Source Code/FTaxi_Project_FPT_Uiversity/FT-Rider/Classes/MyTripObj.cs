using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class MyTripObj
    {
        private string _tid;

        public string Tid
        {
            get { return _tid; }
            set { _tid = value; }
        }
        private string _did;

        public string Did
        {
            get { return _did; }
            set { _did = value; }
        }
        private string _dName;

        public string DName
        {
            get { return _dName; }
            set { _dName = value; }
        }
        private string _dMobile;

        public string DMobile
        {
            get { return _dMobile; }
            set { _dMobile = value; }
        }
        private string _plate;

        public string Plate
        {
            get { return _plate; }
            set { _plate = value; }
        }
        private string _cLvl;

        public string CLvl
        {
            get { return _cLvl; }
            set { _cLvl = value; }
        }
        private string _from;

        public string From
        {
            get { return _from; }
            set { _from = value; }
        }
        private string _to;

        public string To
        {
            get { return _to; }
            set { _to = value; }
        }
        private string _sTime;

        public string STime
        {
            get { return _sTime; }
            set { _sTime = value; }
        }
        private string _eTime;

        public string ETime
        {
            get { return _eTime; }
            set { _eTime = value; }
        }
        private Double? _distance;

        public Double? Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }
        private Double? _fare;

        public Double? Fare
        {
            get { return _fare; }
            set { _fare = value; }
        }
        private Double? _payment;

        public Double? Payment
        {
            get { return _payment; }
            set { _payment = value; }
        }
        private string _currency;

        public string Currency
        {
            get { return _currency; }
            set { _currency = value; }
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
        private string _img;

        public string Img
        {
            get { return _img; }
            set { _img = value; }
        }
        private Double? _rate;

        public Double? Rate
        {
            get { return _rate; }
            set { _rate = value; }
        }
        private string _interCode;

        public string InterCode
        {
            get { return _interCode; }
            set { _interCode = value; }
        }
        private bool _favorite;

        public bool Favorite
        {
            get { return _favorite; }
            set { _favorite = value; }
        }

        public MyTripObj(string tid, string did, string dName, string dMobile, string plate, string cLvl, string from, string to, string sTime, string eTime, Double? distance, Double? fare, Double? payment, string currency, string fNam, string lName, string img, Double? rate, string interCode, bool favorite)
        {
            this.Tid = tid; 
            this.Did = did; 
            this.DName = DName; 
            this.DMobile = dMobile; 
            this.Plate = plate; 
            this.CLvl = cLvl; 
            this.From = from; 
            this.To = to; 
            this.STime = sTime; 
            this.ETime = eTime; 
            this.Distance = distance; 
            this.Fare = fare; 
            this.Payment = payment; 
            this.Currency = currency; 
            this.FName = fNam; 
            this.LName = lName; 
            this.Img = img; 
            this.Rate = rate; 
            this.InterCode = interCode; 
            this.Favorite = favorite;
        }
    }
}
