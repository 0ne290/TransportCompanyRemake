using Domain.Constants;

namespace DomainTests.Tests.Entities;

public class OrderTest
{
    // OrderWithTwoDriversAndHazardClassFlag
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_ArgumentsIsValid_ReturnTheOrder()
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
        const int expectedHazardClassFlag = HazardClassesFlags.Class21;
        var expextedDateBegin = DateTime.Now;
        var expextedDateBeginError = TimeSpan.FromSeconds(10);
        
        // Act
        var order = CreateOrderWithTwoDriversAndHazardClassFlag(expectedUser, expectedTruck, expectedDriver1, expectedDriver2, _stubOfGeolocationService, hazardClassFlag: expectedHazardClassFlag);
        
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
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        truck.IsAvailable = false;
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_Driver1IsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        driver1.IsAvailable = false;
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_Driver2IsAvailableIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        driver2.IsAvailable = false;
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_TankAndDriver1AdrQualificationOfTankIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch, adrQualificationOfTank: false);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_TankAndDriver2AdrQualificationOfTankIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch, adrQualificationOfTank: false);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_TankIsTrueAndTruckTankIsFalse_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch, tank: false);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_TankIsFalseAndTruckTankIsTrue_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService, tank: false));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_TruckBranchAndDriver1BranchIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch1 = CreateBranch();
        var branch2 = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch1);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch2);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch1);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }

    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_TruckBranchAndDriver2BranchIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch1 = CreateBranch();
        var branch2 = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch1);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch1);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch2);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_HazardClassFlagIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService, hazardClassFlag: HazardClassesFlags.Class7 + 1));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_TruckPermittedHazardClassesFlagsIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch, permittedHazardClassessFlags: HazardClassesFlags.Class22 | HazardClassesFlags.Class7);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_TruckWithoutPermittedHazardClassesFlags_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithoutPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_Driver1AdrQualificationFlagIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch, adrQualificationFlag: AdrDriverQualificationsFlags.BaseAndClass7);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_Driver1WithoutAdrQualificationFlag_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithoutAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_Driver2AdrQualificationFlagIsInvalid_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithAdrQualificationFlag(branch, adrQualificationFlag: AdrDriverQualificationsFlags.BaseAndClass7);
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_Driver2WithoutAdrQualificationFlag_ThrowArgumentException()
    {
        // Arrange
        var user = CreateUser();
        var branch = CreateBranch();
        var truck = CreateTruckWithPermittedHazardClassesFlags(branch);
        var driver1 = CreateDriver1WithAdrQualificationFlag(branch);
        var driver2 = CreateDriver2WithoutAdrQualificationFlag(branch);
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService));
    }
    
    [Fact]
    public void Order_NewOrderWithTwoDriversAndHazardClassFlag_ArgumentsIsValid_ReturnThe100OrdersWithUniqueGuids_Test()
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
            var order = CreateOrderWithTwoDriversAndHazardClassFlag(user, truck, driver1, driver2, _stubOfGeolocationService);

            // Assert
            Assert.DoesNotContain(order.Guid, guids);

            guids.Add(order.Guid);
        }
    }
    
    // OrderWithTwoDriversAndWithoutHazardClassFlag
    [Fact]
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
    }
}
