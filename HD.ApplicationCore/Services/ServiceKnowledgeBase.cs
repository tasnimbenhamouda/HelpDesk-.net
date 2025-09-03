using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Services
{
    public class ServiceKnowledgeBase : Service<KnowledgeBase>, IServiceKnowledgeBase
    {
        public ServiceKnowledgeBase(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
