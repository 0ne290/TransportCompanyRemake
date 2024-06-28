using Domain.Interfaces;

namespace Domain.Services;

public class DefaultGeolocationService : IGeolocationService
{
    public double CalculateDistanceInKilometersByDegrees((double Latitude, double Longitude) point1,
        (double Latitude, double Longitude) point2)
    {
        point1.Latitude *= NumberOfRadiansInOneDegree;
        point1.Longitude *= NumberOfRadiansInOneDegree;
        point2.Latitude *= NumberOfRadiansInOneDegree;
        point2.Longitude *= NumberOfRadiansInOneDegree;

        var latitudeCosineOfPoint1 = Math.Cos(point1.Latitude);
        var latitudeCosineOfPoint2 = Math.Cos(point2.Latitude);
        var latitudeSineOfPoint1 = Math.Sin(point1.Latitude);
        var latitudeSineOfPoint2 = Math.Sin(point2.Latitude);
        var longitudeDifference = Math.Abs(point2.Longitude - point1.Longitude);
        var cosineOfLongitudeDifference = Math.Cos(longitudeDifference);

        var angularDifference = Math.Atan2(
            Math.Sqrt(Math.Pow(latitudeCosineOfPoint2 * Math.Sin(longitudeDifference), 2) + Math.Pow(
                latitudeCosineOfPoint1 * latitudeSineOfPoint2 -
                latitudeSineOfPoint1 * latitudeCosineOfPoint2 * cosineOfLongitudeDifference, 2)),
            latitudeSineOfPoint1 * latitudeSineOfPoint2 +
            latitudeCosineOfPoint1 * latitudeCosineOfPoint2 * cosineOfLongitudeDifference);

        return angularDifference * EarthRadiusInKm;
    }

    private const double NumberOfRadiansInOneDegree = Math.PI / 180;

    private const double EarthRadiusInKm = 6_371.0088;
}