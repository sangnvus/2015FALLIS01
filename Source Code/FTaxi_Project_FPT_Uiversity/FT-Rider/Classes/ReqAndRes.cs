using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class ReqAndRes
    {
        public static async Task<string> GetJsonString(string uri)
        {
            using (var client = new HttpClient())
            {
                var stringTask = await client.GetStringAsync(uri);
                return stringTask;
            }
        }
    }
}
