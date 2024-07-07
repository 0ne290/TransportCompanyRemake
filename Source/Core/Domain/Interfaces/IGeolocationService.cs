namespace Domain.Interfaces;

public interface IGeolocationService
{
    double CalculateDistanceInKmByDegrees((double Latitude, double Longitude) point1,
        (double Latitude, double Longitude) point2);
}