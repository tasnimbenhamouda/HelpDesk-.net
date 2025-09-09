using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Domain
{
    public class Complaint
    {
        public int ComplaintId { get; set; }

        [MaxLength(30, ErrorMessage ="The title is too long"),MinLength(5, ErrorMessage ="Invalid title")]
        public string Title { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Submission Date")]
        public DateTime SubmissionDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Processed Date")]
        public DateTime ProcessedDate { get; set; }
        public State ComplaintState { get; set; }
        public Status ComplaintStatus { get; set; }
        public ComplaintType ComplaintType { get; set; }


        //La relation OneToMany entre Feature et Complaint
        public virtual int FeatureFK { get; set; }
        [ForeignKey("FeatureFK")]
        public virtual Feature Feature { get; set; }


        //Relation OneToMany entre Client et Complaint
        public virtual int ClientFK { get; set; }
        [ForeignKey("ClientFK")]
        public virtual Client Client { get; set; }


        //Relation ManyToMany entre Agent et Complaint
        public virtual IList<AgentClaimLog> AgentClaimLogs { get; set; }


        //Relation OneToOne Feedback et Complaint
        //Navigation Property
        public virtual Feedback Feedback { get; set; }


        //Relation OneToMany entre Message et Complaint 
        public virtual IList<Message> Messages { get; set; }

        //Relation OneToMany entre Message et Complaint 
        public virtual IList<ComplaintFile> ComplaintFiles { get; set; }


    }
}
