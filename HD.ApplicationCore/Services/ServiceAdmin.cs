using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Services
{
    public class ServiceAdmin : Service<Admin>, IServiceAdmin
    {
        public ServiceAdmin(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public string? GetAdminName(int adminId)
        {
            var admin = Get(a => a.AgentId == adminId);
            if (admin == null)
                throw new KeyNotFoundException($"Admin with ID {adminId} not found.");

            return admin.Name;
        }

        public IEnumerable<object> GetAdmins()
        {
            return GetAll()
                .Select(a => new { a.AgentId, a.Name })
                .ToList();
        }

        public IEnumerable<Admin> GetAdminsByStatus(AccountStatus status)
        {
            return GetMany(a => a.AccountStatus == status);
        }

        public void UpdateAccountStatus(int adminId, AccountStatus newStatus, int agentId)
        {
            var admin = Get(a => a.AgentId == adminId);

            if (admin == null)
                throw new ArgumentException("Admin not found.");

            admin.AccountStatus = newStatus;
            Update(admin);
        }
    }
}
