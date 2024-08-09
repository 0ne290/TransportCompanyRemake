using Bogus;
using Bogus.DataSets;
using Domain.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EntityStorageServices;

public sealed class TransportCompanyContext : DbContext
{
    public static async Task LoadTestData(TransportCompanyContext dbContext)
    {
        var hazardClassesFlags = new[]
        {
            HazardClassesFlags.Class11, HazardClassesFlags.Class12, HazardClassesFlags.Class13,
            HazardClassesFlags.Class14, HazardClassesFlags.Class15, HazardClassesFlags.Class16,
            HazardClassesFlags.Class21, HazardClassesFlags.Class22, HazardClassesFlags.Class23,
            HazardClassesFlags.Class3, HazardClassesFlags.Class41, HazardClassesFlags.Class42,
            HazardClassesFlags.Class43, HazardClassesFlags.Class51, HazardClassesFlags.Class52,
            HazardClassesFlags.Class61, HazardClassesFlags.Class62, HazardClassesFlags.Class7,
            HazardClassesFlags.Class8, HazardClassesFlags.Class9
        };
        var adrDriverQualificationsFlags = new[] { AdrDriverQualificationsFlags.Base, AdrDriverQualificationsFlags.BaseAndClass7, AdrDriverQualificationsFlags.BaseAndClass1, AdrDriverQualificationsFlags.Full };
        var faker = new Faker("ru");
        var branches = new List<Branch>(7);
        var trucks = new List<Truck>(420);
        var drivers = new List<Driver>(630);

        var numberOfBranches = faker.Random.Int(3, 7);
        for (var i = 0; i < numberOfBranches; i++)
        {
            var branch =
                Branch.New(
                    $"г. {faker.Address.City()}, {faker.Address.StreetAddress()}, д. {faker.Address.BuildingNumber()}",
                    (faker.Random.Double(-90, 90), faker.Random.Double(-180, 180)));
            branches.Add(branch);
            
            var numberOfTrucks = faker.Random.Int(30, 60);
            for (var j = 0; j < numberOfTrucks; j++)
            {
                int? permittedHazardClasses;
                if (faker.Random.Bool())
                    permittedHazardClasses = faker.Random
                        .ArrayElements(hazardClassesFlags, faker.Random.Int(1, 6))
                        .Aggregate<int, int?>(0, (current, hazardClassFlag) => current | hazardClassFlag);
                else
                    permittedHazardClasses = null;

                trucks.Add(Truck.New(faker.Vehicle.Vin(), faker.Random.Bool(), faker.Random.Decimal(52, 92),
                    faker.Random.Decimal(0.7m, 1.4m), faker.Random.Decimal(27000, 33000),
                    faker.Random.Decimal(0.0003m, 0.003m), faker.Random.Decimal(0.6m, 1.4m), branch,
                    permittedHazardClasses));
            }
            
            var numberOfDrivers = numberOfTrucks * 1.5;
            for (var j = 0; j < numberOfDrivers; j++)
            {
                int? adrDriverQualificationFlag;
                bool adrDriverQualificationOfTank;
                if (faker.Random.Bool())
                {
                    adrDriverQualificationFlag = faker.Random.ArrayElement(adrDriverQualificationsFlags);
                    adrDriverQualificationOfTank = faker.Random.Bool();
                }
                else
                {
                    adrDriverQualificationFlag = null;
                    adrDriverQualificationOfTank = false;
                }

                drivers.Add(Driver.New(
                    $"{faker.Name.LastName(Name.Gender.Male)} {faker.Name.FirstName(Name.Gender.Male)}", branch,
                    adrDriverQualificationFlag, adrDriverQualificationOfTank));
            }
        }
        
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        await dbContext.Branches.AddRangeAsync(branches);
        await dbContext.Trucks.AddRangeAsync(trucks);
        await dbContext.Drivers.AddRangeAsync(drivers);
        await dbContext.SaveChangesAsync();
    }
    
    public TransportCompanyContext(DbContextOptions<TransportCompanyContext> options) : base(options) => Database.EnsureCreated();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb3_general_ci")
            .HasCharSet("utf8mb3");
        
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Guid).HasName("PRIMARY");

            entity.ToTable("branch");
        });

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
           
           entity.HasOne(d => d.Driver1).WithMany(p => p.PrimaryOrders)
               .HasForeignKey(d => d.Driver1Guid)
               .OnDelete(DeleteBehavior.NoAction)
               .HasConstraintName("Driver1Guid");
           
           entity.HasOne(d => d.Driver2).WithMany(p => p.SecondaryOrders)
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
    
    public DbSet<Branch> Branches { get; set; } = null!;

    public DbSet<Driver> Drivers { get; set; } = null!;

    public DbSet<Order> Orders { get; set; } = null!;

    public DbSet<Truck> Trucks { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;
}