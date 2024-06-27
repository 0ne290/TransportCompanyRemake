namespace Domain.Entities;

public class Branch
{
    public override string ToString() => Address;
    
    public string Address { get; set; } = null!;
    
    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public virtual ICollection<Truck> Trucks { get; set; } = new List<Truck>();

    public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();
}