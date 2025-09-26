using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Services
{
    public class ServiceAgentClaimLog : Service<AgentClaimLog>, IServiceAgentClaimLog
    {
        public ServiceAgentClaimLog(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void AssignComplaintToAdmin(int complaintId, int agentId, int adminId)
        {
            var log = new AgentClaimLog
            {
                ComplaintFK = complaintId,
                AgentFK = agentId,
                AdminFK = adminId,
                Affected = true,
                AffectedDate = DateTime.Now
            };

            Add(log);
            Console.WriteLine("Before Commit");
            Commit();
            Console.WriteLine("After Commit");
        }

        public IEnumerable<AgentClaimLog> GetAssignmentsByAdmin(int adminId)
        {
            return GetMany(l => l.AdminFK == adminId);
        }

        public IEnumerable<AgentClaimLog> GetAssignmentsByComplaint(int complaintId)
        {
            return GetMany(l => l.ComplaintFK == complaintId);
        }
    }
}
