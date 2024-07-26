using Domain.Constants;
using Domain.ServiceInterfaces;

namespace Domain.Entities;

public class Order
{
    private Order() { }
    
    public static Order New(User user, string startAddress, string endAddress, string cargoDescription,
        (double Latitude, double Longitude) startPoint, (double Latitude, double Longitude) endPoint,
        decimal cargoVolume, decimal cargoWeight, bool tank, int? hazardClassFlag) => new()
    {
        Guid = System.Guid.NewGuid().ToString(), Status = OrderStatuses.AwaitingAssignmentOfPerformers, DateCreated = DateTime.Now,
        DateAssignmentOfPerformers = null, DatePaymentAndBegin = null, DateEnd = null, HazardClassFlag = hazardClassFlag, Tank = tank,
        LengthInKm = null, Price = null, ExpectedHoursWorkedByDrivers = null, ActualHoursWorkedByDriver1 = null,
        ActualHoursWorkedByDriver2 = null, UserGuid = user.Guid, TruckGuid = null, Driver1Guid = null,
        Driver2Guid = null, BranchGuid = null, User = user, Truck = null, Driver1 = null, Driver2 = null, Branch = null,
        StartAddress = startAddress, EndAddress = endAddress, CargoDescription = cargoDescription,
        StartPointLatitude = startPoint.Latitude, StartPointLongitude = startPoint.Longitude,
        EndPointLatitude = endPoint.Latitude, EndPointLongitude = endPoint.Longitude, CargoVolume = cargoVolume,
        CargoWeight = cargoWeight
    };
    
    public void AssignPerformers(IGeolocationService geolocationService, Truck truck, Driver driver1, Driver? driver2 = null)
    {
        Status = OrderStatuses.PerformersAssigned;
        DateAssignmentOfPerformers = DateTime.Now;
        
        var order = Base(orderConfig);
        order.AssignTwoDriversAndTruckAndBranchAndHazardClassFlag(driver1, driver2, truck, hazardClassFlag);
        order.AssignUser(user);
        order.AssignLengthInKmAndExpectedHoursWorkedByDriversForTwoDrivers(orderConfig, geolocationService);
        order.AssignPrice();

        truck.IsAvailable = false;
        driver1.IsAvailable = false;
        driver2.IsAvailable = false;

        return order;
    }
    
    public void ConfirmPaymentAndBegin()
    {
        Status = OrderStatuses.InProgress;
        DatePaymentAndBegin = DateTime.Now;
    }

    public static Order New(OrderConfig orderConfig, int hazardClassFlag, User user, Truck truck,
        Driver driver1, Driver driver2, IGeolocationService geolocationService)
    {
        var order = Base(orderConfig);
        order.AssignTwoDriversAndTruckAndBranchAndHazardClassFlag(driver1, driver2, truck, hazardClassFlag);
        order.AssignUser(user);
        order.AssignLengthInKmAndExpectedHoursWorkedByDriversForTwoDrivers(orderConfig, geolocationService);
        order.AssignPrice();

        truck.IsAvailable = false;
        driver1.IsAvailable = false;
        driver2.IsAvailable = false;

        return order;
    }

