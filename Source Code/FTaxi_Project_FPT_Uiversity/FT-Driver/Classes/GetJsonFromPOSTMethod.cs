using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FT_Driver.Classes
{
    class GetJsonFromPOSTMethod
    {
        //Hàm này để lấy một chuỗi Json từ Server qua phương thức POST
        //Truyền vào URL và nội dung Request
        //{\"uid\":\"dao@gmail.com\",\"pw\":\"b65bd772c3b0dfebf0a189efd420352d\",\"mid\":\"123\",\"mType\":\"iOS\"}
        public static async Task<string> GetJsonString(string url, string value)
        {
            string returnString = null;
            Dictionary<string, string> parameter = new Dictionary<string, string>();
            parameter.Add("json", value);

            HttpClient client = new HttpClient();
            HttpContent contents = new FormUrlEncodedContent(parameter);

            try
            {
                var response = await client.PostAsync(new Uri(url), contents);
                var reply = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    returnString = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception)
            {
                
                 MessageBox.Show(ConstantVariable.errConnectingError);
            }
            return returnString;
        }
    }
}
