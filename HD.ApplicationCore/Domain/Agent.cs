using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Domain
{
    public class Agent
    {
        public int AgentId { get; set; }
        public string Name { get; set; }

        public string Pwd { get; set; }

        //Relation ManyToMany entre agent et Complaint
        public virtual IList<AgentClaimLog> AgentClaimLogsAsAgent { get; set; }

        public virtual IList<AgentClaimLog> AgentClaimLogsAsAdmin { get; set; }
    }
}
