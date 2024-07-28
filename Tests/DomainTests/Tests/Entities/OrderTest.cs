using System.Text.RegularExpressions;
using Domain.Constants;
using Domain.Interfaces;
using DomainTests.Fixtures;
using DomainTests.Stubs;
using RegexFixture = DomainTests.Fixtures.RegexFixture;

namespace DomainTests.Tests.Entities;

public class OrderTest
{
    [Theory]
    [InlineData(null)]
    [InlineData(HazardClassesFlags.Class11)]
    public void Order_New_ArgumentsIsValid_ReturnTheOrder_Test(int? expectedHazardClassFlag)
    {
        // Arrange1
        var expectedUser = UserFixture.CreateVk();
        const string expectedStatus = OrderStatuses.AwaitingAssignmentOfPerformers;
        var expextedDateCreated = DateTime.Now;
        var expextedDateCreatedError = TimeSpan.FromSeconds(10);

        // Act
        var order = OrderFixture.Create(expectedUser, hazardClassFlag: expectedHazardClassFlag);

        // Assert
        Assert.Matches(_guidRegex, order.Guid);
        Assert.Equal(expectedStatus, order.Status);
        Assert.Equal(expextedDateCreated, order.DateCreated, expextedDateCreatedError);
        Assert.Null(order.DateAssignmentOfPerformers);
        Assert.Null(order.DatePaymentAndBegin);
        Assert.Null(order.DateEnd);
        Assert.Equal(expectedHazardClassFlag, order.HazardClassFlag);
        Assert.Equal(OrderFixture.DefaultTank, order.Tank);
        Assert.Null(order.LengthInKm);
        Assert.Null(order.Price);
        Assert.Null(order.ExpectedHoursWorkedByDrivers);
        Assert.Null(order.ActualHoursWorkedByDriver1);
        Assert.Null(order.ActualHoursWorkedByDriver2);
        Assert.Equal(expectedUser, order.User);
        Assert.Equal(expectedUser.Guid, order.UserGuid);
        Assert.Null(order.Truck);
        Assert.Null(order.TruckGuid);
        Assert.Null(order.Driver1);
        Assert.Null(order.Driver1Guid);
        Assert.Null(order.Driver2);
        Assert.Null(order.Driver2Guid);
        Assert.Null(order.Branch);
        Assert.Null(order.BranchGuid);
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
    public void Order_New_HazardClassFlagIsInvalid_ThrowArgumentOutOfRangeException_Test()
    {
        // Arrange
        const int hazardClassFlag = HazardClassesFlags.Class7 + 1;

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => OrderFixture.Create(UserFixture.CreateVk(), hazardClassFlag: hazardClassFlag));
    }
    
    [Fact]
    public void Order_New_ArgumentsIsValid_ReturnThe100OrdersWithUniqueGuids_Test()
    {
        // Arrange
        var guids = new HashSet<string>(100);
        var user = UserFixture.CreateVk();

        for (var i = 0; i < 100; i++)
        {
            // Act
            var order = OrderFixture.Create(user);

            // Assert
            Assert.DoesNotContain(order.Guid, guids);

            guids.Add(order.Guid);
        }
    }

    [Fact]
    public void
        Order_AssignPerformersWithDriver2_ContextAndArgumentsIsValid_SetTheDriver1AndDriver2AndTruckAndBranchAndLengthInKmAndExpectedHoursWorkedByDriversAndPriceAndSetTheStatusToPerformersAssignedAndSetTheDateAssignmentOfPerformersToNow_Test()
    {
        // Arrange
        var expectedBranch = BranchFixture.Create();
        var expectedTruck = TruckFixture.CreateWithPermittedHazardClassesFlags(expectedBranch);
        var expectedDriver1 = DriverFixture.CreateWithAdrQualificationFlag(expectedBranch);
        var expectedDriver2 = DriverFixture.CreateWithAdrQualificationFlag(expectedBranch);
        var order = OrderFixture.Create(UserFixture.CreateVk());
        var expectedLengthInKmAndDrivingHours =
            expectedBranch.CalculateLengthInKmOfOrderRouteClosedAtBranchAndApproximateDrivingHoursOfTruckAlongIt(order, _geolocationServiceStub);
        expectedLengthInKmAndDrivingHours.DrivingHours /= 2;
        var expectedPrice = expectedTruck.CalculateOrderPricePerKm(order) * (decimal)expectedLengthInKmAndDrivingHours.LengthInKm;
        const string expectedStatus = OrderStatuses.PerformersAssigned;
        var expextedDateAssignmentOfPerformers = DateTime.Now;
        var expextedDateAssignmentOfPerformersError = TimeSpan.FromSeconds(10);

        // Act
        order.AssignPerformers(_geolocationServiceStub, expectedTruck, expectedDriver1, expectedDriver2);

        // Assert
        Assert.Equal(expectedDriver1, order.Driver1);
        Assert.Equal(expectedDriver1.Guid, order.Driver1Guid);
        Assert.False(expectedDriver1.IsAvailable);
        Assert.Equal(expectedDriver2, order.Driver2);
        Assert.Equal(expectedDriver2.Guid, order.Driver2Guid);
        Assert.False(expectedDriver2.IsAvailable);
        Assert.Equal(expectedTruck, order.Truck);
        Assert.Equal(expectedTruck.Guid, order.TruckGuid);
        Assert.False(expectedTruck.IsAvailable);
        Assert.Equal(expectedBranch, order.Branch);
        Assert.Equal(expectedBranch.Guid, order.BranchGuid);
        Assert.Equal(expectedLengthInKmAndDrivingHours.LengthInKm, order.LengthInKm);
        Assert.Equal(expectedLengthInKmAndDrivingHours.DrivingHours, order.ExpectedHoursWorkedByDrivers);
        Assert.Equal(expectedPrice, order.Price);
        Assert.Equal(expectedStatus, order.Status);
        Assert.NotNull(order.DateAssignmentOfPerformers);
        Assert.Equal(expextedDateAssignmentOfPerformers, order.DateAssignmentOfPerformers.Value, expextedDateAssignmentOfPerformersError);
    }
    
