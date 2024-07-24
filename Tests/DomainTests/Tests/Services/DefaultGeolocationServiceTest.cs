using Domain.ServiceDefaultImplementations;

namespace DomainTests.Tests.Services;

public class DefaultGeolocationServiceTest
{
    // Formula for calculating LengthInKm: haversine formula modified for antipodes
    // Formula for calculating DrivingHours: LengthInKm / 70
    [Fact]
    public void
        DefaultGeolocationService_CalculateLengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt_ContextAndArgumentsIsValid_ReturnTheLengthInKmAndDrivingHours_Test()
    {
        // Arrange
        var point1 = (52, 103);
        var point2 = (76, 111);
        const double expectedLengthInKm = 5_382.593;
        const double expectedDrivingHours = expectedLengthInKm / 70;
        var defaultGeolocationService = new DefaultGeolocationService();

        // Act
        var actualLengthInKmAndDrivingHours = defaultGeolocationService.CalculateLengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt(point1, point2);

        // Assert
        Assert.Equal(expectedLengthInKm, actualLengthInKmAndDrivingHours.LengthInKm, 3);
        Assert.Equal(expectedDrivingHours, actualLengthInKmAndDrivingHours.DrivingHours, 3);
    }
}