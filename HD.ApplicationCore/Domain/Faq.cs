using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Domain
{
    public class Faq
    {
        public int FaqId { get; set; }
        public string Question { get; set; }
        public string Anwser { get; set; }

        //Relation OneToMany entre FAQ et Feature
        public virtual int FeatureFK { get; set; }
        [ForeignKey("FeatureFK")]
        public virtual Feature Feature { get; set; }
    }
}
