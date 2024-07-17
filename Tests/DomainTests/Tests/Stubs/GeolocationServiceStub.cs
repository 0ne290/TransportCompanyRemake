using Domain.Interfaces;
using Moq;

namespace DomainTests.Tests.Stubs;

public static class GeolocationServiceStub
{
    public static IGeolocationService Create()
    {
        var mock = new Mock<IGeolocationService>();
        mock.Setup(gs => gs.CalculateDistanceInKmByDegrees(It.IsAny<ValueTuple<double, double>>(),
            It.IsAny<ValueTuple<double, double>>())).Returns(StubOfCalculateDistanceInKmByDegrees);
        
        return mock.Object;
    }
    
    private static double StubOfCalculateDistanceInKmByDegrees((double Latitude, double Longitude) point1,
        (double Latitude, double Longitude) point2) =>
        point1.Latitude + point1.Longitude + point2.Latitude + point2.Longitude;
}