using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityStorageServices;

public sealed class TransportCompanyContext : DbContext
{
    public TransportCompanyContext(DbContextOptions<TransportCompanyContext> options) : base(options) => Database.EnsureCreated();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.Guid).HasName("PRIMARY");

            entity.ToTable("driver");
            
            entity.HasOne(d => d.Branch).WithMany(p => p.Drivers)
                .HasForeignKey(d => d.BranchGuid)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("Driver_BranchGuid");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Guid).HasName("PRIMARY");

            entity.ToTable("order");

           entity.HasOne(d => d.Truck).WithMany(p => p.Orders)
               .HasForeignKey(d => d.TruckGuid)
               .OnDelete(DeleteBehavior.NoAction)
               .HasConstraintName("TruckGuid");
           
           entity.HasOne(d => d.Driver1).WithMany(p => p.Orders)
               .HasForeignKey(d => d.Driver1Guid)
               .OnDelete(DeleteBehavior.NoAction)
               .HasConstraintName("Driver1Guid");
           
           entity.HasOne(d => d.Driver2).WithMany(p => p.Orders)
               .HasForeignKey(d => d.Driver2Guid)
               .OnDelete(DeleteBehavior.NoAction)
               .HasConstraintName("Driver2Guid");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserGuid)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("UserGuid");
            
            entity.HasOne(d => d.Branch).WithMany(p => p.Orders)
                .HasForeignKey(d => d.BranchGuid)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("BranchGuid");
        });

        modelBuilder.Entity<Truck>(entity =>
        {
            entity.HasKey(e => e.Guid).HasName("PRIMARY");

            entity.ToTable("truck");
            
            entity.HasIndex(e => e.Number, "TruckNumber_UNIQUE").IsUnique();

            entity.HasOne(d => d.Branch).WithMany(p => p.Trucks)
                .HasForeignKey(d => d.BranchGuid)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("Truck_BranchGuid");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Guid).HasName("PRIMARY");

            entity.ToTable("user");

            entity.HasIndex(e => e.Login, "Login_UNIQUE").IsUnique();
            entity.HasIndex(e => e.Password, "Password_UNIQUE").IsUnique();
            entity.HasIndex(e => e.VkUserId, "VkUserId_UNIQUE").IsUnique();
        });
    }

    public DbSet<Driver> Drivers { get; set; } = null!;

    public DbSet<Order> Orders { get; set; } = null!;

    public DbSet<Truck> Trucks { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;
}