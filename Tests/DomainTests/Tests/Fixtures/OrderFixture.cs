using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;

namespace DomainTests.Tests.Fixtures;

public static class OrderFixture
{
    public static Order CreateOrderWithTwoDriversAndHazardClassFlag(User user, Truck truck, Driver driver1,
        Driver driver2, IGeolocationService geolocationService, string startAddress = DefaultOrderStartAddress,
        string endAddress = DefaultOrderEndAddress, string cargoDescription = DefaultOrderCargoDescription,
        double startPointLatitude = DefaultOrderStartPointLatitude, double startPointLongitude = DefaultOrderStartPointLongitude,
        double endPointLatitude = DefaultOrderEndPointLatitude, double endPointLongitude = DefaultOrderEndPointLongitude,
        decimal cargoVolume = DefaultOrderCargoVolume, decimal cargoWeight = DefaultOrderCargoWeight,
        int hazardClassFlag = DefaultOrderHazardClassFlag, bool tank = DefaultOrderTank) => Order.New(startAddress, endAddress,
        cargoDescription, (startPointLatitude, startPointLongitude), (endPointLatitude, endPointLongitude), cargoVolume,
        cargoWeight, hazardClassFlag, tank, user, truck, driver1, driver2, geolocationService);
    
    public static Order CreateOrderWithTwoDriversAndWithoutHazardClassFlag(User user, Truck truck, Driver driver1,
        Driver driver2, IGeolocationService geolocationService, string startAddress = DefaultOrderStartAddress,
        string endAddress = DefaultOrderEndAddress, string cargoDescription = DefaultOrderCargoDescription,
        double startPointLatitude = DefaultOrderStartPointLatitude, double startPointLongitude = DefaultOrderStartPointLongitude,
        double endPointLatitude = DefaultOrderEndPointLatitude, double endPointLongitude = DefaultOrderEndPointLongitude,
        decimal cargoVolume = DefaultOrderCargoVolume, decimal cargoWeight = DefaultOrderCargoWeight,
        bool tank = DefaultOrderTank) => Order.New(startAddress, endAddress,
        cargoDescription, (startPointLatitude, startPointLongitude), (endPointLatitude, endPointLongitude), cargoVolume,
        cargoWeight, tank, user, truck, driver1, driver2, geolocationService);
    
    public static Order CreateOrderWithOneDriverAndHazardClassFlag(User user, Truck truck, Driver driver1,
        IGeolocationService geolocationService, string startAddress = DefaultOrderStartAddress,
        string endAddress = DefaultOrderEndAddress, string cargoDescription = DefaultOrderCargoDescription,
        double startPointLatitude = DefaultOrderStartPointLatitude, double startPointLongitude = DefaultOrderStartPointLongitude,
        double endPointLatitude = DefaultOrderEndPointLatitude, double endPointLongitude = DefaultOrderEndPointLongitude,
        decimal cargoVolume = DefaultOrderCargoVolume, decimal cargoWeight = DefaultOrderCargoWeight,
        int hazardClassFlag = DefaultOrderHazardClassFlag, bool tank = DefaultOrderTank) => Order.New(startAddress, endAddress,
        cargoDescription, (startPointLatitude, startPointLongitude), (endPointLatitude, endPointLongitude), cargoVolume,
        cargoWeight, hazardClassFlag, tank, user, truck, driver1, geolocationService);

    public static User CreateUser(string name = DefaultUserName, string contact = DefaultUserContact,
        long vkUserId = DefaultUserVkUserId) => User.New(name, contact, vkUserId);

    public static Branch CreateBranch(string address = DefaultBranchAddress, double latitude = DefaultBranchLatitude,
        double longitude = DefaultBranchLongitude) => Branch.New(address, (latitude, longitude));

    public static Truck CreateTruckWithPermittedHazardClassesFlags(Branch branch, string number = DefaultTruckNumber, bool tank = DefaultTruckTank,
        decimal volumeMax = DefaultTruckVolumeMax, decimal volumePrice = DefaultTruckVolumePrice,
        decimal weightMax = DefaultTruckWeightMax, decimal weightPrice = DefaultTruckWeightPrice,
        decimal pricePerKm = DefaultTruckPricePerKm,
        int permittedHazardClassessFlags = DefaultTruckPermittedHazardClassessFlags) => Truck.New(number, tank,
        volumeMax, volumePrice, weightMax, weightPrice, pricePerKm, permittedHazardClassessFlags, branch);
    
