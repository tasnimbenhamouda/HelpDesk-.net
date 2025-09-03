using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Domain
{
    public class Feature
    {
        public int FeatureId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
        //La relation OneToMany entre Feature et Reclamation
        public virtual IList<Complaint> Complaints { get; set; }

        //Relation OneToMany entre FAQ et Feature
        public virtual IList<Faq> Faqs { get; set; }

        //Relation OneToMany entre Feature et KnowledgeBase
        public virtual IList<KnowledgeBase> KnowledgeBases { get; set; }

    }
}
