using Microsoft.Phone.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class RiderFunctions
    {


        //Fare Cal
        public static double EstimateCostCalculate(IDictionary<string, ListDriverDTO> taxiInput, string did, double kmInput)
        { 
            double maxKm = 0;
            double maxPrice = 0;

            double price;
            double oKm = taxiInput[did].oKm;
            double oPrice = taxiInput[did].oPrice;
            double f1Km = 0;
            if (taxiInput[did].f1Km != null)
            {
                f1Km = (double)taxiInput[did].f1Km;
            }
            double f2Km = 0;
            if(taxiInput[did].f2Km != null)
            {
                f2Km = (double)taxiInput[did].f2Km;
            }
            double f3Km = 0;
            if (taxiInput[did].f3Km != null)
            {
                f3Km = (double)taxiInput[did].f3Km;
            }
            double f4Km = 0;
            if (taxiInput[did].f4Km != null)
            {
                f4Km = (double)taxiInput[did].f4Km;
            }
            double f1Price = 0;
            if (taxiInput[did].f1Price != null)
            {
                f1Price = (double)taxiInput[did].f1Price;
            }
            double f2Price = 0;
            if (taxiInput[did].f2Price != null)
            {
                f2Price = (double)taxiInput[did].f2Price;
            }
            double f3Price = 0;
            if (taxiInput[did].f3Price != null)
            {
                f3Price = (double)taxiInput[did].f3Price;
            }
            double f4Price = 0;
            if (taxiInput[did].f4Price != null)
            {
                f4Price = (double)taxiInput[did].f4Price;
            }


            
            if (f4Km == 0)
            {
                maxPrice = f3Price;
                maxKm = f3Km;
            }
            else if (f3Km == 0)
            {
                maxKm = f2Km;
                maxPrice = f2Price;
            }
            
            price = oPrice;

            if (kmInput > 0 && kmInput < oKm)
            {
                price = oPrice; // Nếu như mới lên xe hoặc đi quãng đường nhỏ hơn open km (0.6) thì bằng giá mở cửa
            }
            else if (kmInput > oKm && kmInput < (f1Km + oKm)) //Nếu đi trong khoảng từ 0.6 đến 15.6 (+ thêm 15km) thì
            {
                price = oPrice + ((kmInput - oKm) * f1Price);
            }
            else if (kmInput > (f1Km + oKm) && kmInput < (f2Km + f1Km + oKm))
            {
                price = oPrice + (f1Km * f1Price) + (kmInput - (f1Km + oKm)) * f2Price;
            }
            else if (f4Km == 0 && f3Km == 0)
            {
                price = oPrice + (f1Km * f1Price) + (kmInput - (f1Km + oKm)) * f2Price;
            }
            else if (f4Km == 0 && f3Km != 0)
            {
                price = oPrice + (f1Km * f1Price) + (f2Km * f2Price) + (kmInput - (f1Km + oKm + f2Km)) * f3Price;
            }
            else if (f4Km != 0 && f3Km != 0)
            {
                if (kmInput > (oKm + f1Km + f2Km + f3Km) && kmInput < (oKm + f1Km + f2Km + f3Km + f4Km))
                {
                    price = oPrice + (f1Km * f1Price) + (f2Km * f2Price) + (f3Km * f3Price) +  (kmInput - (f1Km + oKm + f2Km + f3Km)) * f4Price;
                }
                else
                {
                    price = oPrice + (f1Km * f1Price) + (f2Km * f2Price) + (kmInput - (f1Km + oKm + f2Km)) * f3Price;
                }
            }
            return price;
        }


        public static void CallToNumber(string name, string number)
        {
            PhoneCallTask makePhone = new PhoneCallTask();
            makePhone.DisplayName = name;
            makePhone.PhoneNumber = number;
            makePhone.Show();
        }


    }
}
