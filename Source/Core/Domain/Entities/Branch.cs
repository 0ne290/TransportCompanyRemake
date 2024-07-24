using Domain.Configs;
using Domain.ServiceInterfaces;

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
        CalculateLengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt(
            OrderConfig orderConfig, IGeolocationService geolocationService) =>
        geolocationService.CalculateLengthInKmOfClosedRouteAndApproximateDrivingHoursOfTruckAlongIt(
            (Latitude, Longitude), orderConfig.StartPoint, orderConfig.EndPoint);
    
    public override string ToString() => $"Address = {Address}";
    
    public string Guid { get; private set; } = null!;
    
    public virtual ICollection<Truck> Trucks { get; private set; } = new List<Truck>();

    public virtual ICollection<Driver> Drivers { get; private set; } = new List<Driver>();
    
    public string Address { get; set; } = null!;
    
    public double Latitude { get; set; }

    public double Longitude { get; set; }
}