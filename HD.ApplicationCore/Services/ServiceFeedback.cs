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
            Console.WriteLine("Before Commit");
            Commit();
            Console.WriteLine("After Commit");
        }

        public double GetAverageClientFeedback()
        {
           var feedback = GetAll().ToList();

            if (!feedback.Any()) return 0;

            return feedback.Average(f => f.Rating);
        }

        public Feedback GetFeedbackByComplaint(int complaintId)
        {
            return Get(f => f.ComplaintFK == complaintId);
        }
    }
}
