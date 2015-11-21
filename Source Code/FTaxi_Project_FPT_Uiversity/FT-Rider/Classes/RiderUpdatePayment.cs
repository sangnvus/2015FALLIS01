using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT_Rider.Classes
{
    class RiderUpdatePaymentRequest : RiderBase
    {
        private List<int> deletedPaymentId { get; set; }
        private List<RiderPaymentDTO> paymentDTOs { get; set; }

        public List<int> getDeletedPaymentId()
        {
            return deletedPaymentId;
        }

        public void setDeletedPaymentId(List<int> deletedPaymentId)
        {
            this.deletedPaymentId = deletedPaymentId;
        }

        public List<RiderPaymentDTO> getPaymentDTOs()
        {
            return paymentDTOs;
        }

        public void setPaymentDTOs(List<RiderPaymentDTO> paymentDTOs)
        {
            this.paymentDTOs = paymentDTOs;
        }
    }
}
