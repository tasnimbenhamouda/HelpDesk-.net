using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HD.ApplicationCore.Services
{
    public class ServiceDashboard : IServiceDashboard
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceDashboard(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Total réclamations
        public int GetTotalComplaints()
        {
            return _unitOfWork.Repository<Complaint>().GetAll().Count();
        }

        // Nombre de réclamations traitées par admin
        public Dictionary<int, int> GetComplaintsCountByAdmin()
        {
            return _unitOfWork.Repository<AgentClaimLog>()
                .GetAll()
                .Where(log => log.Affected == false) // traitées ou clôturées
                .GroupBy(log => log.AdminFK)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Temps moyen global de traitement
        public double GetAverageResolutionTime()
        {
            // On ne prend que les réclamations dont ProcessedDate est strictement après SubmissionDate
            // (filtrage côté BD si GetMany est implémenté avec EF Expression -> SQL)
            var complaints = _unitOfWork.Repository<Complaint>()
                .GetMany(c => c.ProcessedDate > c.SubmissionDate)
                .ToList();

            if (!complaints.Any()) return 0;

            return complaints.Average(c => (c.ProcessedDate - c.SubmissionDate).TotalHours);
        }

        // Temps moyen par admin
        public Dictionary<int, double> GetAverageResolutionTimeByAdmin()
        {
            // On utilise AgentClaimLog : on prend les logs non actifs (affectation terminée)
            // et où la période est valide (ProcessedDate >= AffectedDate)
            var logs = _unitOfWork.Repository<AgentClaimLog>()
                .GetMany(l => l.Affected == false && l.ProcessedDate >= l.AffectedDate)
                .ToList();

            if (!logs.Any()) return new Dictionary<int, double>();

            return logs
                .GroupBy(l => l.AdminFK)
                .ToDictionary(
                    g => g.Key,
                    g => g.Average(l => (l.ProcessedDate - l.AffectedDate).Value.TotalHours)
                );
        }


        // Répartition par Feature
        public Dictionary<Feature, int> GetComplaintsCountByFeature()
        {
            return _unitOfWork.Repository<Complaint>()
                .GetAll()
                .GroupBy(c => c.Feature)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Répartition par Type
        public Dictionary<ComplaintType, int> GetComplaintsCountByType()
        {
            return _unitOfWork.Repository<Complaint>()
                .GetAll()
                .GroupBy(c => c.ComplaintType)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Répartition par Etat
        public Dictionary<State, int> GetComplaintsCountByState()
        {
            return _unitOfWork.Repository<Complaint>()
                .GetAll()
                .GroupBy(c => c.ComplaintState)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Répartition par Statut
        public Dictionary<Status, int> GetComplaintsCountByStatus()
        {
            return _unitOfWork.Repository<Complaint>()
                .GetAll()
                .GroupBy(c => c.ComplaintStatus)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Moyenne des feedbacks clients
        public double GetAverageClientFeedback()
        {
            var feedbacks = _unitOfWork.Repository<Feedback>().GetAll().ToList();

            if (!feedbacks.Any()) return 0;

            return feedbacks.Average(f => f.Rating);
        }
    }
}
