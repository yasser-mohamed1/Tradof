using Tradof.Common.Base;

namespace Tradof.Data.Entities
{
    public class ProposalEditRequest : AuditEntity<long>
    {
        public int NewDuration { get; set; }
        public double NewPrice { get; set; }

    }
}
