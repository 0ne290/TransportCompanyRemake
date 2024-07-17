using Domain.Constants;
using Domain.Entities;

namespace DomainTests.Tests.Fixtures;

public static class TruckFixture
{
    public static Truck CreateWithPermittedHazardClassesFlags(Branch branch, string number = DefaultNumber,
        bool tank = DefaultTank,
        decimal volumeMax = DefaultVolumeMax, decimal volumePrice = DefaultVolumePrice,
        decimal weightMax = DefaultWeightMax, decimal weightPrice = DefaultWeightPrice,
        decimal pricePerKm = DefaultPricePerKm,
        int permittedHazardClassessFlags = DefaultPermittedHazardClassessFlags) => Truck.New(number, tank,
        volumeMax, volumePrice, weightMax, weightPrice, pricePerKm, permittedHazardClassessFlags, branch);

    public static Truck CreateWithoutPermittedHazardClassesFlags(Branch branch, string number = DefaultNumber,
        bool tank = DefaultTank,
        decimal volumeMax = DefaultVolumeMax, decimal volumePrice = DefaultVolumePrice,
        decimal weightMax = DefaultWeightMax, decimal weightPrice = DefaultWeightPrice,
        decimal pricePerKm = DefaultPricePerKm) => Truck.New(number, tank,
        volumeMax, volumePrice, weightMax, weightPrice, pricePerKm, branch);

    public const string DefaultNumber = "AnyNumber";

    public const bool DefaultTank = true;

    public const decimal DefaultVolumeMax = 78;

    public const decimal DefaultVolumePrice = 1.2m;

    public const decimal DefaultWeightMax = 17000;

    public const decimal DefaultWeightPrice = 0.15m;

    public const decimal DefaultPricePerKm = 1;

    public const int DefaultPermittedHazardClassessFlags = HazardClassesFlags.Class21 | HazardClassesFlags.Class22 |
                                                           HazardClassesFlags.Class23 | HazardClassesFlags.Class8;
}