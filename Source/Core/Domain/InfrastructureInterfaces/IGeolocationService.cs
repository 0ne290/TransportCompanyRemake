namespace Domain.InfrastructureInterfaces;

public interface IGeolocationService
{
    (double LengthInKm, double DrivingHours) CalculateLengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt(
        params (double Latitude, double Longitude)[] closedRoute);
}