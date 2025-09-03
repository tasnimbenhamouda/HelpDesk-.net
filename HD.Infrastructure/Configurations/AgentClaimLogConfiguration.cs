using HD.ApplicationCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.Infrastructure.Configurations
{
    public class AgentClaimLogConfiguration : IEntityTypeConfiguration<AgentClaimLog>
    {
        public void Configure(EntityTypeBuilder<AgentClaimLog> builder)
        {

            builder.HasOne(acl => acl.Agent)
                   .WithMany(a => a.AgentClaimLogsAsAgent)
                   .HasForeignKey(acl => acl.AgentFK)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relation avec Admin (qui est hérite d'un Agent)
            builder.HasOne(acl => acl.Admin)
                   .WithMany(a => a.AgentClaimLogsAsAdmin)
                   .HasForeignKey(acl => acl.AdminFK)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasKey(acl => new
            {
                acl.ComplaintFK,
                acl.AgentFK,
                acl.AdminFK,
                acl.AffectedDate
            });
        }
    }
}
