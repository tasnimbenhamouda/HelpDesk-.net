using HD.ApplicationCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Interfaces
{
    public interface IServiceAdmin: IService<Admin>
    {
        void UpdateAccountStatus(int adminId, AccountStatus newStatus, int agentId);
        IEnumerable<Admin> GetAdminsByStatus(AccountStatus status);

        public string? GetAdminName(int adminId);
    }
}
