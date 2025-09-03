using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Domain
{
    public class Client
    {
        public int clientId {  get; set; }
        public string clientName { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Pwd { get; set; }

        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }

        //Relation OneToMany entre Client et Complaint
        public virtual IList<Complaint> Complaints { get; set; }
    }
}
