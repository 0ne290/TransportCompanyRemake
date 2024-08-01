using Domain.Interfaces;

namespace Domain.Entities;

public class Branch
{
    private Branch() { }

    public static Branch New(string address, (double Latitude, double Longitude) point) => new()
    {
        Guid = System.Guid.NewGuid().ToString(), Address = address, Latitude = point.Latitude,
        Longitude = point.Longitude
    };

    public (double LengthInKm, double DrivingHours)
        CalculateLengthInKmOfOrderRouteClosedAtBranchAndApproximateDrivingHoursOfTruckAlongIt(
            Order order, IGeolocationService geolocationService) =>
        geolocationService.CalculateLengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt(
            (Latitude, Longitude), (order.StartPointLatitude, order.StartPointLongitude), (order.EndPointLatitude, order.EndPointLongitude));
    
    public override string ToString() => $"Address = {Address}";
    
    public string Guid { get; private set; } = null!;
    
    public string Address { get; set; } = null!;
    
    public double Latitude { get; set; }

    public double Longitude { get; set; }
    
    public virtual ICollection<Truck> Trucks { get; private set; } = new List<Truck>();
    
    public virtual ICollection<Driver> Drivers { get; private set; } = new List<Driver>();
}