using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Services
{
    public class ServiceAgent : Service<Agent>, IServiceAgent
    {
        public ServiceAgent(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public string? GetAgentName(int agentId)
        {
            return Get(a=>a.AgentId==agentId).Name;
        }
    }
}
