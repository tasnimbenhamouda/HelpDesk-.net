using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Domain
{
    public class Horodatage
    {
        public double HorodatageId { get; set; }
        public int ComplaintId { get; set; }
        public int AgentId { get; set; }
        public DateTime Date { get; set; }
        public State ClaimState { get; set; }
        

    }
}
