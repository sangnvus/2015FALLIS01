using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    public class Pmt
    {
        public String pid { get; set; }
        public String cNO { get; set; }
        public String cvv { get; set; }
        public String mon { get; set; }
        public String year { get; set; }
        public String postal { get; set; }
        public String cate { get; set; }
    }

    public class RiderLoginContent
    {
        public String fName { get; set; }
        public String lName { get; set; }
        public String interCode { get; set; }
        public String img { get; set; }
        public String mobile { get; set; }
        public String hAdd { get; set; }
        public String lan { get; set; }
        public String cntry { get; set; }
        public String email { get; set; }
        public String rid { get; set; }
        public String name { get; set; }
        public String oAdd { get; set; }
        public Double? hAddLat { get; set; }
        public Double? hAddLng { get; set; }
        public Double? oAddLat { get; set; }
        public Double? oAddLng { get; set; }
        public IList<Pmt> pmt { get; set; }
        public long lmd { get; set; }
        public int nlmd { get; set; }
        public String status { get; set; }
        public String uid { get; set; }
        public String pw { get; set; }
    }

    public class RiderLogin
    {
        public string status { get; set; }
        public long lmd { get; set; }
        public RiderLoginContent content { get; set; }
    }

    //lưu ý cái lmd
    //các thông tin khi trả về
    //như login
    //có lmd
    //phải gửi lên đúng cái đấy
    //mới update đc
    //cập nhật profile
    //cũng phải có lmd
    //cập nhật xong cũng có cái mới

}
