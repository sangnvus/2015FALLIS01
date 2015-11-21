using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class RiderUpdateDriverStatusDTO
    {
        //Hiện tại chưa cần dùng
        //Cài này để bên Rider cập nhật vị trí hiện tại của các Driver
        //http://123.30.236.109:8088/TN/restServices/RiderController/updateDriverStatus?json=
        //{ "uid" : "apl.ytb2@gmail.com", "did" : [ "acffb40c-6a2a-4d3d-af03-ca5371524792"] }
        private String uid;
        private List<String> did;
    }
}