    [Fact]
    public void
        Order_AssignPerformersWithoutDriver2_ContextAndArgumentsIsValid_SetTheDriver1AndTruckAndBranchAndLengthInKmAndExpectedHoursWorkedByDriversAndPriceAndSetTheStatusToPerformersAssignedAndSetTheDateAssignmentOfPerformersToNow_Test()
    {
        // Arrange
        var expectedBranch = BranchFixture.Create();
        var expectedTruck = TruckFixture.CreateWithPermittedHazardClassesFlags(expectedBranch);
        var expectedDriver1 = DriverFixture.CreateWithAdrQualificationFlag(expectedBranch);
        var order = OrderFixture.Create(UserFixture.CreateVk());
        var expectedLengthInKmAndDrivingHours =
            expectedBranch.CalculateLengthInKmOfOrderRouteClosedAtBranchAndApproximateDrivingHoursOfTruckAlongIt(order, _geolocationServiceStub);
        var expectedPrice = expectedTruck.CalculateOrderPricePerKm(order) * (decimal)expectedLengthInKmAndDrivingHours.LengthInKm;
        const string expectedStatus = OrderStatuses.PerformersAssigned;
        var expextedDateAssignmentOfPerformers = DateTime.Now;
        var expextedDateAssignmentOfPerformersError = TimeSpan.FromSeconds(10);

        // Act
        order.AssignPerformers(_geolocationServiceStub, expectedTruck, expectedDriver1);

        // Assert
        Assert.Equal(expectedDriver1, order.Driver1);
        Assert.Equal(expectedDriver1.Guid, order.Driver1Guid);
        Assert.False(expectedDriver1.IsAvailable);
        Assert.Null(order.Driver2);
        Assert.Null(order.Driver2Guid);
        Assert.Equal(expectedTruck, order.Truck);
        Assert.Equal(expectedTruck.Guid, order.TruckGuid);
        Assert.False(expectedTruck.IsAvailable);
        Assert.Equal(expectedBranch, order.Branch);
        Assert.Equal(expectedBranch.Guid, order.BranchGuid);
        Assert.Equal(expectedLengthInKmAndDrivingHours.LengthInKm, order.LengthInKm);
        Assert.Equal(expectedLengthInKmAndDrivingHours.DrivingHours, order.ExpectedHoursWorkedByDrivers);
        Assert.Equal(expectedPrice, order.Price);
        Assert.Equal(expectedStatus, order.Status);
        Assert.NotNull(order.DateAssignmentOfPerformers);
        Assert.Equal(expextedDateAssignmentOfPerformers, order.DateAssignmentOfPerformers.Value, expextedDateAssignmentOfPerformersError);
    }
    
