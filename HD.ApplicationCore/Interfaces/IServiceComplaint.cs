using HD.ApplicationCore.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Interfaces
{
    public interface IServiceComplaint : IService<Complaint>
    {

        //Les services Agent : 
        public IEnumerable<Complaint> GetComplaintsByState(State state);
        public IEnumerable<Complaint> GetComplaintsByStatus(Status status);
        public IEnumerable<Complaint> GetComplaintsByDate(DateTime date);
        public IEnumerable<Complaint> GetComplaintsByType(ComplaintType type);
        public IEnumerable<Complaint> GetComplaintsByFeature(Feature feature);
        public IEnumerable<Complaint> GetComplaintsByProcessedDate(DateTime date);
        public IEnumerable<Complaint> GetComplaintsByAdmin(int adminId);



        //Les services client: 
        public void CreateComplaint(int clientId, string title, string description, ComplaintType type, List<string>? filePath, int featureId);
        public Complaint GetComplaintDetails(int complaintId, int clientId);
        public void DeletePendingComplaint(int complaintId, int clientId);
        public IEnumerable<Complaint> GetComplaintsByClientId(int clientId);
        public IEnumerable<Complaint> GetOpenComplaints(int clientId);
        public IEnumerable<Complaint> GetProcessedComplaints(int clientId);
        public IEnumerable<Complaint> GetClosedComplaint(int clientId);
        public IEnumerable<Complaint> GetComplaintsByType(int clientId, ComplaintType t);
        public IEnumerable<Complaint> GetComplaintsByFeature(int clientId, Feature f);
        public IEnumerable<Complaint> GetComplaintsBySubmissionDate(int clientId, DateTime date);
        public Complaint GetComplaintByTitle(int clientId, string title);
        public void ValidateClosure(int complaintId, int clientId, bool resolved);
        public void UpdateComplaintByClient(int complaintId, int clientId, string title, 
            string description, ComplaintType type, List<string>? filePath, int featureId);


        //Les Services de l'Admin
        public Complaint GetComplaintDetailsByAdmin(int adminId, int complaintId);
        public void UpdateComplaintState(int adminId, int complaintId, State newState);
        public void RollbackComplaintToAgent(int adminId, int complaintId);
        public IEnumerable<Complaint> GetComplaintsByClientName(string clientName);


        //Dashboard
        public int GetTotalComplaints();
        public double GetAverageResolutionTime();
        public Dictionary<int, int> GetComplaintsCountByFeature();
        public Dictionary<ComplaintType, int> GetComplaintsCountByType();
        public Dictionary<State, int> GetComplaintsCountByState();
        public Dictionary<Status, int> GetComplaintsCountByStatus();
    }
}
