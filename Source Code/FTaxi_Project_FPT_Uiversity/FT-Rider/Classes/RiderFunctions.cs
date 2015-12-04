using Microsoft.Phone.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class RiderFunctions
    {
        //Return Near Taxi as Object
        public static RiderGetNearDriver ReturnTaxisPosition(string jsonInput)
        {
            RiderGetNearDriver nearTaxisPosition;
            nearTaxisPosition = JsonConvert.DeserializeObject<RiderGetNearDriver>(jsonInput);
            return nearTaxisPosition;
        }


        //Fare Cal
        public static Double EstimateCostCalculate(IDictionary<string, ListDriverDTO> taxiInput, string did, double kmInput)
        {
            Double price;
            Double oKm = taxiInput[did].oKm;
            Double oPrice = taxiInput[did].oPrice;
            Double f1Km = (Double)taxiInput[did].f1Km;
            Double f2Km = (Double)taxiInput[did].f2Km;
            Double f3Km = (Double)taxiInput[did].f3Km;
            Double f4Km = (Double)taxiInput[did].f4Km;
            Double f1Price = (Double)taxiInput[did].f1Price;
            Double f2Price = (Double)taxiInput[did].f2Price;
            Double f3Price = (Double)taxiInput[did].f3Price;
            Double f4Price = (Double)taxiInput[did].f4Price;
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


        public static void CallToNumber(string name, string number)
        {
            PhoneCallTask makePhone = new PhoneCallTask();
            makePhone.DisplayName = "tài xế" + " " + name;
            makePhone.PhoneNumber = number;
            makePhone.Show();
        }


    }
}
