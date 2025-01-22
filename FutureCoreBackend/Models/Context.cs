using Microsoft.EntityFrameworkCore;

namespace FutureCoreBackend.Models
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<FamilyMember>().HasQueryFilter(fm => !fm.IsDeleted);

            builder.Entity<FamilyMember>()
           .HasOne(fm => fm.Employee) // Ensure this is the correct relationship
           .WithMany(e => e.FamilyMembers)
           .HasForeignKey(fm => fm.EmployeeId); // Foreign key
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<FamilyMember> FamilyMembers { get; set; }

    }
}
