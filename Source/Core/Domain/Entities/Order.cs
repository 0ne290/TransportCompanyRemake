using Domain.Constants;
using Domain.Interfaces;

namespace Domain.Entities;

public class Order
{
    private Order() { }

    public static Order New(string startAddress, string endAddress, string cargoDescription, (double Latitude, double Longitude) startPoint,
        (double Latitude, double Longitude) endPoint, decimal cargoVolume, decimal cargoWeight, int? hazardClassFlag,
        User user, Truck truck,
        Driver driver1, Driver driver2, IGeolocationService geolocationService)
    {
        if (!truck.IsAvailable)
            throw new ArgumentException("To assign a truck to an order, it must be available.", nameof(truck));
        if (!driver1.IsAvailable)
            throw new ArgumentException("To assign a driver to an order, it must be available.", nameof(driver1));
        if (!driver2.IsAvailable)
            throw new ArgumentException("To assign a driver to an order, it must be available.", nameof(driver2));
        if (truck.BranchGuid != driver1.BranchGuid || truck.BranchGuid != driver2.BranchGuid)
            throw new ArgumentException("To assign drivers and truck to an order, they must belong to the same branch");
        
        var order = new Order
        {
            Guid = System.Guid.NewGuid().ToString(), DateBegin = DateTime.Now, DateEnd = null, StartAddress = startAddress, EndAddress = endAddress,
            CargoDescription = cargoDescription, StartPointLatitude = startPoint.Latitude, StartPointLongitude = startPoint.Longitude,
            EndPointLatitude = endPoint.Latitude, EndPointLongitude = endPoint.Longitude, CargoVolume = cargoVolume, CargoWeight = cargoWeight
        };
        order.SetHazardClassFlag(hazardClassFlag);
        order.SetUser(user);
        order.SetTruck(truck);
        order.SetDrivers(driver1, driver2);
        order.SetBranch(truck.Branch);
        order.CalculateAndSetDistanceInKm(geolocationService);
        order.CalculateAndSetPrice();

        return order;
    }
    
    public static Order New(string startAddress, string endAddress, string cargoDescription, (double Latitude, double Longitude) startPoint,
        (double Latitude, double Longitude) endPoint, decimal cargoVolume, decimal cargoWeight, int? hazardClassFlag,
        User user, Truck truck,
        Driver driver1, IGeolocationService geolocationService)
    {
        if (!truck.IsAvailable)
            throw new ArgumentException("To assign a truck to an order, it must be available.", nameof(truck));
        if (!driver1.IsAvailable)
            throw new ArgumentException("To assign a driver to an order, it must be available.", nameof(driver1));
        if (truck.BranchGuid != driver1.BranchGuid)
            throw new ArgumentException("To assign drivers and truck to an order, they must belong to the same branch");
        
        var order = new Order
        {
            Guid = System.Guid.NewGuid().ToString(), DateBegin = DateTime.Now, DateEnd = null, StartAddress = startAddress, EndAddress = endAddress,
            CargoDescription = cargoDescription, StartPointLatitude = startPoint.Latitude, StartPointLongitude = startPoint.Longitude,
            EndPointLatitude = endPoint.Latitude, EndPointLongitude = endPoint.Longitude, CargoVolume = cargoVolume, CargoWeight = cargoWeight
        };
        order.SetHazardClassFlag(hazardClassFlag);
        order.SetUser(user);
        order.SetTruck(truck);
        order.SetDriver(driver1);
        order.SetBranch(truck.Branch);
        order.CalculateAndSetDistanceInKm(geolocationService);
        order.CalculateAndSetPrice();

        return order;
    }

    private void SetHazardClassFlag(int? hazardClassFlag)
    {
        if (hazardClassFlag != null)
            if (!HazardClassesFlags.IsFlag(hazardClassFlag.Value))
                throw new ArgumentOutOfRangeException(nameof(hazardClassFlag), hazardClassFlag,
                    "The value is not a hazard class flag.");
            
        HazardClassFlag = hazardClassFlag;
    }

    private void CalculateAndSetDistanceInKm(IGeolocationService geolocationService)
    {
        var distanceFromBranchToStart = Branch.CalculateDistanceInKmByDegrees(geolocationService, (StartPointLatitude, StartPointLongitude));
        var distanceFromStartToEnd = geolocationService.CalculateDistanceInKmByDegrees((StartPointLatitude, StartPointLongitude), (EndPointLatitude, EndPointLongitude));
        var distanceFromEndToBranch = Branch.CalculateDistanceInKmByDegrees(geolocationService, (EndPointLatitude, EndPointLongitude));

        DistanceInKm = distanceFromBranchToStart + distanceFromStartToEnd + distanceFromEndToBranch;
    }

    private void CalculateAndSetPrice() => Price = Truck.CalculateOrderPrice(this);

    private void CalculateAndSetExpectedHoursWorkedByDrivers()
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

    private void SetUser(User user)
    {
        UserGuid = user.Guid;
        User = user;
    }
    
    private void SetTruck(Truck truck)
    {
        TruckGuid = truck.Guid;
        Truck = truck;
    }
    
    private void SetDriver(Driver driver1)
    {
        Driver1Guid = driver1.Guid;
        Driver1 = driver1;
        
        Driver2Guid = null;
        Driver2 = null;
    }

    private void SetDrivers(Driver driver1, Driver driver2)
    {
        Driver1Guid = driver1.Guid;
        Driver1 = driver1;
        
        Driver2Guid = driver2.Guid;
        Driver2 = driver2;
    }

    private void SetBranch(Branch branch)
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

    public string StartAddress { get; set; } = null!;
    
    public string EndAddress { get; set; } = null!;
    
    public string CargoDescription { get; set; } = null!;
    
    public double StartPointLatitude { get; set; }

    public double StartPointLongitude { get; set; }

    public double EndPointLatitude { get; set; }

    public double EndPointLongitude { get; set; }

    public decimal CargoVolume { get; set; }

    public decimal CargoWeight { get; set; }

    private const double AverageTruckSpeedInKmPerHour = 70;
}
