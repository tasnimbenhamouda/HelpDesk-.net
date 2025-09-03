using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Services
{
    public class ServiceFeedback : Service<Feedback>, IServiceFeedback
    {
        public ServiceFeedback(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public void AddFeedback(int complaintId, int rating, string comment)
        {
            var feedback = new Feedback
            {
                ComplaintFK = complaintId,
                Rating = rating,
                Message = comment,
            };
            Add(feedback);
        }

        public Feedback GetFeedbackByComplaint(int complaintId)
        {
            return Get(f => f.ComplaintFK == complaintId);
        }
    }
}
