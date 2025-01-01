using System.ComponentModel.DataAnnotations.Schema;
using Tradof.Common.Base;
using Tradof.Common.Enums;

namespace Tradof.Data.Entities
{
    public class Invoice : AuditEntity<long>
    {
        public int InvoiceNumber { get; set; }
        public double TotalAmount { get; set; }
        public InvoiceStatus InvoiceStatus { get; set; }
        public long ProjectId { get; set; }
        public long ProjectPaymentId { get; set; }

        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
        [ForeignKey("ProjectPaymentId")]
        public ProjectPayment ProjectPayment { get; set; }
    }
}