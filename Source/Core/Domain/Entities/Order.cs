using Domain.Constants;
using Domain.Interfaces;

namespace Domain.Entities;

public class Order
{
    private Order() { }

    public static Order New(string address, string cargoDescription, double startLatitude, double startLongitude,
        double endLatitude, double endLongitude, decimal cargoVolume, decimal cargoWeight, int? hazardClassFlag,
        User user, Truck truck,
        Driver driver1, Driver driver2, Branch branch, IGeolocationService geolocationService)
    {
        if (!truck.IsAvailable)
            throw new ArgumentException("To assign a truck to an order, it must be available.", nameof(truck));
        if (!driver1.IsAvailable)
            throw new ArgumentException("To assign a driver to an order, it must be available.", nameof(driver1));
        if (!driver2.IsAvailable)
            throw new ArgumentException("To assign a driver to an order, it must be available.", nameof(driver2));
        
        var order = new Order
        {
            Guid = System.Guid.NewGuid().ToString(), DateBegin = DateTime.Now, DateEnd = null, Address = address,
            CargoDescription = cargoDescription, StartLatitude = startLatitude, StartLongitude = startLongitude,
            EndLatitude = endLatitude, EndLongitude = endLongitude, CargoVolume = cargoVolume, CargoWeight = cargoWeight
        };
        order.SetHazardClassFlag(hazardClassFlag);
        order.SetUser(user);
        order.SetTruck(truck);
        order.SetDrivers(driver1, driver2);
        order.SetBranch(branch);
        order.CalculateAndSetDistanceInKm(geolocationService);
        order.CalculateAndSetPrice();

        return order;
    }
    
    public static Order New(string address, string cargoDescription, double startLatitude, double startLongitude,
        double endLatitude, double endLongitude, decimal cargoVolume, decimal cargoWeight, int? hazardClassFlag,
        User user, Truck truck,
        Driver driver1, Branch branch, IGeolocationService geolocationService)
    {
        if (!truck.IsAvailable)
            throw new ArgumentException("To assign a truck to an order, it must be available.", nameof(truck));
        if (!driver1.IsAvailable)
            throw new ArgumentException("To assign a driver to an order, it must be available.", nameof(driver1));
        
        var order = new Order
        {
            Guid = System.Guid.NewGuid().ToString(), DateBegin = DateTime.Now, DateEnd = null, Address = address,
            CargoDescription = cargoDescription, StartLatitude = startLatitude, StartLongitude = startLongitude,
            EndLatitude = endLatitude, EndLongitude = endLongitude, CargoVolume = cargoVolume, CargoWeight = cargoWeight
        };
        order.SetHazardClassFlag(hazardClassFlag);
        order.SetUser(user);
        order.SetTruck(truck);
        order.SetDriver(driver1);
        order.SetBranch(branch);
        order.CalculateAndSetDistanceInKm(geolocationService);
        order.CalculateAndSetPrice();

        return order;
    }

    public void SetHazardClassFlag(int? hazardClassFlag)
    {
        if (hazardClassFlag != null)
            if (!HazardClassesFlags.IsFlag(hazardClassFlag.Value))
                throw new ArgumentOutOfRangeException(nameof(hazardClassFlag), hazardClassFlag,
                    "The value is not a hazard class flag.");
            
        HazardClassFlag = hazardClassFlag;
    }

    public void CalculateAndSetDistanceInKm(IGeolocationService geolocationService)
    {
        var distanceFromBranchToStart = Branch.CalculateDistanceInKmByDegrees(geolocationService, (StartLatitude, StartLongitude));
        var distanceFromStartToEnd = geolocationService.CalculateDistanceInKmByDegrees((StartLatitude, StartLongitude), (EndLatitude, EndLongitude));
        var distanceFromEndToBranch = Branch.CalculateDistanceInKmByDegrees(geolocationService, (EndLatitude, EndLongitude));

        DistanceInKm = distanceFromBranchToStart + distanceFromStartToEnd + distanceFromEndToBranch;
    }

