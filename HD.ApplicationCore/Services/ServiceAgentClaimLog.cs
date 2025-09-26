using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
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


        //Dashboard
        public Dictionary<string, double> GetAverageResolutionTimeByAdmin()
        {
            var logs = GetMany(l => l.Affected == true && l.ProcessedDate >= l.AffectedDate)
                .ToList();

            if (!logs.Any()) return new Dictionary<string, double>();

            return logs
                .GroupBy(l => l.Admin.Name)
                .ToDictionary(
                    g => g.Key,
                    g => g.Average(l => (l.ProcessedDate - l.AffectedDate).Value.TotalHours)
                );
        }

        public Dictionary<string, int> GetComplaintsCountByAdmin()
        {
            // On prend uniquement les logs actifs (Affected = true)
            var logs = GetMany(l => l.Affected == true).ToList();

            if (!logs.Any()) return new Dictionary<string, int>();

            return logs
                .GroupBy(l => l.Admin != null ? l.Admin.Name : "Inconnu")
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );
        }

    }
}
