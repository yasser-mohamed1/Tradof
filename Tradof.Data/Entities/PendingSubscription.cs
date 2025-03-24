namespace Tradof.Data.Entities
{
    public class PendingSubscription
    {
        public int Id { get; set; }
        public long CompanyId { get; set; }
        public long PackageId { get; set; }
        public double Amount { get; set; }
        public string? TransactionReference { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}