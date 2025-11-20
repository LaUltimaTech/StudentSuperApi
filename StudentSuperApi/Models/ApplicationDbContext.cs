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

        // SchoolAdminInfo -> School FK
        modelBuilder.Entity<SchoolAdminInfo>()
            .HasOne(sa => sa.School)
            .WithMany(s => s.Admins)
            .HasForeignKey(sa => sa.School_Id_fk)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SchoolAdminInfo>()
            .HasOne(sa => sa.SuperAdmin)
            .WithMany(sa => sa.SchoolAdmins)
            .HasForeignKey(sa => sa.CreatedBySuperAdmin)
            .OnDelete(DeleteBehavior.Restrict);

        // StateCity -> State FK (ensure correct column)
        modelBuilder.Entity<StateCity>()
            .HasOne(sc => sc.State)
            .WithMany(s => s.Cities)
            .HasForeignKey(sc => sc.State_Id_fk)
            .OnDelete(DeleteBehavior.Restrict);

        // Explicitly configure School -> State and School -> City FKs
        modelBuilder.Entity<SchoolBasicInformation>()
            .HasOne(s => s.State)
            .WithMany(st => st.Schools)
            .HasForeignKey(s => s.State_Id_fk)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SchoolBasicInformation>()
            .HasOne(s => s.City)
            .WithMany(c => c.Schools)
            .HasForeignKey(s => s.City_Id_fk)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
