using Domain.Interfaces;

namespace Domain.Entities;

public class Branch
{
    public override string ToString() => Address;

    public double CalculateDistanceInKmByDegrees(IGeolocationService geolocationService,
        (double Latitude, double Longitude) point) =>
        geolocationService.CalculateDistanceInKmByDegrees((Latitude, Longitude), (point.Latitude, point.Longitude));
    
    public string Address { get; set; } = null!;
    
    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public virtual ICollection<Truck> Trucks { get; set; } = new List<Truck>();

    public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();
}