using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Driver.Classes
{

    //DriverController/selectAvailableVehicle
    //input : {"id":"acffb40c-6a2a-4d3d-af03-ca5371524792","uid":"acffb40c-6a2a-4d3d-af03-ca5371524792"}
    //public class VehicleInfo
    //{
    //    public double oPrice { get; set; }
    //    public double oKm { get; set; }
    //    public Double? f1Price { get; set; }
    //    public Double? f1Km { get; set; }
    //    public Double? f2Price { get; set; }
    //    public Double? f2Km { get; set; }
    //    public Double? f3Price { get; set; }
    //    public Double? f3Km { get; set; }
    //    public Double? f4Price { get; set; }
    //    public Double? f4Km { get; set; }
    //    public int vehicleId { get; set; }
    //    public String plate { get; set; }
    //    public String carTitle { get; set; }
    //    public String carLevel { get; set; }
    //    public String vRegDate { get; set; }
    //    public String manuYear { get; set; }
    //    public String status { get; set; }
    //}

    public class DriverSelectAvailableVehicleContent
    {
        public IList<VehicleInfo> vehicleInfos { get; set; }
    }

    public class DriverSelectAvailableVehicle
    {
        public string status { get; set; }
        public long lmd { get; set; }
        public DriverSelectAvailableVehicleContent content { get; set; }
    }
}
