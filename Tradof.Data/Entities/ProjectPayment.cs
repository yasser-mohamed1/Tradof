using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class ProjectPayment : AuditEntity<long>
    {
        public string TransactionNumber { get; set; }
        public double amount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public DateTime PaymenyDate { get; set; }
        public long ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
        public PaymentProcess PaymentProcess { get; set; }
    }
}