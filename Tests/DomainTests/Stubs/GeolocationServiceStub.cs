using Domain.Interfaces;
using Moq;

namespace DomainTests.Stubs;

public static class GeolocationServiceStub
{
    public static IGeolocationService Create()
    {
        var mock = new Mock<IGeolocationService>();
        mock.Setup(gs =>
                gs.CalculateLengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt(
                    It.IsAny<ValueTuple<double, double>[]>()))
            .Returns(StubOfCalculateLengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt);

        return mock.Object;
    }

    private static (double LengthInKm, double DrivingHours)
        StubOfCalculateLengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt(
            params (double Latitude, double Longitude)[] closedRoute)
    {
        var lengthInKm = closedRoute.Sum(point => point.Latitude + point.Longitude);

        return (lengthInKm, lengthInKm + 37);
    }

}