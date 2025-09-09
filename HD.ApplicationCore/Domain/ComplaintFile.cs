using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Domain
{
    public class ComplaintFile
    {
        public int ComplaintFileId { get; set; }
        public string FilePath { get; set; }

        //Relation OneToMany entre complaintFile et Complaint 
        public virtual int ComplaintFK { get; set; }
        [ForeignKey("ComplaintFK")]
        public virtual Complaint Complaint { get; set; }
    }
}
