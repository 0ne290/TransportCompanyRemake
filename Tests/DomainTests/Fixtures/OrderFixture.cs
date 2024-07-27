using Domain.Constants;
using Domain.Entities;

namespace DomainTests.Fixtures;

public static class OrderFixture
{
    public static Order Create(User user, string startAddress = DefaultStartAddress,
        string endAddress = DefaultEndAddress, string cargoDescription = DefaultCargoDescription,
        double startPointLatitude = DefaultStartPointLatitude, double startPointLongitude = DefaultStartPointLongitude,
        double endPointLatitude = DefaultEndPointLatitude, double endPointLongitude = DefaultEndPointLongitude,
        decimal cargoVolume = DefaultCargoVolume, decimal cargoWeight = DefaultCargoWeight, bool tank = DefaultTank,
        int? hazardClassFlag = DefaultHazardClassFlag) => Order.New(user, startAddress, endAddress, cargoDescription,
        (startPointLatitude, startPointLongitude), (endPointLatitude, endPointLongitude), cargoVolume, cargoWeight,
        tank, hazardClassFlag);

    public const double AverageTruckSpeedInKmPerHour = 70;

    public const string DefaultStartAddress = "AnyStartAddress";

    public const string DefaultEndAddress = "AnyEndAddress";

    public const string DefaultCargoDescription = "AnyDescription";

    public const double DefaultStartPointLatitude = 56.9;

    public const double DefaultStartPointLongitude = 4.8;

    public const double DefaultEndPointLatitude = -9;

    public const double DefaultEndPointLongitude = 8;

    public const decimal DefaultCargoVolume = 60;

    public const decimal DefaultCargoWeight = 6000;

    public const int DefaultHazardClassFlag = HazardClassesFlags.Class11;

    public const bool DefaultTank = true;
}