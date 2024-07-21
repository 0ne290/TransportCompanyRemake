namespace Domain.Interfaces;

public interface IGeolocationService
{
    double CalculateDistanceInKmByDegrees((double Latitude, double Longitude) point1,
        (double Latitude, double Longitude) point2);

    double CalculateApproximateDrivingHoursOfTruckAlongClosedRoute(params (double Latitude, double Longitude)[] closedRoute);
}