    private void AssignTwoDriversAndTruckAndBranchAndHazardClassFlag(Driver driver1, Driver driver2, Truck truck,
        int hazardClassFlag)
    {
        if (!driver1.IsAvailable)
            throw new ArgumentException("To assign a driver to an order, it must be available.", nameof(driver1));
        if (!driver2.IsAvailable)
            throw new ArgumentException("To assign a driver to an order, it must be available.", nameof(driver2));
        if (!truck.IsAvailable)
            throw new ArgumentException("To assign a truck to an order, it must be available.", nameof(truck));
        
        if (Tank && !driver1.AdrQualificationOfTank)
            throw new ArgumentException(
                "To assign a driver to an order with a hazard class load that requires a tank, that driver must have an ADR qualification for tanks.",
                nameof(driver1));
        if (Tank && !driver2.AdrQualificationOfTank)
            throw new ArgumentException(
                "To assign a driver to an order with a hazard class load that requires a tank, that driver must have an ADR qualification for tanks.",
                nameof(driver2));
        
        if (Tank && !truck.Tank)
            throw new ArgumentException(
                "To assign a truck to an order with cargo that requires a tank, the truck must have a tank.",
                nameof(truck));
        if (!Tank && truck.Tank)
            throw new ArgumentException(
                "To assign a truck to an order with cargo that does not require a tank, the truck must not have a tank.",
                nameof(truck));
        
        if (truck.BranchGuid != driver1.BranchGuid || truck.BranchGuid != driver2.BranchGuid)
            throw new ArgumentException("To assign drivers and truck to an order, they must belong to the same branch");
        
        if (!HazardClassesFlags.IsFlag(hazardClassFlag))
            throw new ArgumentOutOfRangeException(nameof(hazardClassFlag), hazardClassFlag,
                "The value is not a hazard class flag.");
        if ((hazardClassFlag & truck.PermittedHazardClassesFlags ?? 0) < 1)
            throw new ArgumentOutOfRangeException(nameof(hazardClassFlag), hazardClassFlag,
                "The truck cannot transport cargo with this hazard class.");
        if ((hazardClassFlag & driver1.AdrQualificationFlag ?? 0) < 1)
            throw new ArgumentOutOfRangeException(nameof(hazardClassFlag), hazardClassFlag,
                "The driver1 is not qualified enough to transport cargo with this hazard class.");
        if ((hazardClassFlag & driver2.AdrQualificationFlag ?? 0) < 1)
            throw new ArgumentOutOfRangeException(nameof(hazardClassFlag), hazardClassFlag,
                "The driver2 is not qualified enough to transport cargo with this hazard class.");

        Driver1Guid = driver1.Guid;
        Driver1 = driver1;

        Driver2Guid = driver2.Guid;
        Driver2 = driver2;
        ActualHoursWorkedByDriver2 = 0;

        TruckGuid = truck.Guid;
        Truck = truck;

        Branch = truck.Branch;
        BranchGuid = truck.BranchGuid;

        HazardClassFlag = hazardClassFlag;
    }

    public static Order New(OrderConfig orderConfig, User user, Truck truck, Driver driver1,
        Driver driver2, IGeolocationService geolocationService)
    {
        var order = Base(orderConfig);
        order.AssignTwoDriversAndTruckAndBranch(driver1, driver2, truck);
        order.AssignUser(user);
        order.AssignLengthInKmAndExpectedHoursWorkedByDriversForTwoDrivers(orderConfig, geolocationService);
        order.AssignPrice();
        
        truck.IsAvailable = false;
        driver1.IsAvailable = false;
        driver2.IsAvailable = false;

        return order;
    }

    private void AssignTwoDriversAndTruckAndBranch(Driver driver1, Driver driver2, Truck truck)
    {
        if (!driver1.IsAvailable)
            throw new ArgumentException("To assign a driver to an order, it must be available.", nameof(driver1));
        if (!driver2.IsAvailable)
            throw new ArgumentException("To assign a driver to an order, it must be available.", nameof(driver2));
        if (!truck.IsAvailable)
            throw new ArgumentException("To assign a truck to an order, it must be available.", nameof(truck));
        
        if (Tank && !truck.Tank)
            throw new ArgumentException(
                "To assign a truck to an order with cargo that requires a tank, the truck must have a tank.",
                nameof(truck));
        if (!Tank && truck.Tank)
            throw new ArgumentException(
                "To assign a truck to an order with cargo that does not require a tank, the truck must not have a tank.",
                nameof(truck));
        
        if (truck.BranchGuid != driver1.BranchGuid || truck.BranchGuid != driver2.BranchGuid)
            throw new ArgumentException("To assign drivers and truck to an order, they must belong to the same branch");

        Driver1Guid = driver1.Guid;
        Driver1 = driver1;

        Driver2Guid = driver2.Guid;
        Driver2 = driver2;
        ActualHoursWorkedByDriver2 = 0;

        TruckGuid = truck.Guid;
        Truck = truck;

        Branch = truck.Branch;
        BranchGuid = truck.BranchGuid;
    }

