using HD.ApplicationCore.Domain;
using HD.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.Infrastructure
{
    // 1 Ajout de la classe DbContext
    // 2 installation de l'ORM : Entity Framework
    // 3 Ajout de la chaine de connexion
    // 4 instllation des package Design (projet console) et tools (projet infra)
    // 5 Lancement des migrations avec la commande Add-migration nomMigration et Update-Database
    // NB : Il faut définir le projet Infrastructure comme le projet par défaut dans le Package manager console
    public class HelpDeskContext : DbContext
    {
        // 3 Ajout de la liste des entités que seront implementées dans la DB

        public DbSet<Agent> Agents { get; set; }
        public DbSet<AgentClaimLog> AgentClaimLogs { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Faq> Faqs { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Horodatage> Horodatage { get; set; }
        public DbSet<KnowledgeBase> KnowledgeBases { get; set; }
        public DbSet<Message> Messages { get; set; }


        // Ajout de la chaine de connexion 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\mssqllocaldb;
                Initial Catalog=HelpDeskDB2;Integrated Security=true");
            base.OnConfiguring(optionsBuilder);
        }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Appel des classes de configurations des fluentAPI
            modelBuilder.ApplyConfiguration(new AgentClaimLogConfiguration());

            //configuration de TPH: approche par defaut de l'heritage
            modelBuilder.Entity<Agent>()
                .HasDiscriminator<int>("AgentType") // descriminator walla esmou AgentType de type entier
                .HasValue<Agent>(0) // k n'inseri un Agent f discriminator ihot 0
                .HasValue<Admin>(2);



            base.OnModelCreating(modelBuilder);
        }
       
    }
}
