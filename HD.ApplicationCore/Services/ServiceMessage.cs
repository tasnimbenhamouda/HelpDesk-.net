using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Services
{
    public class ServiceMessage : Service<Message>, IServiceMessage
    {
        public ServiceMessage(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        //Récuperer les messages d'une réclamation
        public IEnumerable<Message> GetMessagesByComplaint(int complaintId)
        {
            return GetMany(m => m.ComplaintFK == complaintId)
                           .OrderBy(m => m.SendDate);
        }

        //Ajouter un message relatif à une réclamation
        public void SendMessage(int complaintId, int senderId, string content, TypeSender senderType)
        {
            var message = new Message
            {
                ComplaintFK = complaintId,
                SenderId = senderId,
                Content = content,
                TypeSender = senderType,
                SendDate = DateTime.Now
            };
            Add(message);
        }
    }
}
