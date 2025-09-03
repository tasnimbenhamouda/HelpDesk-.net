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
    public class AgentClaimLog
    {
        public bool Affected { get; set; }
        [DataType(DataType.Date)]
        [DisplayName(" Processed Date")]
        public DateTime? ProcessedDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Affected Date")]
        public DateTime AffectedDate { get; set; }

        // Relation ManyToMany entre Agent et Complaint

        public virtual int AgentFK {  get; set; }
        public virtual Agent Agent { get; set; }

        public virtual int ComplaintFK { get; set; }
        [ForeignKey("ComplaintFK")]
        public virtual Complaint Complaint { get; set; }

        public virtual int AdminFK { get; set; }
        public virtual Agent Admin { get; set; }

    }
}
