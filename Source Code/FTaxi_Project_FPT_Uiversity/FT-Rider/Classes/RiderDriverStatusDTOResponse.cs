using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class RiderDriverStatusDTOResponse
    {
        private List<DriverCurrentStatus> driverStatusList;
        private String did;
        private String status;
        private Double? lat;
        private Double? lng;
    }
    public class DriverCurrentStatus
    {
        private String did;
        private String status;
        private Double? lat;
        private Double? lng;

    }
}
