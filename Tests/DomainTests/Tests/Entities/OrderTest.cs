using Domain.Constants;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace DomainTests.Tests.Entities;

public class OrderTest
{
    [Fact]
    public void Order_New_ArgumentsIsValid_ReturnTheOrderWithTwoDriversAndHazardClassFlag()
    {
        var mock = new Mock<IGeolocationService>();
        mock.Setup(gs => gs.CalculateDistanceInKmByDegrees(It.IsAny<ValueTuple<double, double>>(),
            It.IsAny<ValueTuple<double, double>>())).Returns(StubOfCalculateDistanceInKmByDegrees);

        var stubOfGeolocationService = mock.Object;
        
        var expectedStartAddress = "AnyStartAddress";
        var expectedEndAddress = "AnyEndAddress";
        var expectedDescription = "AnyDescription";
        var expectedStartPointLatitude = 56.9;
        var expectedStartPointLongitude = 4.8;
        var expectedEndPointLatitude = -9;
        var expectedEndPointLongitude = 8;
        var expectedCargoVolume = 60;
        var expectedCargoWeight = 6000;
        var expectedHazardClassFlag = HazardClassesFlags.Class21;
        var expectedTank = true;
        var expectedUser = User.New("AnyName", "AnyContact", 364);
        var expectedBranch = Branch.New("AnyAddress", (34, 75));
        var expectedTruck = Truck.New("AnyNumber", true, 78, 1.2m, 17000, 0.15m, 1,
            HazardClassesFlags.Class21 | HazardClassesFlags.Class22 | HazardClassesFlags.Class23, expectedBranch);
        var expectedDriver1 = Driver.New("AnyDriver1Name", AdrDriverQualificationsFlags.Base, true, expectedBranch);
        var expectedDriver2 = Driver.New("AnyDriver2Name", AdrDriverQualificationsFlags.Base, true, expectedBranch);
        var expectedDistanceInKm = expectedBranch.CalculateDistanceInKmByDegrees(stubOfGeolocationService, (expectedStartPointLatitude, expectedStartPointLongitude)) + stubOfGeolocationService.CalculateDistanceInKmByDegrees((expectedStartPointLatitude, expectedStartPointLongitude),
            (expectedEndPointLatitude, expectedEndPointLongitude)) + expectedBranch.CalculateDistanceInKmByDegrees(stubOfGeolocationService, (expectedEndPointLatitude, expectedEndPointLongitude));
        var expectedPrice = expectedTruck.CalculateOrderPrice(order);
        
        return;
        
        double StubOfCalculateDistanceInKmByDegrees((double Latitude, double Longitude) point1,
            (double Latitude, double Longitude) point2) =>
            point1.Latitude + point1.Longitude + point2.Latitude + point2.Longitude;
    }
}