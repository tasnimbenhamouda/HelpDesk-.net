using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Services
{
    public class ServiceComplaint : Service<Complaint>, IServiceComplaint
    {
        public ServiceComplaint(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        //Création d'une récalamtion par un client X
        public void CreateComplaint(int clientId, string title, string description, ComplaintType type, List<string>? filePaths, int featureId)
        {
            var complaint = new Complaint
            {
                Title = title,
                Description = description,
                ComplaintType = type,
                ComplaintFiles = new List<ComplaintFile>(),
                FeatureFK = featureId,
                ClientFK = clientId,
                SubmissionDate = DateTime.Now,
                ComplaintState = State.Pending,
            };

            if (filePaths != null && filePaths.Any())
            {
                foreach (var path in filePaths)
                {
                    complaint.ComplaintFiles.Add(new ComplaintFile { FilePath = path });
                }
            }

            Add(complaint);
            Console.WriteLine("Before Commit");
            Commit();
            Console.WriteLine("After Commit");

        }

        public void UpdateComplaintByClient(int complaintId, int clientId, 
            string title, string description, ComplaintType type, List<string>? filePath, int featureId)
        {
            // 1) Charger et vérifier l’appartenance
            var complaint = Get(c => c.ComplaintId == complaintId && c.ClientFK == clientId);
            if (complaint == null)
                throw new ArgumentException("Calim not found, or doesn't belong to this client.");

            // 2) Autoriser la modification uniquement si l’état est Pending
            if (complaint.ComplaintState != State.Pending)
                throw new InvalidOperationException("Only claims with a pending status can be modified by the customer.");

            // 3) Appliquer les changements autorisés côté client
            complaint.Title = title?.Trim();
            complaint.Description = description?.Trim();
            complaint.ComplaintType = type;

            // filePath est optionnel : si null, on garde l’existant
            if (filePath != null && filePath.Any())
            {
                foreach (var path in filePath)
                {
                    complaint.ComplaintFiles.Add(new ComplaintFile { FilePath = path });
                }
            }

            // Feature est optionnelle
            complaint.FeatureFK = featureId;

            // 4) Pas de changement sur: ClientFK, AdminFK, SubmissionDate, State, Status ici
            //    (Business: l’état reste Pending tant que l’agent/admin ne le prend pas en charge)

            Update(complaint);
            Console.WriteLine("Before Commit");
            Commit();
            Console.WriteLine("After Commit");
        }


        // Le client ne peut supprimer une réclamation que si son état est en attente (Pending).
        public void DeletePendingComplaint(int complaintId, int clientId)
        {
            var complaint = Get(c => c.ComplaintId == complaintId && c.ClientFK == clientId);

            if (complaint == null)
                throw new ArgumentException("Calim not found.");

            if (complaint.ComplaintState != State.Pending)
                throw new InvalidOperationException("Only claims with a pending status can be deleted.");

            Delete(complaint);
            Console.WriteLine("Before Commit");
            Commit();
            Console.WriteLine("After Commit");
        }

        // Récupérer toutes les réclamations d’un client.
        public IEnumerable<Complaint> GetComplaintsByClientId(int clientId)
        {
            return GetMany(c => c.ClientFK == clientId).ToList();
        }

        // Récupérer les détails d’une réclamation d’un client
        public Complaint GetComplaintDetails(int complaintId, int clientId)
        {
            return Get(c => c.ComplaintId == complaintId && c.ClientFK == clientId,
                       c => c.ComplaintFiles,  
                       c => c.Feature,         
                       c => c.Client);         
        }

        // Récupérer les réclamations ouvertes d’un client (Pending ou InProgress).
        public IEnumerable<Complaint> GetOpenComplaints(int clientId)
        {
            return GetMany(c => c.ClientFK == clientId &&
                               (c.ComplaintState == State.Pending || c.ComplaintState == State.In_Progress));
        }

        public IEnumerable<Complaint> GetProcessedComplaints(int clientId)
        {
            return GetMany(c => c.ClientFK == clientId &&
                               (c.ComplaintState == State.Processed));
        }

        public IEnumerable<Complaint> GetClosedComplaint(int clientId)
        {
            return GetMany(c => c.ClientFK == clientId
                                && (c.ComplaintState == State.Closed));
        }

        public IEnumerable<Complaint> GetComplaintsByType(int clientId, ComplaintType t)
        {
            return GetMany(c => c.ClientFK == clientId &&
                            (c.ComplaintType == t));
        }

        public IEnumerable<Complaint> GetComplaintsByFeature(int clientId, Feature f)
        {
            return GetMany(c=>c.ClientFK==clientId &&
                              c.Feature.Name== f.Name);
        }

        public IEnumerable<Complaint> GetComplaintsBySubmissionDate(int clientId, DateTime date)
        {
            return GetMany(c => c.ClientFK == clientId &&
                             (c.SubmissionDate.Date == date.Date));
        }

        public Complaint GetComplaintByTitle(int clientId, string title)
        {
           return Get(c=>c.ClientFK == clientId &&
                            ( c.Title == title));
        }

        //Un client juge qu'une réclamation est resolved ou unresolved une fois son state est Processed, si elle est unresloved elle est remise à in_progress
        public void ValidateClosure(int complaintId, int clientId, bool resolved)
        {
            var complaint = Get(c => c.ComplaintId == complaintId && c.ClientFK == clientId);

            if (complaint == null)
                throw new ArgumentException("Calim not found, or doesn't belong to this client.");

            if (complaint.ComplaintState != State.Processed)
                throw new InvalidOperationException("Claim must be 'Processed' before validation.");

            if (resolved)
            {
                // Juste marquer la réclamation comme résolue
                complaint.ComplaintStatus = Status.Resolved;
                complaint.ComplaintState=State.Closed;
            }

            else
            {
                // Sinon elle repart en traitement
                complaint.ComplaintState = State.In_Progress;
                complaint.ComplaintStatus = Status.Unresolved;
            }

            Update(complaint);
            Console.WriteLine("Before Commit");
            Commit();
            Console.WriteLine("After Commit");


        }




        //Les services de l'agent: 

        // Récupérer les réclamations par date de soumission.
        public IEnumerable<Complaint> GetComplaintsByDate(DateTime date)
        {
            return GetMany(c => c.SubmissionDate.Date == date.Date);
        }

        // Récupérer les réclamations par état (State).
        public IEnumerable<Complaint> GetComplaintsByState(State state)
        {
            return GetMany(c => c.ComplaintState == state);
        }

        // Récupérer les réclamations par statut (Status).
        public IEnumerable<Complaint> GetComplaintsByStatus(Status status)
        {
            return GetMany(c => c.ComplaintStatus == status);
        }

        public IEnumerable<Complaint> GetComplaintsByType(ComplaintType type)
        {
            return GetMany(c => c.ComplaintType == type);
        }

        public IEnumerable<Complaint> GetComplaintsByFeature(Feature feature)
        {
            return GetMany(c => c.Feature.Name == feature.Name);
        }

        public IEnumerable<Complaint> GetComplaintsByProcessedDate(DateTime date)
        {
            return GetMany(c=>c.ProcessedDate.Date == date.Date);
        }

        public IEnumerable<Complaint> GetComplaintsByAdmin(int adminId)
        {
            return GetMany(c=>c.AgentClaimLogs.Any(f=>f.AdminFK==adminId));
             
        }


        //Les Services de l'Admin
        public Complaint GetComplaintDetailsByAdmin(int adminId, int complaintId)
        {
            return Get(c => c.ComplaintId == complaintId &&
                    c.AgentClaimLogs.Any(log => log.AdminFK == adminId),
               c => c.ComplaintFiles,
               c => c.Feature,
               c => c.Client,
               c => c.AgentClaimLogs);
        }

        public void UpdateComplaintState(int adminId, int complaintId, State newState)
        {
            var complaint = Get(c => c.ComplaintId == complaintId &&
                                 c.AgentClaimLogs.Any(l => l.AdminFK == adminId));

            if (complaint == null)
                throw new ArgumentException("Calim not found, or doesn't belong to this admin.");

            complaint.ComplaintState = newState;
            if (newState == State.Closed || newState == State.Processed)
                complaint.ProcessedDate = DateTime.Now;

            Update(complaint);
            Console.WriteLine("Before Commit");
            Commit();
            Console.WriteLine("After Commit");
        }

        public void RollbackComplaintToAgent(int adminId, int complaintId)
        {
            // 1) Vérifier que la réclamation est bien affectée à cet admin
            var complaint = Get(c => c.ComplaintId == complaintId &&
                                     c.AgentClaimLogs.Any(l => l.AdminFK == adminId && l.Affected));

            if (complaint == null)
                throw new ArgumentException("Claim not found, or not assigned to this admin.");

            // 2) Mettre l'état de la réclamation en attente
            complaint.ComplaintState = State.Pending;
            Update(complaint);

            // 3) Désactiver l'affectation en cours dans AgentClaimLog
            var activeLog = complaint.AgentClaimLogs
                                     .FirstOrDefault(l => l.AdminFK == adminId && l.Affected);

            if (activeLog != null)
            {
                activeLog.Affected = false;
                activeLog.ProcessedDate = DateTime.Now;
            }
        }

        public IEnumerable<Complaint> GetComplaintsByClientName(string clientName)
        {
            return GetMany(c=>c.Client.clientName.Equals(clientName));
        }

    }
}
