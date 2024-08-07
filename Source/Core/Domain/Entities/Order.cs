using Domain.Constants;
using Domain.Interfaces;

namespace Domain.Entities;

public class Order
{
    private Order() { }
    
    public static Order New(User user, string startAddress, string endAddress, string cargoDescription,
        (double Latitude, double Longitude) startPoint, (double Latitude, double Longitude) endPoint,
        decimal cargoVolume, decimal cargoWeight, bool tankRequired, int? hazardClassFlag)
    {
        if (hazardClassFlag != null && !HazardClassesFlags.IsFlag(hazardClassFlag.Value))
            throw new ArgumentOutOfRangeException(nameof(hazardClassFlag), hazardClassFlag,
                "The value is not a hazard class flag.");

        return new Order
        {
            UserGuid = user.Guid, User = user, StartAddress = startAddress, EndAddress = endAddress,
            CargoDescription = cargoDescription, StartPointLatitude = startPoint.Latitude,
            StartPointLongitude = startPoint.Longitude, EndPointLatitude = endPoint.Latitude,
            EndPointLongitude = endPoint.Longitude, CargoVolume = cargoVolume, CargoWeight = cargoWeight, TankRequired = tankRequired,
            HazardClassFlag = hazardClassFlag
        };
    }
    
    public void AssignPerformers(IGeolocationService geolocationService, Truck truck, Driver driver1, Driver? driver2)
    {
        if (Status != OrderStatuses.AwaitingAssignmentOfPerformers)
            throw new InvalidOperationException("Status is invalid");

        if (driver1 == driver2)
            throw new ArgumentException("It is impossible to assign two identical drivers.", $"{nameof(driver1)}; {nameof(driver2)}");
        if (!driver1.IsAvailable)
            throw new ArgumentException("To assign a driver1 to an order, it must be available.", nameof(driver1));
        if (driver2 is { IsAvailable: false })
            throw new ArgumentException("To assign a driver2 to an order, it must be available.", nameof(driver2));
        if (!truck.IsAvailable)
            throw new ArgumentException("To assign a truck to an order, it must be available.", nameof(truck));
        
        if (HazardClassFlag != null && TankRequired && !driver1.AdrQualificationOfTank)
            throw new ArgumentException(
                "To assign a driver1 to an order with a hazard class load that requires a tank, that driver must have an ADR qualification for tanks.",
                nameof(driver1));
        if (HazardClassFlag != null && driver2 != null && TankRequired && !driver2.AdrQualificationOfTank)
            throw new ArgumentException(
                "To assign a driver2 to an order with a hazard class load that requires a tank, that driver must have an ADR qualification for tanks.",
                nameof(driver2));
        
        if (TankRequired && !truck.TrailerIsTank)
            throw new ArgumentException(
                "To assign a truck to an order with cargo that requires a tank, the truck must have a tank.",
                nameof(truck));
        if (!TankRequired && truck.TrailerIsTank)
            throw new ArgumentException(
                "To assign a truck to an order with cargo that does not require a tank, the truck must not have a tank.",
                nameof(truck));
        
        if (truck.BranchGuid != driver1.BranchGuid || (driver2 != null && truck.BranchGuid != driver2.BranchGuid))
            throw new ArgumentException("To assign drivers and truck to an order, they must belong to the same branch");
        
        if (HazardClassFlag != null && (HazardClassFlag & truck.PermittedHazardClassesFlags ?? 0) < 1)
            throw new ArgumentOutOfRangeException(nameof(truck), truck,
                "The truck cannot transport cargo with this hazard class.");
        if (HazardClassFlag != null && (HazardClassFlag & driver1.AdrQualificationFlag ?? 0) < 1)
            throw new ArgumentOutOfRangeException(nameof(driver1), driver1,
                "The driver1 is not qualified enough to transport cargo with this hazard class.");
        if (HazardClassFlag != null && driver2 != null && (HazardClassFlag & driver2.AdrQualificationFlag ?? 0) < 1)
            throw new ArgumentOutOfRangeException(nameof(driver2), driver2,
                "The driver2 is not qualified enough to transport cargo with this hazard class.");

        Driver1Guid = driver1.Guid;
        Driver1 = driver1;
        Driver1.SetIsAvailable(false);
        
        if (driver2 != null)
        {
            Driver2Guid = driver2.Guid;
            Driver2 = driver2;
            Driver2.SetIsAvailable(false);
        }

        TruckGuid = truck.Guid;
        Truck = truck;
        Truck.SetIsAvailable(false);

        Branch = truck.Branch;
        BranchGuid = truck.BranchGuid;
        
        var lengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt =
        Branch.CalculateLengthInKmOfOrderRouteClosedAtBranchAndApproximateDrivingHoursOfTruckAlongIt(this,
            geolocationService);
        LengthInKm = lengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt.LengthInKm;
        ExpectedHoursWorkedByDrivers = lengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt.DrivingHours;
        if (Driver2 != null)
            ExpectedHoursWorkedByDrivers /= 2;
        
        Price = Truck.CalculateOrderPricePerKm(this) * (decimal)LengthInKm;
        
        Status = OrderStatuses.PerformersAssigned;
        DateAssignmentOfPerformers = DateTime.Now;
    }
    
    public void ConfirmPaymentAndBegin()
    {
        if (Status != OrderStatuses.PerformersAssigned)
            throw new InvalidOperationException("Status is invalid");
        
        Status = OrderStatuses.InProgress;
        DatePaymentAndBegin = DateTime.Now;
    }

    public void Finish(double actualHoursWorkedByDriver1, double? actualHoursWorkedByDriver2)
    {
        if (Status != OrderStatuses.InProgress)
            throw new InvalidOperationException("Status is invalid");
        
        if ((actualHoursWorkedByDriver2 != null && Driver2Guid == null) || (actualHoursWorkedByDriver2 == null && Driver2Guid != null))
            throw new ArgumentException("ActualHoursWorkedByDriver2 is invalid", nameof(actualHoursWorkedByDriver2));

        Truck!.SetIsAvailable(true);
        
        Driver1!.SetIsAvailable(true);
        Driver1.AddHoursWorked(actualHoursWorkedByDriver1);
        ActualHoursWorkedByDriver1 = actualHoursWorkedByDriver1;
        
        if (Driver2 != null)
        {
            Driver2.SetIsAvailable(true);
            Driver2.AddHoursWorked(actualHoursWorkedByDriver2!.Value);
            ActualHoursWorkedByDriver2 = actualHoursWorkedByDriver2;
        }

        Status = OrderStatuses.Completed;
        DateEnd = DateTime.Now;
    }

    public string Guid { get; private set; } = System.Guid.NewGuid().ToString();

    public string Status { get; private set; } = OrderStatuses.AwaitingAssignmentOfPerformers;

    public DateTime DateCreated { get; private set; } = DateTime.Now;
    
    public DateTime? DateAssignmentOfPerformers { get; private set; }
    
    public DateTime? DatePaymentAndBegin { get; private set; }

    public DateTime? DateEnd { get; private set; }

    public int? HazardClassFlag { get; private set; }

    public bool TankRequired { get; private set; }

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