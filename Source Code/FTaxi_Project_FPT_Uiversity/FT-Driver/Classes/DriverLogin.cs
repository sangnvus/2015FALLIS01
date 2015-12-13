using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Driver.Classes
{
    //{"uid":"dao@gmail.com","pw":"b65bd772c3b0dfebf0a189efd420352d","mid":"123","mType":"iOS"}
    //pass là Abc123!
    public class DriverInfo
    {
        public string fName { get; set; }
        public string lName { get; set; }
        public string interCode { get; set; }
        public string img { get; set; }
        public string phone { get; set; }
        public string hAdd { get; set; }
        public string lan { get; set; }
        public string cntry { get; set; }
        public string email { get; set; }
        public string did { get; set; }
        public Double? rate { get; set; }
        public Double? balance { get; set; }
    }

    public class CompanyInfo
    {
        public string cName { get; set; }
        public string phone { get; set; }
        public string postal { get; set; }
        public string add { get; set; }
        public Double? vat { get; set; }
        public string city { get; set; }
        public string currency { get; set; }
    }

    public class VehicleInfo
    {
        public double oPrice { get; set; }
        public double oKm { get; set; }
        public Double? f1Price { get; set; }
        public Double? f1Km { get; set; }
        public Double? f2Price { get; set; }
        public Double? f2Km { get; set; }
        public Double? f3Price { get; set; }
        public Double? f3Km { get; set; }
        public Double? f4Price { get; set; }
        public Double? f4Km { get; set; }
        public int vehicleId { get; set; }
        public string plate { get; set; }
        public string carTitle { get; set; }
        public string carLevel { get; set; }
        public string vRegDate { get; set; }
        public string manuYear { get; set; }
        public string status { get; set; }
        public long lmd { get; set; }
        public object content { get; set; }
        public int cap { get; set; }
    }

    public class CityName
    {
        public int cityId { get; set; }
        public string lan { get; set; }
        public string cityName { get; set; }
        public string googleName { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class DriverLoginContent
    {
        public DriverInfo driverInfo { get; set; }
        public CompanyInfo companyInfo { get; set; }
        public IList<VehicleInfo> vehicleInfos { get; set; }
        public IList<CityName> cityNames { get; set; }
    }

    public class DriverLogin
    {
        public string status { get; set; }
        public long lmd { get; set; }
        public DriverLoginContent content { get; set; }
    }
}
