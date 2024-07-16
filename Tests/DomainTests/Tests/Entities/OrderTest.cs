using System.Text.RegularExpressions;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace DomainTests.Tests.Entities;

public partial class OrderTest
{
    public OrderTest()
    {
        _guidRegex = GuidRegex();
                
        var mock = new Mock<IGeolocationService>();
                mock.Setup(gs => gs.CalculateDistanceInKmByDegrees(It.IsAny<ValueTuple<double, double>>(),
                    It.IsAny<ValueTuple<double, double>>())).Returns(StubOfCalculateDistanceInKmByDegrees);
        
        _stubOfGeolocationService = mock.Object;
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_ArgumentsIsValid_ReturnTheOrder()
    {
        // Arrange1
        var expectedUser = CreateUser();
        var expectedBranch = CreateBranch();
        var expectedTruck = CreateTruck(expectedBranch);
        var expectedDriver1 = CreateDriver1(expectedBranch);
        var expectedDriver2 = CreateDriver2(expectedBranch);
        var expectedDistanceInKm = expectedBranch.CalculateDistanceInKmByDegrees(_stubOfGeolocationService, (DefaultOrderStartPointLatitude, DefaultOrderStartPointLongitude)) + _stubOfGeolocationService.CalculateDistanceInKmByDegrees((DefaultOrderStartPointLatitude, DefaultOrderStartPointLongitude),
            (DefaultOrderEndPointLatitude, DefaultOrderEndPointLongitude)) + expectedBranch.CalculateDistanceInKmByDegrees(_stubOfGeolocationService, (DefaultOrderEndPointLatitude, DefaultOrderEndPointLongitude));
        var expectedExpectedHoursWorkedByDrivers = expectedDistanceInKm / AverageTruckSpeedInKmPerHour / 2;
        const int expextedActualHoursWorkedByDriver1 = 0;
        const int expextedActualHoursWorkedByDriver2 = 0;
        var expextedDateBegin = DateTime.Now;
        var expextedDateBeginError = TimeSpan.FromSeconds(10);
        
        // Act
        var order = CreateOrderWithTwoDriversAndHazardClassFlag(expectedUser, expectedTruck, expectedDriver1, expectedDriver2, _stubOfGeolocationService);
        
        // Arrange2
        var expectedPrice = expectedTruck.CalculateOrderPrice(order);
        
        // Assert
        Assert.Matches(_guidRegex, order.Guid);
        Assert.Equal(expextedDateBegin, order.DateBegin, expextedDateBeginError);
        Assert.Null(order.DateEnd);
        Assert.Equal(DefaultOrderHazardClassFlag, order.HazardClassFlag);
        Assert.Equal(DefaultOrderTank, order.Tank);
        Assert.Equal(expectedDistanceInKm, order.DistanceInKm);
        Assert.Equal(expectedPrice, order.Price);
        Assert.Equal(expectedExpectedHoursWorkedByDrivers, order.ExpectedHoursWorkedByDrivers);
        Assert.Equal(expextedActualHoursWorkedByDriver1, order.ActualHoursWorkedByDriver1);
        Assert.Equal(expextedActualHoursWorkedByDriver2, order.ActualHoursWorkedByDriver2);
        Assert.Equal(expectedUser, order.User);
        Assert.Equal(expectedUser.Guid, order.UserGuid);
        Assert.Equal(expectedTruck, order.Truck);
        Assert.Equal(expectedTruck.Guid, order.TruckGuid);
        Assert.False(order.Truck.IsAvailable);
        Assert.Equal(expectedDriver1, order.Driver1);
        Assert.Equal(expectedDriver1.Guid, order.Driver1Guid);
        Assert.False(order.Driver1.IsAvailable);
        Assert.Equal(expectedDriver2, order.Driver2);
        Assert.Equal(expectedDriver2.Guid, order.Driver2Guid);
        Assert.False(order.Driver2.IsAvailable);
        Assert.Equal(expectedBranch, order.Branch);
        Assert.Equal(expectedBranch.Guid, order.BranchGuid);
        Assert.Equal(expectedUser.Guid, order.UserGuid);
        Assert.Equal(DefaultOrderStartAddress, order.StartAddress);
        Assert.Equal(DefaultOrderEndAddress, order.EndAddress);
        Assert.Equal(DefaultOrderCargoDescription, order.CargoDescription);
        Assert.Equal(DefaultOrderStartPointLatitude, order.StartPointLatitude);
        Assert.Equal(DefaultOrderStartPointLongitude, order.StartPointLongitude);
        Assert.Equal(DefaultOrderEndPointLatitude, order.EndPointLatitude);
        Assert.Equal(DefaultOrderEndPointLongitude, order.EndPointLongitude);
        Assert.Equal(DefaultOrderCargoVolume, order.CargoVolume);
        Assert.Equal(DefaultOrderCargoWeight, order.CargoWeight);
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_TruckIsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruck(branch);
        truck.IsAvailable = false;
        var driver1 = CreateDriver1(branch);
        var driver2 = CreateDriver2(branch);
        
        // Act
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_Driver1IsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruck(branch);
        var driver1 = CreateDriver1(branch);
        driver1.IsAvailable = false;
        var driver2 = CreateDriver2(branch);
        
        // Act
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_Driver2IsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruck(branch);
        var driver1 = CreateDriver1(branch);
        var driver2 = CreateDriver2(branch);
        driver2.IsAvailable = false;
        
        // Act
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_TankAndDriver1AdrQualificationOfTankIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruck(branch);
        var driver1 = CreateDriver1(branch, adrQualificationOfTank: false);
        var driver2 = CreateDriver2(branch);
        
        // Act
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_TankAndDriver2AdrQualificationOfTankIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruck(branch);
        var driver1 = CreateDriver1(branch);
        var driver2 = CreateDriver2(branch, adrQualificationOfTank: false);
        
        // Act
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_TankIsTrueAndTruckTankIsFalse_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruck(branch, tank: false);
        var driver1 = CreateDriver1(branch);
        var driver2 = CreateDriver2(branch);
        
        // Act
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_TankIsFalseAndTruckTankIsTrue_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruck(branch);
        truck.IsAvailable = false;
        var driver1 = CreateDriver1(branch);
        var driver2 = CreateDriver2(branch);
        
        // Act
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService, tank: false));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_TruckBranchAndDriver1BranchIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch1 = CreateBranch();
        var branch2 = CreateBranch();
        var truck = CreateTruck(branch);
        var driver1 = CreateDriver1(branch1);
        var driver2 = CreateDriver2(branch);
        
