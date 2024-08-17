using System.Globalization;
using Application.Dtos.Branch;

namespace Web.ViewModels;

public record Branch
{
    public Branch(Response branch)
    {
        Guid = branch.Guid;
        Address = branch.Address;
        Latitude = branch.Latitude.ToString("F6", CultureInfo.InvariantCulture);
        Longitude = branch.Longitude.ToString("F6", CultureInfo.InvariantCulture);
        Trucks = branch.Trucks!.Select(t => new Truck(t)).ToList();
        Drivers = branch.Drivers!.Select(d => new Driver(d)).ToList();
    }

    public string Guid { get; }
    
    public string Address { get; }
    
    public string Latitude { get; }
    
    public string Longitude { get; }
    
    public ICollection<Truck> Trucks { get; }
    
    public ICollection<Driver> Drivers { get; }
}