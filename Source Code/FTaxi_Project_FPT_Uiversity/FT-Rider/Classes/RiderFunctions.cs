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
        public static RiderGetNearDriverResponse ReturnTaxisPosition(string jsonInput)
        {
            RiderGetNearDriverResponse nearTaxisPosition;
            nearTaxisPosition = JsonConvert.DeserializeObject<RiderGetNearDriverResponse>(jsonInput);
            return nearTaxisPosition;
        }


        //Fare Cal
        public static Double TaxiPriceCalculator(RiderGetNearDriverResponse taxiInput, Double kmInput)
        {
            int i = 0;
            Double price;
            Double oKm = taxiInput.content.listDriverDTO[i].oKm;
            Double oPrice = taxiInput.content.listDriverDTO[i].oPrice;
            Double f1Km = (Double)taxiInput.content.listDriverDTO[i].f1Km;
            Double f2Km = (Double)taxiInput.content.listDriverDTO[i].f2Km;
            Double f3Km = (Double)taxiInput.content.listDriverDTO[i].f3Km;
            Double f4Km = (Double)taxiInput.content.listDriverDTO[i].f4Km;
            Double f1Price = (Double)taxiInput.content.listDriverDTO[i].f1Price;
            Double f2Price = (Double)taxiInput.content.listDriverDTO[i].f2Price;
            Double f3Price = (Double)taxiInput.content.listDriverDTO[i].f3Price;
            Double f4Price = (Double)taxiInput.content.listDriverDTO[i].f4Price;
            price = oPrice;
            if (kmInput > oKm && kmInput < f1Km || kmInput == f1Km)
            {
                price = price + (kmInput * f1Price);
            }
            else if (kmInput > f1Km && kmInput < f2Km || kmInput == f2Km)
            {
                price = price + (kmInput * f2Price);
            }
            else if (kmInput > f2Km && kmInput < f3Km || kmInput == f3Km)
            {
                price = price + (kmInput * f3Price);
            }
            else if (kmInput > f3Km)
            {
                price = price + (kmInput * f4Price);
            }
            return price;
        }

        public static Double estimateTaxiPriceCalculator(RiderGetNearDriverResponse taxiInput, Double kmInput)
        {
            int i = 0;
            Double price;
            Double oKm = taxiInput.content.listDriverDTO[i].oKm;
            Double oPrice = taxiInput.content.listDriverDTO[i].oPrice;
            Double f1Km = (Double)taxiInput.content.listDriverDTO[i].f1Km;
            Double f2Km = (Double)taxiInput.content.listDriverDTO[i].f2Km;
            Double f3Km = (Double)taxiInput.content.listDriverDTO[i].f3Km;
            Double f4Km = (Double)taxiInput.content.listDriverDTO[i].f4Km;
            Double f1Price = (Double)taxiInput.content.listDriverDTO[i].f1Price;
            Double f2Price = (Double)taxiInput.content.listDriverDTO[i].f2Price;
            Double f3Price = (Double)taxiInput.content.listDriverDTO[i].f3Price;
            Double f4Price = (Double)taxiInput.content.listDriverDTO[i].f4Price;
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
