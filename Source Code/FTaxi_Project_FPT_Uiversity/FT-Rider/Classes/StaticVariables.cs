using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class StaticVariables
    {
        public static string queryAutocompleteRequestsBaseURI = "https://maps.googleapis.com/maps/api/place/queryautocomplete/json?key=";
        public static string googleGeolocationAPIkey = "AIzaSyAOi7TswVYRlkqvZcQ88Qf9SUHODK67TR0";
        public static string destiationAddressDescription = "Địa chỉ đón";
        public static string errInvalidAddress = "Địa chỉ không hợp lệ, vui lòng thử lại!";
    }
}
