using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Services
{
    public class ServiceFeature : Service<Feature>, IServiceFeature
    {
        public ServiceFeature(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IEnumerable<Feature> GetFeatureByClient(int clientId)
        {
            return GetMany(f => f.Complaints.Any(c => c.ClientFK == clientId));


            //var complaints = _unitOfWork.Repository<Complaint>()
            //                                   .GetMany(c => c.ClientFK == clientId);

            //return complaints.Select(c => c.Feature).Distinct().ToList();
        }
    }
}
