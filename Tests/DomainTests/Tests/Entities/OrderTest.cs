using System.Text.RegularExpressions;
using Domain.Constants;
using Domain.Interfaces;
using DomainTests.Tests.Fixtures;
using DomainTests.Tests.Stubs;

namespace DomainTests.Tests.Entities;

public class OrderTest
{
    // OrderWithTwoDriversAndHazardClassFlag
    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_ArgumentsIsValid_ReturnTheOrder()
    {
        // Arrange1
        var expectedUser = UserFixture.CreateVk();
        var expectedBranch = BranchFixture.Create();
        var expectedTruck = TruckFixture.CreateWithPermittedHazardClassesFlags(expectedBranch);
        var expectedDriver1 = DriverFixture.CreateWithAdrQualificationFlag(expectedBranch);
        var expectedDriver2 = DriverFixture.CreateWithAdrQualificationFlag(expectedBranch);
        var expectedDistanceInKm =
            expectedBranch.CalculateDistanceInKmByDegrees(_geolocationServiceStub,
                (OrderFixture.DefaultStartPointLatitude, OrderFixture.DefaultStartPointLongitude)) +
            _geolocationServiceStub.CalculateDistanceInKmByDegrees(
                (OrderFixture.DefaultStartPointLatitude, OrderFixture.DefaultStartPointLongitude),
                (OrderFixture.DefaultEndPointLatitude, OrderFixture.DefaultEndPointLongitude)) +
            expectedBranch.CalculateDistanceInKmByDegrees(_geolocationServiceStub,
                (OrderFixture.DefaultEndPointLatitude, OrderFixture.DefaultEndPointLongitude));
        var expectedExpectedHoursWorkedByDrivers = expectedDistanceInKm / OrderFixture.AverageTruckSpeedInKmPerHour / 2;
        const int expextedActualHoursWorkedByDriver1 = 0;
        const int expextedActualHoursWorkedByDriver2 = 0;
        var expextedDateBegin = DateTime.Now;
        var expextedDateBeginError = TimeSpan.FromSeconds(10);

        // Act
        var order = OrderFixture.CreateWithTwoDriversAndHazardClassFlag(expectedUser, expectedTruck, expectedDriver1,
            expectedDriver2, _geolocationServiceStub);

        // Arrange2
        var expectedPrice = expectedTruck.CalculateOrderPrice(order);

        // Assert
        Assert.Matches(_guidRegex, order.Guid);
        Assert.Equal(expextedDateBegin, order.DateBegin, expextedDateBeginError);
        Assert.Null(order.DateEnd);
        Assert.Equal(OrderFixture.DefaultHazardClassFlag, order.HazardClassFlag);
        Assert.Equal(OrderFixture.DefaultTank, order.Tank);
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
        Assert.NotNull(order.Driver2);
        Assert.False(order.Driver2.IsAvailable);
        Assert.Equal(expectedBranch, order.Branch);
        Assert.Equal(expectedBranch.Guid, order.BranchGuid);
        Assert.Equal(expectedUser.Guid, order.UserGuid);
        Assert.Equal(OrderFixture.DefaultStartAddress, order.StartAddress);
        Assert.Equal(OrderFixture.DefaultEndAddress, order.EndAddress);
        Assert.Equal(OrderFixture.DefaultCargoDescription, order.CargoDescription);
        Assert.Equal(OrderFixture.DefaultStartPointLatitude, order.StartPointLatitude);
        Assert.Equal(OrderFixture.DefaultStartPointLongitude, order.StartPointLongitude);
        Assert.Equal(OrderFixture.DefaultEndPointLatitude, order.EndPointLatitude);
        Assert.Equal(OrderFixture.DefaultEndPointLongitude, order.EndPointLongitude);
        Assert.Equal(OrderFixture.DefaultCargoVolume, order.CargoVolume);
        Assert.Equal(OrderFixture.DefaultCargoWeight, order.CargoWeight);
    }

    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_TruckIsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
        truck.IsAvailable = false;
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_Driver1IsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        driver1.IsAvailable = false;
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_Driver2IsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        driver2.IsAvailable = false;
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_TankAndDriver1AdrQualificationOfTankIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch, adrQualificationOfTank: false);
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub, tank: true));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_TankAndDriver2AdrQualificationOfTankIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch, adrQualificationOfTank: false);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub, tank: true));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_TankIsTrueAndTruckTankIsFalse_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch, tank: false);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub, tank: true));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_TankIsFalseAndTruckTankIsTrue_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch, tank: true);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub, tank: false));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_TruckBranchAndDriver1BranchIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch1 = BranchFixture.Create();
        var branch2 = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch1);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch2);
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch1);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub));
    }

    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_TruckBranchAndDriver2BranchIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch1 = BranchFixture.Create();
        var branch2 = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch1);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch1);
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch2);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_HazardClassFlagIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub, hazardClassFlag: HazardClassesFlags.Class7 + 1));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_TruckPermittedHazardClassesFlagsIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch, permittedHazardClassessFlags: HazardClassesFlags.Class8);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch, adrQualificationFlag: AdrDriverQualificationsFlags.BaseAndClass7);
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch, adrQualificationFlag: AdrDriverQualificationsFlags.BaseAndClass7);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub, hazardClassFlag: HazardClassesFlags.Class7));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_TruckWithoutPermittedHazardClassesFlags_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_Driver1AdrQualificationFlagIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch, permittedHazardClassessFlags: HazardClassesFlags.Class7);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch, adrQualificationFlag: AdrDriverQualificationsFlags.Base);
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch, adrQualificationFlag: AdrDriverQualificationsFlags.BaseAndClass7);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub, hazardClassFlag: HazardClassesFlags.Class7));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_Driver1WithoutAdrQualificationFlag_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_Driver2AdrQualificationFlagIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch, permittedHazardClassessFlags: HazardClassesFlags.Class7);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch, adrQualificationFlag: AdrDriverQualificationsFlags.BaseAndClass7);
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch, adrQualificationFlag: AdrDriverQualificationsFlags.Base);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub, hazardClassFlag: HazardClassesFlags.Class7));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_Driver2WithoutAdrQualificationFlag_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        var driver2 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndHazardClassFlag_ArgumentsIsValid_ReturnThe100OrdersWithUniqueGuids_Test()
    {
        // Arrange
        var guids = new HashSet<string>(100);
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var order = OrderFixture.CreateWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub);
            truck.IsAvailable = true;
            driver1.IsAvailable = true;
            driver2.IsAvailable = true;

            // Assert
            Assert.DoesNotContain(order.Guid, guids);

            guids.Add(order.Guid);
        }
    }
    
    // OrderWithTwoDriversAndWithoutHazardClassFlag
    /*[Fact]
    public void Order_NewOrderWithTwoDriverAndWithoutHazardClassFlag_ArgumentsIsValid_ReturnTheOrder()
    {
        // Arrange1
        var expectedUser = CreateUser();
        var expectedBranch = CreateBranch();
        var expectedTruck = CreateTruckWithPermittedHazardClassesFlags(expectedBranch);
        var expectedDriver1 = CreateDriver1WithAdrQualificationFlag(expectedBranch);
        var expectedDriver2 = CreateDriver2WithAdrQualificationFlag(expectedBranch);
        var expectedDistanceInKm = expectedBranch.CalculateDistanceInKmByDegrees(_stubOfGeolocationService, (DefaultOrderStartPointLatitude, DefaultOrderStartPointLongitude)) + _stubOfGeolocationService.CalculateDistanceInKmByDegrees((DefaultOrderStartPointLatitude, DefaultOrderStartPointLongitude),
            (DefaultOrderEndPointLatitude, DefaultOrderEndPointLongitude)) + expectedBranch.CalculateDistanceInKmByDegrees(_stubOfGeolocationService, (DefaultOrderEndPointLatitude, DefaultOrderEndPointLongitude));
        var expectedExpectedHoursWorkedByDrivers = expectedDistanceInKm / AverageTruckSpeedInKmPerHour / 2;
        const int expextedActualHoursWorkedByDriver1 = 0;
        const int expextedActualHoursWorkedByDriver2 = 0;
        var expextedDateBegin = DateTime.Now;
        var expextedDateBeginError = TimeSpan.FromSeconds(10);
        
        // Act
        var order = CreateOrderWithTwoDriversAndWithoutHazardClassFlag(expectedUser, expectedTruck, expectedDriver1, expectedDriver2, _stubOfGeolocationService);
        
        // Arrange2
        var expectedPrice = expectedTruck.CalculateOrderPrice(order);
        
        // Assert
        Assert.Matches(_guidRegex, order.Guid);
        Assert.Equal(expextedDateBegin, order.DateBegin, expextedDateBeginError);
        Assert.Null(order.DateEnd);
        Assert.Null(order.HazardClassFlag);
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
    public void Order_NewOrderWithTwoDriverAndWithoutHazardClassFlag_TruckIsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        truck.IsAvailable = false;
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriverAndWithoutHazardClassFlag_Driver1IsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        driver1.IsAvailable = false;
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriverAndWithoutHazardClassFlag_Driver2IsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        driver2.IsAvailable = false;
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriverAndWithoutHazardClassFlag_TankAndDriver1AdrQualificationOfTankIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch, adrQualificationOfTank: false);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriverAndWithoutHazardClassFlag_TankAndDriver2AdrQualificationOfTankIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch, adrQualificationOfTank: false);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriverAndWithoutHazardClassFlag_TankIsTrueAndTruckTankIsFalse_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch, tank: false);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriverAndWithoutHazardClassFlag_TankIsFalseAndTruckTankIsTrue_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService, tank: false));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriverAndWithoutHazardClassFlag_TruckBranchAndDriver1BranchIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch1 = CreateBranch();
        var branch2 = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch1);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch2);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch1);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }

    [Fact]
    public void Order_NewOrderWithTwoDriverAndWithoutHazardClassFlag_TruckBranchAndDriver2BranchIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch1 = CreateBranch();
        var branch2 = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch1);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch1);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch2);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void New_NewOrderWithTwoDriverAndWithoutHazardClassFlag_ArgumentsIsValid_ReturnThe100OrdersWithUniqueGuids_Test()
    {
        // Arrange
        var guids = new HashSet<string>(100);
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var order = CreateOrderWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService);

            // Assert
            Assert.DoesNotContain(order.Guid, guids);

            guids.Add(order.Guid);
        }
    }
    
    // OrderWithOneDriverAndHazardClassFlag
    [Fact]
    public void Order_NewOrderWithOneDriverAndHazardClassFlag_ArgumentsIsValid_ReturnTheOrder()
    {
        // Arrange1
        var expectedUser = CreateUser();
        var expectedBranch = CreateBranch();
        var expectedTruck = CreateTruckWithPermittedHazardClassesFlags(expectedBranch);
        var expectedDriver1 = CreateDriver1WithAdrQualificationFlag(expectedBranch);
        var expectedDistanceInKm = expectedBranch.CalculateDistanceInKmByDegrees(_stubOfGeolocationService, (DefaultOrderStartPointLatitude, DefaultOrderStartPointLongitude)) + _stubOfGeolocationService.CalculateDistanceInKmByDegrees((DefaultOrderStartPointLatitude, DefaultOrderStartPointLongitude),
            (DefaultOrderEndPointLatitude, DefaultOrderEndPointLongitude)) + expectedBranch.CalculateDistanceInKmByDegrees(_stubOfGeolocationService, (DefaultOrderEndPointLatitude, DefaultOrderEndPointLongitude));
        var expectedExpectedHoursWorkedByDrivers = expectedDistanceInKm / AverageTruckSpeedInKmPerHour;
        const int expextedActualHoursWorkedByDriver1 = 0;
        const int expectedHazardClassFlag = HazardClassesFlags.Class21;
        var expextedDateBegin = DateTime.Now;
        var expextedDateBeginError = TimeSpan.FromSeconds(10);
        
        // Act
        var order = CreateOrderWithOneDriverAndHazardClassFlag(expectedUser, expectedTruck, expectedDriver1, _stubOfGeolocationService, hazardClassFlag: expectedHazardClassFlag);
        
        // Arrange2
        var expectedPrice = expectedTruck.CalculateOrderPrice(order);
        
        // Assert
        Assert.Matches(_guidRegex, order.Guid);
        Assert.Equal(expextedDateBegin, order.DateBegin, expextedDateBeginError);
        Assert.Null(order.DateEnd);
        Assert.Equal(expectedHazardClassFlag, order.HazardClassFlag);
        Assert.Equal(DefaultOrderTank, order.Tank);
        Assert.Equal(expectedDistanceInKm, order.DistanceInKm);
        Assert.Equal(expectedPrice, order.Price);
        Assert.Equal(expectedExpectedHoursWorkedByDrivers, order.ExpectedHoursWorkedByDrivers);
        Assert.Equal(expextedActualHoursWorkedByDriver1, order.ActualHoursWorkedByDriver1);
        Assert.Null(order.ActualHoursWorkedByDriver2);
        Assert.Equal(expectedUser, order.User);
        Assert.Equal(expectedUser.Guid, order.UserGuid);
        Assert.Equal(expectedTruck, order.Truck);
        Assert.Equal(expectedTruck.Guid, order.TruckGuid);
        Assert.False(order.Truck.IsAvailable);
        Assert.Equal(expectedDriver1, order.Driver1);
        Assert.Equal(expectedDriver1.Guid, order.Driver1Guid);
        Assert.False(order.Driver1.IsAvailable);
        Assert.Null(order.Driver2);
        Assert.Null(order.Driver2Guid);
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
    public void Order_NewOrderWithOneDriverAndHazardClassFlag_TruckIsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        truck.IsAvailable = false;
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithOneDriverAndHazardClassFlag(user, truck, driver1, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithOneDriverAndHazardClassFlag_Driver1IsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        driver1.IsAvailable = false;
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithOneDriverAndHazardClassFlag(user, truck, driver1, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithOneDriverAndHazardClassFlag_TankAndDriver1AdrQualificationOfTankIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch, adrQualificationOfTank: false);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithOneDriverAndHazardClassFlag(user, truck, driver1, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithOneDriverAndHazardClassFlag_TankIsTrueAndTruckTankIsFalse_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch, tank: false);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithOneDriverAndHazardClassFlag(user, truck, driver1, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithOneDriverAndHazardClassFlag_TankIsFalseAndTruckTankIsTrue_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithOneDriverAndHazardClassFlag(user, truck, driver1, _stubOfGeolocationService, tank: false));
    }
    
    [Fact]
    public void Order_NewOrderWithOneDriverAndHazardClassFlag_TruckBranchAndDriver1BranchIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch1 = CreateBranch();
        var branch2 = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch1);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch2);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithOneDriverAndHazardClassFlag(user, truck, driver1, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithOneDriverAndHazardClassFlag_HazardClassFlagIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateOrderWithOneDriverAndHazardClassFlag(user, truck, driver1, _stubOfGeolocationService, hazardClassFlag: HazardClassesFlags.Class7 + 1));
    }
    
    [Fact]
    public void Order_NewOrderWithOneDriverAndHazardClassFlag_TruckPermittedHazardClassesFlagsIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch, permittedHazardClassessFlags: HazardClassesFlags.Class22 | HazardClassesFlags.Class7);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateOrderWithOneDriverAndHazardClassFlag(user, truck, driver1, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithOneDriverAndHazardClassFlag_TruckWithoutPermittedHazardClassesFlags_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithoutPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateOrderWithOneDriverAndHazardClassFlag(user, truck, driver1, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithOneDriverAndHazardClassFlag_Driver1AdrQualificationFlagIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch, adrQualificationFlag: AdrDriverQualificationsFlags.BaseAndClass7);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateOrderWithOneDriverAndHazardClassFlag(user, truck, driver1, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithOneDriverAndHazardClassFlag_Driver1WithoutAdrQualificationFlag_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithoutAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithOneDriverAndHazardClassFlag(user, truck, driver1, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithOneDriverAndHazardClassFlag_ArgumentsIsValid_ReturnThe100OrdersWithUniqueGuids_Test()
    {
        // Arrange
        var guids = new HashSet<string>(100);
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var order = CreateOrderWithOneDriverAndHazardClassFlag(user, truck, driver1, _stubOfGeolocationService);

            // Assert
            Assert.DoesNotContain(order.Guid, guids);

            guids.Add(order.Guid);
        }
    }*/

    private readonly IGeolocationService _geolocationServiceStub = GeolocationServiceStub.Create();
    
    private readonly Regex _guidRegex = RegexFixture.GuidRegex();
}
