using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PWM_asp.Models
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }

        public DbSet<SavedPWD> SavedPWDs { get; set; }
        public DbSet<ArchivedPWD> ArchivedPWDs {  get; set; }
        public DbSet<Source> Sources { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SavedPWD>()
                .ToTable(tb => tb.UseSqlOutputClause(false));

            builder.Entity<SavedPWD>()
                .HasOne(s => s.Source)
                .WithMany(s => s.SavedPWD)
                .HasForeignKey(s => s.SourceId);

            builder.Entity<ArchivedPWD>()
                .HasOne(a => a.Source)
                .WithMany(s => s.ArchivedPWD)
                .HasForeignKey(a => a.SourceId);

            builder.Entity<SavedPWD>()
                .HasOne(s => s.User)
                .WithMany(u => u.SavedPWD)
                .HasForeignKey(s => s.UserId);

            builder.Entity<ArchivedPWD>()
                .HasOne(a => a.User)
                .WithMany(u => u.ArchivedPWD)
                .HasForeignKey(a => a.UserId);



        }


    }
}