    [Fact]
    public void
        Order_AssignPerformers_StatusIsInvalid_ThrowInvalidOperationException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var order = OrderFixture.Create(UserFixture.CreateVk());
        order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch), DriverFixture.CreateWithAdrQualificationFlag(branch));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch), DriverFixture.CreateWithAdrQualificationFlag(branch)));
    }
    
    [Fact]
    public void
        Order_AssignPerformers_Driver1AndDriver2IsInvalid_ThrowArgumentException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var driver = DriverFixture.CreateWithAdrQualificationFlag(branch);
        var order = OrderFixture.Create(UserFixture.CreateVk());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch), driver, driver));
    }
    
    [Fact]
    public void
        Order_AssignPerformers_Driver1IsAvailableIsInvalid_ThrowArgumentException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        driver1.IsAvailable = false;
        var order = OrderFixture.Create(UserFixture.CreateVk());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch), driver1));
    }
    
    [Fact]
    public void
        Order_AssignPerformers_Driver2IsAvailableIsInvalid_ThrowArgumentException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch);
        driver2.IsAvailable = false;
        var order = OrderFixture.Create(UserFixture.CreateVk());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch), DriverFixture.CreateWithAdrQualificationFlag(branch), driver2));
    }
    
    [Fact]
    public void
        Order_AssignPerformers_TruckIsAvailableIsInvalid_ThrowArgumentException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var truck = TruckFixture.CreateWithPermittedHazardClassesFlags(branch);
        truck.IsAvailable = false;
        var order = OrderFixture.Create(UserFixture.CreateVk());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => order.AssignPerformers(_geolocationServiceStub, truck, DriverFixture.CreateWithAdrQualificationFlag(branch)));
    }
    
    [Fact]
    public void
        Order_AssignPerformers_TankAndDriver1AdrQualificationOfTankIsInvalid_ThrowArgumentException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var driver1 = DriverFixture.CreateWithAdrQualificationFlag(branch, adrQualificationOfTank: false);
        var order = OrderFixture.Create(UserFixture.CreateVk(), tank: true);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch), driver1));
    }
    
    [Fact]
    public void
        Order_AssignPerformers_TankAndDriver2AdrQualificationOfTankIsInvalid_ThrowArgumentException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var driver2 = DriverFixture.CreateWithAdrQualificationFlag(branch, adrQualificationOfTank: false);
        var order = OrderFixture.Create(UserFixture.CreateVk(), tank: true);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch), DriverFixture.CreateWithAdrQualificationFlag(branch), driver2));
    }
    
    [Fact]
    public void
        Order_AssignPerformers_TankIsTrueAndTruckTankIsFalse_ThrowArgumentException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var order = OrderFixture.Create(UserFixture.CreateVk(), tank: true);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch, tank: false), DriverFixture.CreateWithAdrQualificationFlag(branch)));
    }
    
    [Fact]
    public void
        Order_AssignPerformers_TankIsFalseAndTruckTankIsTrue_ThrowArgumentException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var order = OrderFixture.Create(UserFixture.CreateVk(), tank: false);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch, tank: true), DriverFixture.CreateWithAdrQualificationFlag(branch)));
    }
    
    [Fact]
    public void
        Order_AssignPerformers_Driver1BranchIsInvalid_ThrowArgumentException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var order = OrderFixture.Create(UserFixture.CreateVk());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch), DriverFixture.CreateWithAdrQualificationFlag(BranchFixture.Create())));
    }
    
    [Fact]
    public void
        Order_AssignPerformers_Driver2BranchIsInvalid_ThrowArgumentException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var order = OrderFixture.Create(UserFixture.CreateVk());

        // Act & Assert
        Assert.Throws<ArgumentException>(() => order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch), DriverFixture.CreateWithAdrQualificationFlag(branch), DriverFixture.CreateWithAdrQualificationFlag(BranchFixture.Create())));
    }
    
    [Fact]
    public void
        Order_AssignPerformers_TruckPermittedHazardClassesFlagsIsInvalid_ThrowArgumentOutOfRangeException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var order = OrderFixture.Create(UserFixture.CreateVk(), hazardClassFlag: HazardClassesFlags.Class7);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch, permittedHazardClassessFlags: HazardClassesFlags.Class8), DriverFixture.CreateWithAdrQualificationFlag(branch)));
    }
    
    [Fact]
    public void
        Order_AssignPerformers_Driver1AdrQualificationFlagIsInvalid_ThrowArgumentOutOfRangeException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var order = OrderFixture.Create(UserFixture.CreateVk(), hazardClassFlag: HazardClassesFlags.Class7);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch, permittedHazardClassessFlags: HazardClassesFlags.Class7), DriverFixture.CreateWithAdrQualificationFlag(branch, adrQualificationFlag: AdrDriverQualificationsFlags.Base)));
    }
    
    [Fact]
    public void
        Order_AssignPerformers_Driver2AdrQualificationFlagIsInvalid_ThrowArgumentOutOfRangeException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var order = OrderFixture.Create(UserFixture.CreateVk(), hazardClassFlag: HazardClassesFlags.Class7);

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch, permittedHazardClassessFlags: HazardClassesFlags.Class7), DriverFixture.CreateWithAdrQualificationFlag(branch, adrQualificationFlag: AdrDriverQualificationsFlags.BaseAndClass7), DriverFixture.CreateWithAdrQualificationFlag(branch, adrQualificationFlag: AdrDriverQualificationsFlags.Base)));
    }
    
    [Fact]
    public void Order_ConfirmPaymentAndBegin_ContextIsValid_SetTheStatusToInProgressAndSetTheDatePaymentAndBeginToNow_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var order = OrderFixture.Create(UserFixture.CreateVk());
        order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch), DriverFixture.CreateWithAdrQualificationFlag(branch));
        const string expectedStatus = OrderStatuses.InProgress;
        var expextedDatePaymentAndBegin = DateTime.Now;
        var expextedDatePaymentAndBeginError = TimeSpan.FromSeconds(10);
        
        // Act
        order.ConfirmPaymentAndBegin();

        // Assert
        Assert.Equal(expectedStatus, order.Status);
        Assert.NotNull(order.DatePaymentAndBegin);
        Assert.Equal(expextedDatePaymentAndBegin, order.DatePaymentAndBegin.Value, expextedDatePaymentAndBeginError);
    }
    
    [Fact]
    public void Order_ConfirmPaymentAndBegin_StatusIsInvalid_ThrowInvalidOperationException_Test()
    {
        // Arrange
        var order = OrderFixture.Create(UserFixture.CreateVk());

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.ConfirmPaymentAndBegin());
    }

    [Fact]
    public void Order_Finish_ContextAndArgumentsIsValid_SetTheTruckIsAvailableAndDriver1IsAvailableAndDriver2IsAvailableToTrueAndSetTheActualHoursWorkedByDriver1AndActualHoursWorkedByDriver2AndCallTheDriver1AddHoursWorkedAndDriver2AddHoursWorkedAndSetTheStatusToCompletedAndSetTheDateEndToNow_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var order = OrderFixture.Create(UserFixture.CreateVk());
        order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch), DriverFixture.CreateWithAdrQualificationFlag(branch), DriverFixture.CreateWithAdrQualificationFlag(branch));
        order.ConfirmPaymentAndBegin();
        const int expectedHoursWorked1 = 13;
        const int expectedHoursWorked2 = 17;
        const string expectedStatus = OrderStatuses.Completed;
        var expextedDateEnd = DateTime.Now;
        var expextedDateEndError = TimeSpan.FromSeconds(10);
        
        // Act
        order.Finish(expectedHoursWorked1, expectedHoursWorked2);

        // Assert
        Assert.True(order.Truck!.IsAvailable);
        Assert.True(order.Driver1!.IsAvailable);
        Assert.Equal(expectedHoursWorked1, order.Driver1.HoursWorkedPerWeek);
        Assert.Equal(expectedHoursWorked1, order.Driver1.TotalHoursWorked);
        Assert.Equal(expectedHoursWorked1, order.ActualHoursWorkedByDriver1);
        Assert.True(order.Driver2!.IsAvailable);
        Assert.Equal(expectedHoursWorked2, order.Driver2.HoursWorkedPerWeek);
        Assert.Equal(expectedHoursWorked2, order.Driver2.TotalHoursWorked);
        Assert.Equal(expectedHoursWorked2, order.ActualHoursWorkedByDriver2);
        Assert.Equal(expectedStatus, order.Status);
        Assert.Equal(expextedDateEnd, order.DateEnd!.Value, expextedDateEndError);
    }
    
    [Fact]
    public void Order_Finish_StatusIsInvalid_ThrowInvalidOperationException_Test()
    {
        // Arrange
        var order = OrderFixture.Create(UserFixture.CreateVk());

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => order.Finish(11));
    }
    
    [Fact]
    public void Order_Finish_Driver2IsNotNullAndActualHoursWorkedByDriver2IsNull_ThrowArgumentException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var order = OrderFixture.Create(UserFixture.CreateVk());
        order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch), DriverFixture.CreateWithAdrQualificationFlag(branch), DriverFixture.CreateWithAdrQualificationFlag(branch));
        order.ConfirmPaymentAndBegin();
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => order.Finish(11));
    }
    
    [Fact]
    public void Order_Finish_Driver2IsNullAndActualHoursWorkedByDriver2IsNotNull_ThrowArgumentException_Test()
    {
        // Arrange
        var branch = BranchFixture.Create();
        var order = OrderFixture.Create(UserFixture.CreateVk());
        order.AssignPerformers(_geolocationServiceStub, TruckFixture.CreateWithPermittedHazardClassesFlags(branch), DriverFixture.CreateWithAdrQualificationFlag(branch));
        order.ConfirmPaymentAndBegin();
        
        // Act & Assert
        Assert.Throws<ArgumentException>(() => order.Finish(11, 12));
    }

    private readonly IGeolocationService _geolocationServiceStub = GeolocationServiceStub.Create();
    
    private readonly Regex _guidRegex = RegexFixture.GuidRegex();
}