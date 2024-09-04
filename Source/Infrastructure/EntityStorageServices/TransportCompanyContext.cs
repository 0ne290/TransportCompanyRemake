using Bogus;
using Bogus.DataSets;
using Domain.Constants;
using Domain.DefaultImplementations;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EntityStorageServices;

public sealed class TransportCompanyContext : DbContext
{
    public static async Task LoadTestData(TransportCompanyContext dbContext)
    {
        var faker = new Faker("ru");
        
        var defaultCryptographicService = new DefaultCryptographicService();
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ123456789!=-_";
        var numberOfUsers = faker.Random.Int(75, 125);
        var users = new User[numberOfUsers];
        for (var i = 0; i < numberOfUsers; i++)
            users[i] = User.New($"{faker.Name.LastName(Name.Gender.Male)} {faker.Name.FirstName(Name.Gender.Male)}",
                faker.Phone.PhoneNumber("+7 (###) ###-##-##"), faker.Random.String2(12, 24, chars),
                faker.Random.String2(12, 24, chars), defaultCryptographicService);
        
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
        var numberOfBranches = faker.Random.Int(3, 7);
        var branches = new Branch[numberOfBranches];
        var trucks = new List<Truck>(420);
        var drivers = new List<Driver>(630);
        for (var i = 0; i < numberOfBranches; i++)
        {
            var branch =
                Branch.New(
                    $"г. {faker.Address.City()}, {faker.Address.StreetAddress()}, д. {faker.Address.BuildingNumber()}",
                    (faker.Random.Double(-90, 90), faker.Random.Double(-180, 180)));
            branches[i] = branch;
            
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

                var truck = Truck.New(faker.Vehicle.Vin(), faker.Random.Bool(), faker.Random.Decimal(52, 92),
                    faker.Random.Decimal(0.7m, 1.4m), faker.Random.Decimal(27000, 33000),
                    faker.Random.Decimal(0.0003m, 0.003m), faker.Random.Decimal(0.6m, 1.4m), branch,
                    permittedHazardClasses);
                branch.Trucks.Add(truck);
                trucks.Add(truck);
            }
            
            for (var j = 0; j < numberOfTrucks * 1.5; j++)
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

                var driver = Driver.New(
                    $"{faker.Name.LastName(Name.Gender.Male)} {faker.Name.FirstName(Name.Gender.Male)}", branch,
                    adrDriverQualificationFlag, adrDriverQualificationOfTank);
                branch.Drivers.Add(driver);
                drivers.Add(driver);
            }
        }
        
        var defaultGeolocationService = new DefaultGeolocationService();
        var numberOfOrders = faker.Random.Int(4 * numberOfUsers, 6 * numberOfUsers);
        var orders = new Order[numberOfOrders];
        for (var i = 0; i < numberOfOrders; i++)
        {
            int? hazardClassFlag;
            if (faker.Random.Bool())
                hazardClassFlag = faker.Random.ArrayElement(hazardClassesFlags);
            else
                hazardClassFlag = null;

            var order = Order.New(faker.Random.ArrayElement(users),
                $"г. {faker.Address.City()}, {faker.Address.StreetAddress()}, д. {faker.Address.BuildingNumber()}",
                $"г. {faker.Address.City()}, {faker.Address.StreetAddress()}, д. {faker.Address.BuildingNumber()}",
                faker.Random.String2(12, 24, chars), (faker.Random.Double(-90, 90), faker.Random.Double(-180, 180)),
                (faker.Random.Double(-90, 90), faker.Random.Double(-180, 180)), faker.Random.Decimal(1, 70),
                faker.Random.Decimal(1, 30000), faker.Random.Bool(), hazardClassFlag);
            var performers = TryFindOrderPerformers(branches, order, defaultGeolocationService, faker.Random.Bool());
            if (performers == null)
            {
                for (var j = 0; j < i; j++)
                {
                    var currentOrder = orders[j];
                    if (currentOrder.Status != OrderStatuses.InProgress)
                        continue;
                    if (currentOrder.Driver2 == null)
                        currentOrder.Finish(faker.Random.Double(currentOrder.ExpectedHoursWorkedByDrivers!.Value - 2, currentOrder.ExpectedHoursWorkedByDrivers.Value + 2), null);
                    else
                        currentOrder.Finish(faker.Random.Double(currentOrder.ExpectedHoursWorkedByDrivers!.Value - 2, currentOrder.ExpectedHoursWorkedByDrivers.Value + 2), faker.Random.Double(currentOrder.ExpectedHoursWorkedByDrivers!.Value - 2, currentOrder.ExpectedHoursWorkedByDrivers.Value + 2));
                }
                
                performers = TryFindOrderPerformers(branches, order, defaultGeolocationService, faker.Random.Bool());
            }
            
            order.AssignPerformers(defaultGeolocationService, performers!.Value.Truck, performers.Value.Driver1, performers.Value.Driver2);
            order.ConfirmPaymentAndBegin();
            orders[i] = order;
        }
        for (var i = 0; i < numberOfOrders; i++)
        {
            var currentOrder = orders[i];
            if (currentOrder.Status != OrderStatuses.InProgress)
                continue;
            if (currentOrder.Driver2 == null)
                currentOrder.Finish(faker.Random.Double(currentOrder.ExpectedHoursWorkedByDrivers!.Value - 2, currentOrder.ExpectedHoursWorkedByDrivers.Value + 2), null);
            else
                currentOrder.Finish(faker.Random.Double(currentOrder.ExpectedHoursWorkedByDrivers!.Value - 2, currentOrder.ExpectedHoursWorkedByDrivers.Value + 2), faker.Random.Double(currentOrder.ExpectedHoursWorkedByDrivers!.Value - 2, currentOrder.ExpectedHoursWorkedByDrivers.Value + 2));
        }
        
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        await dbContext.Users.AddRangeAsync(users);
        await dbContext.Branches.AddRangeAsync(branches);
        await dbContext.Trucks.AddRangeAsync(trucks);
        await dbContext.Drivers.AddRangeAsync(drivers);
        await dbContext.Orders.AddRangeAsync(orders);
        await dbContext.SaveChangesAsync();
    }

    private static (Truck Truck, Driver Driver1, Driver? Driver2)? TryFindOrderPerformers(IEnumerable<Branch> branches, Order order, IGeolocationService geolocationService, bool twoDrivers)
    {
        branches = branches.OrderBy(b => b.CalculateLengthInKmOfOrderRouteClosedAtBranchAndApproximateDrivingHoursOfTruckAlongIt(order, geolocationService));
        Func<ICollection<Driver>, bool> continueCondition = twoDrivers ? d => d.Count < 2 : d => d.Count < 1;
        Func<Truck, bool> truckPredicate;
        Func<Driver, bool> driverPredicate;
        if (order.HazardClassFlag != null)
        {
            truckPredicate = t =>
                t.IsAvailable && t.VolumeMax >= order.CargoVolume && t.WeightMax >= order.CargoWeight && t.TrailerIsTank == order.TankRequired &&
                (order.HazardClassFlag & t.PermittedHazardClassesFlags ?? 0) > 0;
            if (order.TankRequired)
                driverPredicate = d =>
                    d.IsAvailable && (order.HazardClassFlag & d.AdrQualificationFlag ?? 0) > 0 &&
                    d.AdrQualificationOfTank;
            else
                driverPredicate = d => d.IsAvailable && (order.HazardClassFlag & d.AdrQualificationFlag ?? 0) > 0;
        }
        else
        {
            truckPredicate = t =>
                t.IsAvailable && t.VolumeMax >= order.CargoVolume && t.WeightMax >= order.CargoWeight && t.TrailerIsTank == order.TankRequired;
            driverPredicate = d => d.IsAvailable;
        }

        foreach (var branch in branches)
        {
            var truck = branch.Trucks.Where(t => truckPredicate(t)).MinBy(t => t.CalculateOrderPricePerKm(order));
            var drivers = branch.Drivers.Where(d => driverPredicate(d)).Take(2).ToList();
            
            if (truck == null || continueCondition(drivers))
                continue;

            return twoDrivers ? (truck, drivers[0], drivers[1]) : (truck, drivers[0], null);
        }

        return null;
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