        // Act
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }

    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_TruckBranchAndDriver2BranchIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch1 = CreateBranch();
        var branch2 = CreateBranch();
        var truck = CreateTruck(branch);
        var driver1 = CreateDriver1(branch);
        var driver2 = CreateDriver2(branch1);
        
        // Act
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }

    private static Order CreateOrderWithTwoDriversAndHazardClassFlag(User user, Truck truck, Driver driver1,
        Driver driver2, IGeolocationService geolocationService, string startAddress = DefaultOrderStartAddress,
        string endAddress = DefaultOrderEndAddress, string cargoDescription = DefaultOrderCargoDescription,
        double startPointLatitude = DefaultOrderStartPointLatitude, double startPointLongitude = DefaultOrderStartPointLongitude,
        double endPointLatitude = DefaultOrderEndPointLatitude, double endPointLongitude = DefaultOrderEndPointLongitude,
        decimal cargoVolume = DefaultOrderCargoVolume, decimal cargoWeight = DefaultOrderCargoWeight,
        int hazardClassFlag = DefaultOrderHazardClassFlag, bool tank = DefaultOrderTank) => Order.New(startAddress, endAddress,
        cargoDescription, (startPointLatitude, startPointLongitude), (endPointLatitude, endPointLongitude), cargoVolume,
        cargoWeight, hazardClassFlag, tank, user, truck, driver1, driver2, geolocationService);

    private static User CreateUser(string name = DefaultUserName, string contact = DefaultUserContact,
        long vkUserId = DefaultUserVkUserId) => User.New(name, contact, vkUserId);

    private static Branch CreateBranch(string address = DefaultBranchAddress, double latitude = DefaultBranchLatitude,
        double longitude = DefaultBranchLongitude) => Branch.New(address, (latitude, longitude));

    private static Truck CreateTruck(Branch branch, string number = DefaultTruckNumber, bool tank = DefaultTruckTank,
        decimal volumeMax = DefaultTruckVolumeMax, decimal volumePrice = DefaultTruckVolumePrice,
        decimal weightMax = DefaultTruckWeightMax, decimal weightPrice = DefaultTruckWeightPrice,
        decimal pricePerKm = DefaultTruckPricePerKm,
        int permittedHazardClassessFlags = DefaultTruckPermittedHazardClassessFlags) => Truck.New(number, tank,
        volumeMax, volumePrice, weightMax, weightPrice, pricePerKm, permittedHazardClassessFlags, branch);

    private static Driver CreateDriver1(Branch branch, string name = DefaultDriver1Name,
        int adrQualificationFlag = DefaultDriver1AdrDriverQualificationFlag,
        bool adrQualificationOfTank = DefaultDriver1AdrQualificationOfTank) =>
        Driver.New(name, adrQualificationFlag, adrQualificationOfTank, branch);
    
    private static Driver CreateDriver2(Branch branch, string name = DefaultDriver2Name,
        int adrQualificationFlag = DefaultDriver2AdrDriverQualificationFlag,
        bool adrQualificationOfTank = DefaultDriver2AdrQualificationOfTank) =>
        Driver.New(name, adrQualificationFlag, adrQualificationOfTank, branch);
    
    private static double StubOfCalculateDistanceInKmByDegrees((double Latitude, double Longitude) point1,
        (double Latitude, double Longitude) point2) =>
        point1.Latitude + point1.Longitude + point2.Latitude + point2.Longitude;
    
    [GeneratedRegex(@"^(?i)[a-z\d]{8}-([a-z\d]{4}-){3}[a-z\d]{12}$", RegexOptions.None, "ru-RU")]
    private static partial Regex GuidRegex();
    
    private const double AverageTruckSpeedInKmPerHour = 70;

    private readonly IGeolocationService _stubOfGeolocationService;

    private readonly Regex _guidRegex;
    
    private const string DefaultOrderStartAddress = "AnyStartAddress";
    
    private const string DefaultOrderEndAddress = "AnyEndAddress";
    
    private const string DefaultOrderCargoDescription = "AnyDescription";
    
    private const double DefaultOrderStartPointLatitude = 56.9;
    
    private const double DefaultOrderStartPointLongitude = 4.8;
    
    private const int DefaultOrderEndPointLatitude = -9;
    
    private const int DefaultOrderEndPointLongitude = 8;
    
    private const int DefaultOrderCargoVolume = 60;
    
    private const int DefaultOrderCargoWeight = 6000;
    
    private const int DefaultOrderHazardClassFlag = HazardClassesFlags.Class21;
    
    private const bool DefaultOrderTank = true;

    private const string DefaultUserName = "AnyName";
    
    private const string DefaultUserContact = "AnyContact";
    
    private const long DefaultUserVkUserId = 364;
    
    private const string DefaultBranchAddress = "AnyAddress";
    
    private const double DefaultBranchLatitude = 34;
    
    private const double DefaultBranchLongitude = 75;
    
    private const string DefaultTruckNumber = "AnyNumber";
    
    private const bool DefaultTruckTank = true;
    
    private const decimal DefaultTruckVolumeMax = 78;
    
    private const decimal DefaultTruckVolumePrice = 1.2m;
    
    private const decimal DefaultTruckWeightMax = 17000;
    
    private const decimal DefaultTruckWeightPrice = 0.15m;
    
    private const decimal DefaultTruckPricePerKm = 1;
    
    private const int DefaultTruckPermittedHazardClassessFlags = HazardClassesFlags.Class21 | HazardClassesFlags.Class22 | HazardClassesFlags.Class23;
    
    private const string DefaultDriver1Name = "AnyDriver1Name";
    
    private const int DefaultDriver1AdrDriverQualificationFlag = AdrDriverQualificationsFlags.Base;
    
    private const bool DefaultDriver1AdrQualificationOfTank = true;
    
    private const string DefaultDriver2Name = "AnyDriver2Name";
    
    private const int DefaultDriver2AdrDriverQualificationFlag = AdrDriverQualificationsFlags.Base;
    
    private const bool DefaultDriver2AdrQualificationOfTank = true;
}
