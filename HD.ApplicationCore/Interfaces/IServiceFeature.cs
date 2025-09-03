using HD.ApplicationCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Interfaces
{
    public interface IServiceFeature : IService<Feature>
    {
        public IEnumerable<Feature> GetFeatureByClient(int clientId);    
    }
}
