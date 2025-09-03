using HD.ApplicationCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Interfaces
{
    public interface IServiceDashboard
    {
            int GetTotalComplaints();
            Dictionary<int, int> GetComplaintsCountByAdmin(); // AdminId -> count
            double GetAverageResolutionTime();
            Dictionary<int, double> GetAverageResolutionTimeByAdmin(); // AdminId -> avg time
            Dictionary<Feature, int> GetComplaintsCountByFeature();
            Dictionary<ComplaintType, int> GetComplaintsCountByType();
            Dictionary<State, int> GetComplaintsCountByState();
            Dictionary<Status, int> GetComplaintsCountByStatus();
            double GetAverageClientFeedback();
        
    }


}
