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

        public async void UpdateDriverStatus(string uid, string pwmd5, string status)
        {
            var input = string.Format("{{\"uid\":\"{0}\",\"pw\":\"{1}\",\"status\":\"{2}\"}}", uid, pwmd5, status);
            var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverUpdateStatus, input);
            try
            {
                 var driverStatus = JsonConvert.DeserializeObject<BaseResponse>(output);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public async void UpdateCurrentPosition(string uid, double lat, double lng)
        {
            var input = string.Format("{{\"uid\":\"{0}\",\"lat\":\"{1}\",\"lng\":\"{2}\"}}", uid, lat, lng);
            var output = await GetJsonFromPOSTMethod.GetJsonString(ConstantVariable.tNetDriverUpdatePosition, input);
            try
            {
                var driverStatus = JsonConvert.DeserializeObject<BaseResponse>(output);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
