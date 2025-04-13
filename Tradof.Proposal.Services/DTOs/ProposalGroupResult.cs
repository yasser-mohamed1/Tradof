using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tradof.Common.Enums;

namespace Tradof.Proposal.Services.DTOs
{
    public class ProposalGroupResult
    {
        public int Key { get; set; } // Month or Day
        public int Count { get; set; }
        public ProposalStatus Status { get; set; }
    }
}