    public static Truck CreateTruckWithoutPermittedHazardClassesFlags(Branch branch, string number = DefaultTruckNumber, bool tank = DefaultTruckTank,
        decimal volumeMax = DefaultTruckVolumeMax, decimal volumePrice = DefaultTruckVolumePrice,
        decimal weightMax = DefaultTruckWeightMax, decimal weightPrice = DefaultTruckWeightPrice,
        decimal pricePerKm = DefaultTruckPricePerKm) => Truck.New(number, tank,
        volumeMax, volumePrice, weightMax, weightPrice, pricePerKm, branch);

    public static Driver CreateDriver1WithAdrQualificationFlag(Branch branch, string name = DefaultDriver1Name,
        int adrQualificationFlag = DefaultDriver1AdrDriverQualificationFlag,
        bool adrQualificationOfTank = DefaultDriver1AdrQualificationOfTank) =>
        Driver.New(name, adrQualificationFlag, adrQualificationOfTank, branch);
    
    public static Driver CreateDriver1WithoutAdrQualificationFlag(Branch branch, string name = DefaultDriver1Name) =>
        Driver.New(name, branch);
    
    public static Driver CreateDriver2WithAdrQualificationFlag(Branch branch, string name = DefaultDriver2Name,
        int adrQualificationFlag = DefaultDriver2AdrDriverQualificationFlag,
        bool adrQualificationOfTank = DefaultDriver2AdrQualificationOfTank) =>
        Driver.New(name, adrQualificationFlag, adrQualificationOfTank, branch);
    
    public static Driver CreateDriver2WithoutAdrQualificationFlag(Branch branch, string name = DefaultDriver2Name) =>
        Driver.New(name, branch);
    
    public const double AverageTruckSpeedInKmPerHour = 70;
    
    public const string DefaultOrderStartAddress = "AnyStartAddress";
    
    public const string DefaultOrderEndAddress = "AnyEndAddress";
    
    public const string DefaultOrderCargoDescription = "AnyDescription";
    
    public const double DefaultOrderStartPointLatitude = 56.9;
    
    public const double DefaultOrderStartPointLongitude = 4.8;
    
    public const int DefaultOrderEndPointLatitude = -9;
    
    public const int DefaultOrderEndPointLongitude = 8;
    
    public const int DefaultOrderCargoVolume = 60;
    
    public const int DefaultOrderCargoWeight = 6000;
    
    public const int DefaultOrderHazardClassFlag = HazardClassesFlags.Class8;
    
    public const bool DefaultOrderTank = true;

    public const string DefaultUserName = "AnyName";
    
    public const string DefaultUserContact = "AnyContact";
    
    public const long DefaultUserVkUserId = 364;
    
    public const string DefaultBranchAddress = "AnyAddress";
    
    public const double DefaultBranchLatitude = 34;
    
    public const double DefaultBranchLongitude = 75;
    
    public const string DefaultTruckNumber = "AnyNumber";
    
    public const bool DefaultTruckTank = true;
    
    public const decimal DefaultTruckVolumeMax = 78;
    
    public const decimal DefaultTruckVolumePrice = 1.2m;
    
    public const decimal DefaultTruckWeightMax = 17000;
    
    public const decimal DefaultTruckWeightPrice = 0.15m;
    
    public const decimal DefaultTruckPricePerKm = 1;
    
    public const int DefaultTruckPermittedHazardClassessFlags = HazardClassesFlags.Class21 | HazardClassesFlags.Class22 | HazardClassesFlags.Class23 | HazardClassesFlags.Class8;
    
    public const string DefaultDriver1Name = "AnyDriver1Name";
    
    public const int DefaultDriver1AdrDriverQualificationFlag = AdrDriverQualificationsFlags.BaseAndClass8;
    
    public const bool DefaultDriver1AdrQualificationOfTank = true;
    
    public const string DefaultDriver2Name = "AnyDriver2Name";
    
    public const int DefaultDriver2AdrDriverQualificationFlag = AdrDriverQualificationsFlags.BaseAndClass8;
    
    public const bool DefaultDriver2AdrQualificationOfTank = true;
}