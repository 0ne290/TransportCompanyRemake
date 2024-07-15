using System.Text.RegularExpressions;
using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace DomainTests.Tests.Entities;

public partial class OrderTest
{
    [Fact]
    public void Order_New_ArgumentsIsValid_ReturnTheOrderWithTwoDriversAndHazardClassFlag()
    {
        // Arrange1
        var guidRegex = GuidRegex();
        
        var mock = new Mock<IGeolocationService>();
        mock.Setup(gs => gs.CalculateDistanceInKmByDegrees(It.IsAny<ValueTuple<double, double>>(),
            It.IsAny<ValueTuple<double, double>>())).Returns(StubOfCalculateDistanceInKmByDegrees);

        var stubOfGeolocationService = mock.Object;
        
        const string expectedStartAddress = "AnyStartAddress";
        const string expectedEndAddress = "AnyEndAddress";
        const string expectedDescription = "AnyDescription";
        const double expectedStartPointLatitude = 56.9;
        const double expectedStartPointLongitude = 4.8;
        const int expectedEndPointLatitude = -9;
        const int expectedEndPointLongitude = 8;
        const int expectedCargoVolume = 60;
        const int expectedCargoWeight = 6000;
        const int expectedHazardClassFlag = HazardClassesFlags.Class21;
        const bool expectedTank = true;
        var expectedUser = User.New("AnyName", "AnyContact", 364);
        var expectedBranch = Branch.New("AnyAddress", (34, 75));
        var expectedTruck = Truck.New("AnyNumber", true, 78, 1.2m, 17000, 0.15m, 1,
            HazardClassesFlags.Class21 | HazardClassesFlags.Class22 | HazardClassesFlags.Class23, expectedBranch);
        var expectedDriver1 = Driver.New("AnyDriver1Name", AdrDriverQualificationsFlags.Base, true, expectedBranch);
        var expectedDriver2 = Driver.New("AnyDriver2Name", AdrDriverQualificationsFlags.Base, true, expectedBranch);
        var expectedDistanceInKm = expectedBranch.CalculateDistanceInKmByDegrees(stubOfGeolocationService, (expectedStartPointLatitude, expectedStartPointLongitude)) + stubOfGeolocationService.CalculateDistanceInKmByDegrees((expectedStartPointLatitude, expectedStartPointLongitude),
            (expectedEndPointLatitude, expectedEndPointLongitude)) + expectedBranch.CalculateDistanceInKmByDegrees(stubOfGeolocationService, (expectedEndPointLatitude, expectedEndPointLongitude));
        var expectedExpectedHoursWorkedByDrivers = expectedDistanceInKm / AverageTruckSpeedInKmPerHour / 2;
        var expextedActualHoursWorkedByDriver1 = 0;
        var expextedActualHoursWorkedByDriver2 = 0;
        var expextedDateBegin = DateTime.Now;
        var expextedDateBeginError = TimeSpan.FromSeconds(10);
        
        // Act
        var order = Order.New(expectedStartAddress, expectedEndAddress, expectedDescription,
            (expectedStartPointLatitude, expectedStartPointLongitude),
            (expectedEndPointLatitude, expectedEndPointLongitude), expectedCargoVolume, expectedCargoWeight,
            expectedHazardClassFlag, expectedTank, expectedUser, expectedTruck, expectedDriver1, expectedDriver2,
            stubOfGeolocationService);
        
        // Arrange2
        var expectedPrice = expectedTruck.CalculateOrderPrice(order);
        
        // Assert
        Assert.Matches(guidRegex, order.Guid);
        Assert.Equal(expextedDateBegin, order.DateBegin, expextedDateBeginError);
        Assert.Null(order.DateEnd);
        Assert.Equal(expectedHazardClassFlag, order.HazardClassFlag);
        Assert.Equal(expectedTank, order.Tank);
        Assert.Equal(expectedDistanceInKm, order.DistanceInKm);
        Assert.Equal(expectedPrice, order.Price);
        Assert.Equal(expectedExpectedHoursWorkedByDrivers, order.ExpectedHoursWorkedByDrivers);
        Assert.Equal(expextedActualHoursWorkedByDriver1, order.ActualHoursWorkedByDriver1);
        Assert.Equal(expextedActualHoursWorkedByDriver2, order.ActualHoursWorkedByDriver2);
        Assert.Equal(expectedUser, order.User);
        Assert.Equal(expectedUser.Guid, order.UserGuid);
        Assert.Equal(expectedTruck, order.Truck);
        Assert.Equal(expectedTruck.Guid, order.TruckGuid);
        Assert.Equal(expectedDriver1, order.Driver1);
        Assert.Equal(expectedDriver1.Guid, order.Driver1Guid);
        Assert.Equal(expectedDriver2, order.Driver2);
        Assert.Equal(expectedDriver2.Guid, order.Driver2Guid);
        Assert.Equal(expectedBranch, order.Branch);
        Assert.Equal(expectedBranch.Guid, order.BranchGuid);
        Assert.Equal(expectedUser.Guid, order.UserGuid);
        Assert.Equal(expectedStartAddress, order.StartAddress);
        Assert.Equal(expectedEndAddress, order.EndAddress);
        Assert.Equal(expectedDescription, order.CargoDescription);
        Assert.Equal(expectedStartPointLatitude, order.StartPointLatitude);
        Assert.Equal(expectedStartPointLongitude, order.StartPointLongitude);
        Assert.Equal(expectedEndPointLatitude, order.EndPointLatitude);
        Assert.Equal(expectedEndPointLongitude, order.EndPointLongitude);
        Assert.Equal(expectedCargoVolume, order.CargoVolume);
        Assert.Equal(expectedCargoWeight, order.CargoWeight);
        return;
        
        double StubOfCalculateDistanceInKmByDegrees((double Latitude, double Longitude) point1,
            (double Latitude, double Longitude) point2) =>
            point1.Latitude + point1.Longitude + point2.Latitude + point2.Longitude;
    }
    
    [GeneratedRegex(@"^(?i)[a-z\d]{8}-([a-z\d]{4}-){3}[a-z\d]{12}$", RegexOptions.None, "ru-RU")]
    private static partial Regex GuidRegex();
    
    private const double AverageTruckSpeedInKmPerHour = 70;
}