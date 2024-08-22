using System.Configuration;
using System.Data.Entity;
using TestingProjectWPF.Models;

namespace TestingProjectWPF.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Balance> Balances { get; set; }
        public DbSet<UploadedFile> UploadedFiles { get; set; }

        public ApplicationContext() : base(ConfigurationManager.ConnectionStrings["DBconnection"].ConnectionString)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationContext, Migrations.Configuration>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Account>()
                        .HasRequired(a => a.Balance)
                        .WithOptional(b => b.Account);

            modelBuilder.Entity<Balance>()
                        .HasRequired(b => b.UploadedFile) 
                        .WithMany(u => u.Balances)
                        .HasForeignKey(b => b.FileId)
                        .WillCascadeOnDelete(true);
        }

    }
}
