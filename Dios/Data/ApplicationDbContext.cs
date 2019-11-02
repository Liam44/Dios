using Dios.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dios.Data
{
    public sealed class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            builder.Entity<AddressHost>()
                   .HasKey(ah => new { ah.UserId, ah.AddressID });

            builder.Entity<Parameter>()
                   .HasKey(uf => new { uf.UserId, uf.FlatID });
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<AddressHost> AddressHosts { get; set; }
        public DbSet<Flat> Flats { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<ErrorReport> ErrorReports { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