    public static Order New(OrderConfig orderConfig, int hazardClassFlag, User user, Truck truck,
        Driver driver1, IGeolocationService geolocationService)
    {
        var order = Base(orderConfig);
        order.AssignOneDriverAndTruckAndBranchAndHazardClassFlag(driver1, truck, hazardClassFlag);
        order.AssignUser(user);
        order.AssignLengthInKmAndExpectedHoursWorkedByDriversForOneDriver(orderConfig, geolocationService);
        order.AssignPrice();
        
        truck.IsAvailable = false;
        driver1.IsAvailable = false;

        return order;
    }

    private void AssignOneDriverAndTruckAndBranchAndHazardClassFlag(Driver driver1, Truck truck, int hazardClassFlag)
    {
        if (!driver1.IsAvailable)
            throw new ArgumentException("To assign a driver to an order, it must be available.", nameof(driver1));
        if (!truck.IsAvailable)
            throw new ArgumentException("To assign a truck to an order, it must be available.", nameof(truck));
        
        if (Tank && !driver1.AdrQualificationOfTank)
            throw new ArgumentException(
                "To assign a driver to an order with a hazard class load that requires a tank, that driver must have an ADR qualification for tanks.",
                nameof(driver1));
        
        if (Tank && !truck.Tank)
            throw new ArgumentException(
                "To assign a truck to an order with cargo that requires a tank, the truck must have a tank.",
                nameof(truck));
        if (!Tank && truck.Tank)
            throw new ArgumentException(
                "To assign a truck to an order with cargo that does not require a tank, the truck must not have a tank.",
                nameof(truck));
        
        if (truck.BranchGuid != driver1.BranchGuid)
            throw new ArgumentException("To assign drivers and truck to an order, they must belong to the same branch");
        
        if (!HazardClassesFlags.IsFlag(hazardClassFlag))
            throw new ArgumentOutOfRangeException(nameof(hazardClassFlag), hazardClassFlag,
                "The value is not a hazard class flag.");
        if ((hazardClassFlag & truck.PermittedHazardClassesFlags ?? 0) < 1)
            throw new ArgumentOutOfRangeException(nameof(hazardClassFlag), hazardClassFlag,
                "The truck cannot transport cargo with this hazard class.");
        if ((hazardClassFlag & driver1.AdrQualificationFlag ?? 0) < 1)
            throw new ArgumentOutOfRangeException(nameof(hazardClassFlag), hazardClassFlag,
                "The driver1 is not qualified enough to transport cargo with this hazard class.");

        Driver1Guid = driver1.Guid;
        Driver1 = driver1;

        TruckGuid = truck.Guid;
        Truck = truck;

        Branch = truck.Branch;
        BranchGuid = truck.BranchGuid;

        HazardClassFlag = hazardClassFlag;
    }

    public static Order New(OrderConfig orderConfig, User user, Truck truck, Driver driver1,
        IGeolocationService geolocationService)
    {
        var order = Base(orderConfig);
        order.AssignOneDriverAndTruckAndBranch(driver1, truck);
        order.AssignUser(user);
        order.AssignLengthInKmAndExpectedHoursWorkedByDriversForOneDriver(orderConfig, geolocationService);
        order.AssignPrice();
        
        truck.IsAvailable = false;
        driver1.IsAvailable = false;

        return order;
    }

    private void AssignOneDriverAndTruckAndBranch(Driver driver1, Truck truck)
    {
        if (!driver1.IsAvailable)
            throw new ArgumentException("To assign a driver to an order, it must be available.", nameof(driver1));
        if (!truck.IsAvailable)
            throw new ArgumentException("To assign a truck to an order, it must be available.", nameof(truck));
        
        if (Tank && !truck.Tank)
            throw new ArgumentException(
                "To assign a truck to an order with cargo that requires a tank, the truck must have a tank.",
                nameof(truck));
        if (!Tank && truck.Tank)
            throw new ArgumentException(
                "To assign a truck to an order with cargo that does not require a tank, the truck must not have a tank.",
                nameof(truck));
        
        if (truck.BranchGuid != driver1.BranchGuid)
            throw new ArgumentException("To assign drivers and truck to an order, they must belong to the same branch");

        Driver1Guid = driver1.Guid;
        Driver1 = driver1;

        TruckGuid = truck.Guid;
        Truck = truck;

        Branch = truck.Branch;
        BranchGuid = truck.BranchGuid;
    }

