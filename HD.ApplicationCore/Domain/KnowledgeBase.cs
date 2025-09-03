using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Domain
{
    public class KnowledgeBase
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Response { get; set; }
        public bool AdminAdd { get; set; }
        public DateTime CreatedAt { get; set; }

        //Relation OneToMany entre Feature et KnowledgeBase
        public virtual int FeatureFK { get; set; }
        [ForeignKey("FeatureFK")]
        public virtual Feature Feature { get; set; }

    }
}
