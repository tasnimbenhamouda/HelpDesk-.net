using HD.ApplicationCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Interfaces
{
    public interface IServiceMessage : IService<Message>
    {
        public void SendMessage(int complaintId, int senderId, string content, TypeSender senderType);
        public IEnumerable<Message> GetMessagesByComplaint(int complaintId);


    }
}
