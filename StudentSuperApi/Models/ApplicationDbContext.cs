using Microsoft.EntityFrameworkCore;
using StudentSuperApi.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<SuperAdminCredential> SuperAdmins { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<StateCity> StateCities { get; set; }
    public DbSet<SchoolBasicInformation> Schools { get; set; }
    public DbSet<SchoolAdminInfo> SchoolAdmins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Unique constraints
        modelBuilder.Entity<SuperAdminCredential>()
            .HasIndex(sa => sa.UserName)
            .IsUnique();

        modelBuilder.Entity<State>()
            .HasIndex(s => s.StateName)
            .IsUnique();

        modelBuilder.Entity<SchoolBasicInformation>()
            .HasIndex(sb => sb.SchoolCode)
            .IsUnique();

        modelBuilder.Entity<SchoolAdminInfo>()
            .HasIndex(sa => sa.Username)
            .IsUnique();

        modelBuilder.Entity<SchoolAdminInfo>()
            .HasIndex(sa => sa.Admin_ID)
            .IsUnique();

        // Bind the navigation properties explicitly so EF uses the SchoolID FK property
        modelBuilder.Entity<SchoolAdminInfo>()
            .HasOne(sa => sa.School)
            .WithMany(s => s.Admins)   // <-- specify inverse navigation here
            .HasForeignKey(sa => sa.School_Id_pk)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SchoolAdminInfo>()
            .HasOne(sa => sa.SuperAdmin)
            .WithMany(sa => sa.SchoolAdmins)
            .HasForeignKey(sa => sa.CreatedBySuperAdmin)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
