using System.Text.RegularExpressions;
using Domain.Constants;
using Domain.Interfaces;
using DomainTests.Fixtures;
using DomainTests.Stubs;
using RegexFixture = DomainTests.Fixtures.RegexFixture;

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
        Assert.NotNull(order.Driver2);
        Assert.Equal(expectedDriver2, order.Driver2);
        Assert.Equal(expectedDriver2.Guid, order.Driver2Guid);
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
    [Fact]
    public void Order_NewWithTwoDriversAndWithoutHazardClassFlag_ArgumentsIsValid_ReturnTheOrder()
    {
        // Arrange1
        var expectedUser = UserFixture.CreateVk();
        var expectedBranch = BranchFixture.Create();
        var expectedTruck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(expectedBranch);
        var expectedDriver1 = DriverFixture.CreateWithoutAdrQualificationFlag(expectedBranch);
        var expectedDriver2 = DriverFixture.CreateWithoutAdrQualificationFlag(expectedBranch);
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
        var order = OrderFixture.CreateWithTwoDriversAndWithoutHazardClassFlag(expectedUser, expectedTruck, expectedDriver1,
            expectedDriver2, _geolocationServiceStub);

        // Arrange2
        var expectedPrice = expectedTruck.CalculateOrderPrice(order);

        // Assert
        Assert.Matches(_guidRegex, order.Guid);
        Assert.Equal(expextedDateBegin, order.DateBegin, expextedDateBeginError);
        Assert.Null(order.DateEnd);
        Assert.Null(order.HazardClassFlag);
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
        Assert.NotNull(order.Driver2);
        Assert.Equal(expectedDriver2, order.Driver2);
        Assert.Equal(expectedDriver2.Guid, order.Driver2Guid);
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
    public void Order_NewWithTwoDriversAndWithoutHazardClassFlag_TruckIsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch);
        truck.IsAvailable = false;
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        var driver2 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndWithoutHazardClassFlag_Driver1IsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        driver1.IsAvailable = false;
        var driver2 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndWithoutHazardClassFlag_Driver2IsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        var driver2 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        driver2.IsAvailable = false;
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndWithoutHazardClassFlag_TankIsTrueAndTruckTankIsFalse_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch, tank: false);
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        var driver2 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub, tank: true));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndWithoutHazardClassFlag_TankIsFalseAndTruckTankIsTrue_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch, tank: true);
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        var driver2 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub, tank: false));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndWithoutHazardClassFlag_TruckBranchAndDriver1BranchIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch1 = BranchFixture.Create();
        var branch2 = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch1);
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch2);
        var driver2 = DriverFixture.CreateWithoutAdrQualificationFlag(branch1);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub));
    }

    [Fact]
    public void Order_NewWithTwoDriversAndWithoutHazardClassFlag_TruckBranchAndDriver2BranchIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch1 = BranchFixture.Create();
        var branch2 = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch1);
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch1);
        var driver2 = DriverFixture.CreateWithoutAdrQualificationFlag(branch2);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithTwoDriversAndWithoutHazardClassFlag_ArgumentsIsValid_ReturnThe100OrdersWithUniqueGuids_Test()
    {
        // Arrange
        var guids = new HashSet<string>(100);
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        var driver2 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var order = OrderFixture.CreateWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2, _geolocationServiceStub);
            truck.IsAvailable = true;
            driver1.IsAvailable = true;
            driver2.IsAvailable = true;

            // Assert
            Assert.DoesNotContain(order.Guid, guids);

            guids.Add(order.Guid);
        }
    }
    
    // OrderWithOneDriverAndHazardClassFlag
    [Fact]
    public void Order_NewWithOneDriverAndHazardClassFlag_ArgumentsIsValid_ReturnTheOrder()
    {
        // Arrange1
        var expectedUser = UserFixture.CreateVk();
        var expectedBranch = BranchFixture.Create();
        var expectedTruck = TruckFixture.CreateWithPermittedHazardClassesFlags(expectedBranch);
        var expectedDriver1 = DriverFixture.CreateWithAdrQualificationFlag(expectedBranch);
        var expectedDistanceInKm =
            expectedBranch.CalculateDistanceInKmByDegrees(_geolocationServiceStub,
                (OrderFixture.DefaultStartPointLatitude, OrderFixture.DefaultStartPointLongitude)) +
            _geolocationServiceStub.CalculateDistanceInKmByDegrees(
                (OrderFixture.DefaultStartPointLatitude, OrderFixture.DefaultStartPointLongitude),
                (OrderFixture.DefaultEndPointLatitude, OrderFixture.DefaultEndPointLongitude)) +
            expectedBranch.CalculateDistanceInKmByDegrees(_geolocationServiceStub,
                (OrderFixture.DefaultEndPointLatitude, OrderFixture.DefaultEndPointLongitude));
        var expectedExpectedHoursWorkedByDrivers = expectedDistanceInKm / OrderFixture.AverageTruckSpeedInKmPerHour;
        const int expextedActualHoursWorkedByDriver1 = 0;
        var expextedDateBegin = DateTime.Now;
        var expextedDateBeginError = TimeSpan.FromSeconds(10);

        // Act
        var order = OrderFixture.CreateWithOneDriverAndHazardClassFlag(expectedUser, expectedTruck, expectedDriver1, _geolocationServiceStub);

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
    public void Order_NewWithOneDriverAndHazardClassFlag_TruckIsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
        truck.IsAvailable = false;
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithOneDriverAndHazardClassFlag(user, truck, driver1, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithOneDriverAndHazardClassFlag_Driver1IsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        driver1.IsAvailable = false;
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithOneDriverAndHazardClassFlag(user, truck, driver1, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithOneDriverAndHazardClassFlag_TankAndDriver1AdrQualificationOfTankIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch, adrQualificationOfTank: false);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithOneDriverAndHazardClassFlag(user, truck, driver1, _geolocationServiceStub, tank: true));
    }
    
    [Fact]
    public void Order_NewWithOneDriverAndHazardClassFlag_TankIsTrueAndTruckTankIsFalse_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch, tank: false);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithOneDriverAndHazardClassFlag(user, truck, driver1, _geolocationServiceStub, tank: true));
    }
    
    [Fact]
    public void Order_NewWithOneDriverAndHazardClassFlag_TankIsFalseAndTruckTankIsTrue_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch, tank: true);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithOneDriverAndHazardClassFlag(user, truck, driver1, _geolocationServiceStub, tank: false));
    }
    
    [Fact]
    public void Order_NewWithOneDriverAndHazardClassFlag_TruckBranchAndDriver1BranchIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch1 = BranchFixture.Create();
        var branch2 = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch1);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch2);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithOneDriverAndHazardClassFlag(user, truck, driver1, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithOneDriverAndHazardClassFlag_HazardClassFlagIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => OrderFixture.CreateWithOneDriverAndHazardClassFlag(user, truck, driver1, _geolocationServiceStub, hazardClassFlag: HazardClassesFlags.Class7 + 1));
    }
    
    [Fact]
    public void Order_NewWithOneDriverAndHazardClassFlag_TruckPermittedHazardClassesFlagsIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch, permittedHazardClassessFlags: HazardClassesFlags.Class8);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch, adrQualificationFlag: AdrDriverQualificationsFlags.BaseAndClass7);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => OrderFixture.CreateWithOneDriverAndHazardClassFlag(user, truck, driver1, _geolocationServiceStub, hazardClassFlag: HazardClassesFlags.Class7));
    }
    
    [Fact]
    public void Order_NewWithOneDriverAndHazardClassFlag_TruckWithoutPermittedHazardClassesFlags_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => OrderFixture.CreateWithOneDriverAndHazardClassFlag(user, truck, driver1, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithOneDriverAndHazardClassFlag_Driver1AdrQualificationFlagIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch, permittedHazardClassessFlags: HazardClassesFlags.Class7);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch, adrQualificationFlag: AdrDriverQualificationsFlags.Base);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => OrderFixture.CreateWithOneDriverAndHazardClassFlag(user, truck, driver1, _geolocationServiceStub, hazardClassFlag: HazardClassesFlags.Class7));
    }
    
    [Fact]
    public void Order_NewWithOneDriverAndHazardClassFlag_Driver1WithoutAdrQualificationFlag_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithOneDriverAndHazardClassFlag(user, truck, driver1, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithOneDriverAndHazardClassFlag_ArgumentsIsValid_ReturnThe100OrdersWithUniqueGuids_Test()
    {
        // Arrange
        var guids = new HashSet<string>(100);
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var order = OrderFixture.CreateWithOneDriverAndHazardClassFlag(user, truck, driver1, _geolocationServiceStub);
            truck.IsAvailable = true;
            driver1.IsAvailable = true;

            // Assert
            Assert.DoesNotContain(order.Guid, guids);

            guids.Add(order.Guid);
        }
    }
    
    // OrderWithOneDriverAndWithoutHazardClassFlag
    [Fact]
    public void Order_NewWithOneDriverAndWithoutHazardClassFlag_ArgumentsIsValid_ReturnTheOrder()
    {
        // Arrange1
        var expectedUser = UserFixture.CreateVk();
        var expectedBranch = BranchFixture.Create();
        var expectedTruck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(expectedBranch);
        var expectedDriver1 = DriverFixture.CreateWithoutAdrQualificationFlag(expectedBranch);
        var expectedDistanceInKm =
            expectedBranch.CalculateDistanceInKmByDegrees(_geolocationServiceStub,
                (OrderFixture.DefaultStartPointLatitude, OrderFixture.DefaultStartPointLongitude)) +
            _geolocationServiceStub.CalculateDistanceInKmByDegrees(
                (OrderFixture.DefaultStartPointLatitude, OrderFixture.DefaultStartPointLongitude),
                (OrderFixture.DefaultEndPointLatitude, OrderFixture.DefaultEndPointLongitude)) +
            expectedBranch.CalculateDistanceInKmByDegrees(_geolocationServiceStub,
                (OrderFixture.DefaultEndPointLatitude, OrderFixture.DefaultEndPointLongitude));
        var expectedExpectedHoursWorkedByDrivers = expectedDistanceInKm / OrderFixture.AverageTruckSpeedInKmPerHour;
        const int expextedActualHoursWorkedByDriver1 = 0;
        var expextedDateBegin = DateTime.Now;
        var expextedDateBeginError = TimeSpan.FromSeconds(10);

        // Act
        var order = OrderFixture.CreateWithOneDriverAndWithoutHazardClassFlag(expectedUser, expectedTruck, expectedDriver1, _geolocationServiceStub);

        // Arrange2
        var expectedPrice = expectedTruck.CalculateOrderPrice(order);

        // Assert
        Assert.Matches(_guidRegex, order.Guid);
        Assert.Equal(expextedDateBegin, order.DateBegin, expextedDateBeginError);
        Assert.Null(order.DateEnd);
        Assert.Null(order.HazardClassFlag);
        Assert.Equal(OrderFixture.DefaultTank, order.Tank);
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
    public void Order_NewWithOneDriverAndWithoutHazardClassFlag_TruckIsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch);
        truck.IsAvailable = false;
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithOneDriverAndWithoutHazardClassFlag(user, truck, driver1, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithOneDriverAndWithoutHazardClassFlag_Driver1IsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        driver1.IsAvailable = false;
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithOneDriverAndWithoutHazardClassFlag(user, truck, driver1, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithOneDriverAndWithoutHazardClassFlag_TankIsTrueAndTruckTankIsFalse_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch, tank: false);
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithOneDriverAndWithoutHazardClassFlag(user, truck, driver1, _geolocationServiceStub, tank: true));
    }
    
    [Fact]
    public void Order_NewWithOneDriverAndWithoutHazardClassFlag_TankIsFalseAndTruckTankIsTrue_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch, tank: true);
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithOneDriverAndWithoutHazardClassFlag(user, truck, driver1, _geolocationServiceStub, tank: false));
    }
    
    [Fact]
    public void Order_NewWithOneDriverAndWithoutHazardClassFlag_TruckBranchAndDriver1BranchIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = UserFixture.CreateVk();
        var branch1 = BranchFixture.Create();
        var branch2 = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch1);
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch2);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => OrderFixture.CreateWithOneDriverAndWithoutHazardClassFlag(user, truck, driver1, _geolocationServiceStub));
    }
    
    [Fact]
    public void Order_NewWithOneDriverAndWithoutHazardClassFlag_ArgumentsIsValid_ReturnThe100OrdersWithUniqueGuids_Test()
    {
        // Arrange
        var guids = new HashSet<string>(100);
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);

        for (var i = 0; i < 100; i++)
        {
            // Act
            var order = OrderFixture.CreateWithOneDriverAndWithoutHazardClassFlag(user, truck, driver1, _geolocationServiceStub);
            truck.IsAvailable = true;
            driver1.IsAvailable = true;

            // Assert
            Assert.DoesNotContain(order.Guid, guids);

            guids.Add(order.Guid);
        }
    }

    [Fact]
    public void
        Order_FinishWithTwoDrivers_ContextAndArgumentsIsValid_SetTheDateEndToNowAndSetTheActualHoursWorkedByDriver1AndActualHoursWorkedByDriver2AndExecuteTheDriver1AddHoursWorkedAndDriver2AddHoursWorkedAndSetTheTruckIsAvailableAndDriver1IsAvailableAndDriver2IsAvailableToTrue_Test()
    {
        // Arrange
        const double expectedActualHoursWorkedByDriver1 = 17;
        const double expectedActualHoursWorkedByDriver2 = 15;
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        var driver2 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        var order = OrderFixture.CreateWithTwoDriversAndWithoutHazardClassFlag(user, truck, driver1, driver2,
            _geolocationServiceStub);
        var expextedDateEnd = DateTime.Now;
        var expextedDateEndError = TimeSpan.FromSeconds(10);
        
        // Act
        order.Finish(expectedActualHoursWorkedByDriver1, expectedActualHoursWorkedByDriver2);
        
        // Assert
        Assert.NotNull(order.DateEnd);
        Assert.Equal(expextedDateEnd, order.DateEnd.Value, expextedDateEndError);
        Assert.Equal(expectedActualHoursWorkedByDriver1, order.ActualHoursWorkedByDriver1);
        Assert.Equal(expectedActualHoursWorkedByDriver2, order.ActualHoursWorkedByDriver2);
        Assert.Equal(expectedActualHoursWorkedByDriver1, driver1.HoursWorkedPerWeek);
        Assert.Equal(expectedActualHoursWorkedByDriver1, driver1.TotalHoursWorked);
        Assert.Equal(expectedActualHoursWorkedByDriver2, driver2.HoursWorkedPerWeek);
        Assert.Equal(expectedActualHoursWorkedByDriver2, driver2.TotalHoursWorked);
        Assert.True(driver1.IsAvailable);
        Assert.True(driver2.IsAvailable);
        Assert.True(truck.IsAvailable);
    }
    
    [Fact]
    public void Order_FinishWithTwoDrivers_Driver2IsInvalid_ThrowInvalidOperationException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var order = OrderFixture.CreateWithOneDriverAndWithoutHazardClassFlag(UserFixture.CreateVk(), TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch), DriverFixture.CreateWithoutAdrQualificationFlag(branch), _geolocationServiceStub);
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Finish(5, 2));
    }
    
    [Fact]
    public void Order_FinishWithTwoDrivers_DateEndIsInvalid_ThrowInvalidOperationException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var order = OrderFixture.CreateWithTwoDriversAndWithoutHazardClassFlag(UserFixture.CreateVk(), TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch), DriverFixture.CreateWithoutAdrQualificationFlag(branch), DriverFixture.CreateWithoutAdrQualificationFlag(branch), _geolocationServiceStub);
        order.Finish(5, 2);
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Finish(5, 2));
    }
    
    [Fact]
    public void
        Order_FinishWithOneDriver_ContextAndArgumentIsValid_SetTheDateEndToNowAndSetTheActualHoursWorkedByDriver1AndExecuteTheDriver1AddHoursWorkedAndSetTheTruckIsAvailableAndDriver1IsAvailableToTrue_Test()
    {
        // Arrange
        const double expectedActualHoursWorkedByDriver1 = 21;
        var user = UserFixture.CreateVk();
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch);
        var driver1 = DriverFixture.CreateWithoutAdrQualificationFlag(branch);
        var order = OrderFixture.CreateWithOneDriverAndWithoutHazardClassFlag(user, truck, driver1, _geolocationServiceStub);
        var expextedDateEnd = DateTime.Now;
        var expextedDateEndError = TimeSpan.FromSeconds(10);
        
        // Act
        order.Finish(expectedActualHoursWorkedByDriver1);
        
        // Assert
        Assert.NotNull(order.DateEnd);
        Assert.Equal(expextedDateEnd, order.DateEnd.Value, expextedDateEndError);
        Assert.Equal(expectedActualHoursWorkedByDriver1, order.ActualHoursWorkedByDriver1);
        Assert.Equal(expectedActualHoursWorkedByDriver1, driver1.HoursWorkedPerWeek);
        Assert.Equal(expectedActualHoursWorkedByDriver1, driver1.TotalHoursWorked);
        Assert.True(driver1.IsAvailable);
        Assert.True(truck.IsAvailable);
    }
    
    [Fact]
    public void Order_FinishWithOneDriver_Driver2IsInvalid_ThrowInvalidOperationException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var order = OrderFixture.CreateWithTwoDriversAndWithoutHazardClassFlag(UserFixture.CreateVk(), TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch), DriverFixture.CreateWithoutAdrQualificationFlag(branch), DriverFixture.CreateWithoutAdrQualificationFlag(branch), _geolocationServiceStub);
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Finish(5));
    }
    
    [Fact]
    public void Order_FinishWithOneDriver_DateEndIsInvalid_ThrowInvalidOperationException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var order = OrderFixture.CreateWithOneDriverAndWithoutHazardClassFlag(UserFixture.CreateVk(), TruckFixture.CreateWithoutPermittedHazardClassesFlags(branch), DriverFixture.CreateWithoutAdrQualificationFlag(branch), _geolocationServiceStub);
        order.Finish(5);
        
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Finish(5));
    }

    private readonly IGeolocationService _geolocationServiceStub = GeolocationServiceStub.Create();
    
    private readonly Regex _guidRegex = RegexFixture.GuidRegex();
}