    public void CalculateAndSetPrice() => Price = Truck.CalculateOrderPrice(this);

    public void CalculateAndSetExpectedHoursWorkedByDrivers()
    {
        var drivingHours = DistanceInKm / AverageTruckSpeedInKmPerHour;
        var numberOfDrivers = Driver2Guid == null ? 1 : 2;

        ExpectedHoursWorkedByDrivers = drivingHours / numberOfDrivers;
    }
    
    public void FinishOrder(double actualHoursWorkedByDriver1)
    {
        if (Driver2Guid != null)
            throw new InvalidOperationException("The finish method for an order with one driver cannot be called for an order with two drivers.");
        
        DateEnd = DateTime.Now;
        Truck.IsAvailable = true;
        Driver1.IsAvailable = true;
        Driver2!.IsAvailable = true;
    }
    
    public void FinishOrder(double actualHoursWorkedByDriver1, double actualHoursWorkedByDriver2)
    {
        if (Driver2Guid == null)
            throw new InvalidOperationException("The finish method for an order with two drivers cannot be called for an order with one driver.");
        
        Truck.IsAvailable = true;
        Driver1.IsAvailable = true;
        Driver1.AddHoursWorked(actualHoursWorkedByDriver1);
        Driver2!.IsAvailable = true;
        Driver2.AddHoursWorked(actualHoursWorkedByDriver2);

        ActualHoursWorkedByDriver1 = actualHoursWorkedByDriver1;
        ActualHoursWorkedByDriver2 = actualHoursWorkedByDriver2;
        DateEnd = DateTime.Now;
    }

    public void SetUser(User user)
    {
        UserGuid = user.Guid;
        User = user;
    }
    
    public void SetTruck(Truck truck)
    {
        TruckGuid = truck.Guid;
        Truck = truck;
    }
    
    public void SetDriver(Driver driver1)
    {
        Driver1Guid = driver1.Guid;
        Driver1 = driver1;
        
        Driver2Guid = null;
        Driver2 = null;
    }

    public void SetDrivers(Driver driver1, Driver driver2)
    {
        Driver1Guid = driver1.Guid;
        Driver1 = driver1;
        
        Driver2Guid = driver2.Guid;
        Driver2 = driver2;
    }

    public void SetBranch(Branch branch)
    {
        Branch = branch;
        BranchGuid = branch.Guid;
    }
    
    public string Guid { get; private set; } = null!;

    public DateTime DateBegin { get; private set; }

    public DateTime? DateEnd { get; private set; }
    
    public int? HazardClassFlag { get; private set; }
    
    public double DistanceInKm { get; private set; }

    public decimal Price { get; private set; }
    
    public double ExpectedHoursWorkedByDrivers { get; private set; }
    
    public double ActualHoursWorkedByDriver1 { get; private set; }
    
    public double? ActualHoursWorkedByDriver2 { get; private set; }
    
    public string UserGuid { get; private set; } = null!;

    public string TruckGuid { get; private set; } = null!;
    
    public string Driver1Guid { get; private set; } = null!;
    
    public string? Driver2Guid { get; private set; }

    public string BranchGuid { get; private set; } = null!;

    public virtual User User { get; private set; } = null!;
    
    public virtual Truck Truck { get; private set; } = null!;

    public virtual Driver Driver1 { get; private set; } = null!;

    public virtual Driver? Driver2 { get; private set; }

    public virtual Branch Branch { get; private set; } = null!;

    public string Address { get; set; } = null!;
    
    public string CargoDescription { get; set; } = null!;
    
    public double StartLatitude { get; set; }

    public double StartLongitude { get; set; }

    public double EndLatitude { get; set; }

    public double EndLongitude { get; set; }

    public decimal CargoVolume { get; set; }

    public decimal CargoWeight { get; set; }

    private const double AverageTruckSpeedInKmPerHour = 70;
}
