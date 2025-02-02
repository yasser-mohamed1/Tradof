using Microsoft.Extensions.DependencyInjection;
using Tradof.Proposal.Services.Implementation;
using Tradof.Proposal.Services.Interfaces;

namespace Tradof.Proposal.Services
{
    public static class ProposalModuleDependences
    {
        public static IServiceCollection AddProposalServices(this IServiceCollection service)
        {
            service.AddScoped<IProposalService, ProposalService>();

            return service;
        }
    }
}