    private void AssignUser(User user)
    {
        UserGuid = user.Guid;
        User = user;
    }

    private void AssignLengthInKmAndExpectedHoursWorkedByDriversForTwoDrivers(OrderConfig orderConfig, IGeolocationService geolocationService)
    {
        var lengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt =
            Branch.CalculateLengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt(orderConfig,
                geolocationService);

        LengthInKm = lengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt.LengthInKm;
        ExpectedHoursWorkedByDrivers = lengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt.DrivingHours / 2;
    }
    
    private void AssignLengthInKmAndExpectedHoursWorkedByDriversForOneDriver(OrderConfig orderConfig, IGeolocationService geolocationService)
    {
        var lengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt =
            Branch.CalculateLengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt(orderConfig,
                geolocationService);

        LengthInKm = lengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt.LengthInKm;
        ExpectedHoursWorkedByDrivers = lengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt.DrivingHours;
    }

    private void AssignPrice() => Price = Truck.CalculateOrderPrice(this);

    public void Finish(double actualHoursWorkedByDriver1)
    {
        if (Driver2Guid != null)
            throw new InvalidOperationException(
                "The finish method for an order with one driver cannot be called for an order with two drivers.");
        if (DateEnd != null)
            throw new InvalidOperationException("The order has already been finished.");

        Truck.IsAvailable = true;
        Driver1.IsAvailable = true;
        Driver1.AddHoursWorked(actualHoursWorkedByDriver1);

        ActualHoursWorkedByDriver1 = actualHoursWorkedByDriver1;
        DateEnd = DateTime.Now;
    }

    public void Finish(double actualHoursWorkedByDriver1, double actualHoursWorkedByDriver2)
    {
        if (Driver2Guid == null)
            throw new InvalidOperationException(
                "The finish method for an order with two drivers cannot be called for an order with one driver.");
        if (DateEnd != null)
            throw new InvalidOperationException("The order has already been finished.");

        Truck.IsAvailable = true;
        Driver1.IsAvailable = true;
        Driver1.AddHoursWorked(actualHoursWorkedByDriver1);
        Driver2!.IsAvailable = true;
        Driver2.AddHoursWorked(actualHoursWorkedByDriver2);

        ActualHoursWorkedByDriver1 = actualHoursWorkedByDriver1;
        ActualHoursWorkedByDriver2 = actualHoursWorkedByDriver2;
        DateEnd = DateTime.Now;
    }

    public string Guid { get; private set; } = null!;
    
    public string Status { get; private set; } = null!;
    
    public DateTime DateCreated { get; private set; }
    
    public DateTime? DateAssignmentOfPerformers { get; private set; }
    
    public DateTime? DatePaymentAndBegin { get; private set; }

    public DateTime? DateEnd { get; private set; }

    public int? HazardClassFlag { get; private set; }

    public bool Tank { get; private set; }

    public double? LengthInKm { get; private set; }

    public decimal? Price { get; private set; }

    public double? ExpectedHoursWorkedByDrivers { get; private set; }

    public double? ActualHoursWorkedByDriver1 { get; private set; }

    public double? ActualHoursWorkedByDriver2 { get; private set; }

    public string UserGuid { get; private set; } = null!;

    public string? TruckGuid { get; private set; }

    public string? Driver1Guid { get; private set; }

    public string? Driver2Guid { get; private set; }

    public string? BranchGuid { get; private set; }

    public virtual User User { get; private set; } = null!;

    public virtual Truck? Truck { get; private set; }

    public virtual Driver? Driver1 { get; private set; }

    public virtual Driver? Driver2 { get; private set; }

    public virtual Branch? Branch { get; private set; }

    public string StartAddress { get; private set; } = null!;

    public string EndAddress { get; private set; } = null!;

    public string CargoDescription { get; private set; } = null!;

    public double StartPointLatitude { get; private set; }

    public double StartPointLongitude { get; private set; }

    public double EndPointLatitude { get; private set; }

    public double EndPointLongitude { get; private set; }

    public decimal CargoVolume { get; private set; }

    public decimal CargoWeight { get; private set; }
}