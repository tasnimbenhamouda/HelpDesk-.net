using HD.ApplicationCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Interfaces
{
    public interface IServiceAgentClaimLog : IService<AgentClaimLog>
    {
        public void AssignComplaintToAdmin(int complaintId, int agentId, int adminId);
        public IEnumerable<AgentClaimLog> GetAssignmentsByAdmin(int adminId);
        public IEnumerable<AgentClaimLog> GetAssignmentsByComplaint(int complaintId);
        public Dictionary<string, int> GetComplaintsCountByAdmin();
        public Dictionary<string, double> GetAverageResolutionTimeByAdmin();

    }
}
