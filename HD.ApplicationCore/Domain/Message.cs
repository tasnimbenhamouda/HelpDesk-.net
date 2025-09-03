using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Domain
{
    public class Message
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; }
        public TypeSender TypeSender { get; set; }  
        public DateTime SendDate { get; set; }

        //Relation OneToMany entre Message et Complaint 
        public virtual int ComplaintFK { get; set; }
        [ForeignKey("ComplaintFK")]
        public virtual Complaint Complaint { get; set; }
    }


}
