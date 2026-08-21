using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Model.ModelTCPL
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<EmployeeSession> EmployeeSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmployeeSession>(entity =>
            {
                entity.ToTable("EmployeeSession");

                entity.HasKey(x => x.SessionId);

                entity.Property(x => x.SessionId)
                      .HasDefaultValueSql("NEWID()");

                entity.Property(x => x.LoginTime)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(x => x.IsActive)
                      .HasDefaultValue(true);

                entity.Property(x => x.CreatedDate)
                      .HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
