using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Domain
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public string Message { get; set; }

        [Range(0,5)]
        public int Rating { get; set; }

        //Relation OneToOne Feedback et Reclamation
        public virtual int ComplaintFK { get; set; }
        [ForeignKey("ComplaintFK")]
        public virtual Complaint Complaint { get; set; }
    }
}
