using Microsoft.Phone.Info;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Driver.Classes
{
    class DriverFunctions
    {
        public static string GetMobileID()
        {
            byte[] myDeviceID = (byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId");
            string DeviceIDAsString = Convert.ToBase64String(myDeviceID);
            return DeviceIDAsString;
        }



        //Fare Cal
        public static Double CostCalculate(VehicleInfo taxiInput, double kmInput)
        {
            Double price;
            Double oKm = taxiInput.oKm;
            Double oPrice = taxiInput.oPrice;
            Double f1Km = (Double)taxiInput.f1Km;
            Double f2Km = (Double)taxiInput.f2Km;
            Double f3Km = (Double)taxiInput.f3Km;
            Double f4Km = (Double)taxiInput.f4Km;
            Double f1Price = (Double)taxiInput.f1Price;
            Double f2Price = (Double)taxiInput.f2Price;
            Double f3Price = (Double)taxiInput.f3Price;
            Double f4Price = (Double)taxiInput.f4Price;
            price = oPrice;

            if (kmInput > oKm && kmInput < f1Km || kmInput == f1Km)
            {
                price = oPrice + ((kmInput - oKm) * f1Price);
            }
            else if (kmInput > f1Km && kmInput < f2Km || kmInput == f2Km)
            {
                price = oPrice + (f1Km * f1Price) + (kmInput - (f1Km + oKm)) * f2Price;
            }
            else if (kmInput > f2Km && kmInput < f3Km || kmInput == f3Km)
            {
                price = oPrice + (f1Km * f1Price) + (f2Km * f2Price) + (kmInput - (oKm + f1Km + f2Km)) * f3Price;
            }
            else if (kmInput > f3Km)
            {
                price = oPrice + (f1Km * f1Price) + (f2Km * f2Price) + (f3Km * f3Price) + (kmInput - (oKm + f1Km + f2Km + f3Km)) * f4Price;
            }
            return price;
        }

    }
}
