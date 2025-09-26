using HD.ApplicationCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Interfaces
{
    public interface IServiceFeedback : IService<Feedback>
    {
        public void AddFeedback(int complaintId, int rating, string comment);
        public Feedback GetFeedbackByComplaint(int complaintId);
        public double GetAverageClientFeedback();


    }
}
