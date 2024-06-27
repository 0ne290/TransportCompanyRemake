namespace Domain.Entities;

public class Driver
{
    public Driver()
    {
        Guid = System.Guid.NewGuid().ToString();
    }

    private Driver(string guid)
    {
        Guid = guid;
    }

    public override string ToString() => Name;

    public string Guid { get; } = null!;

    public string Name { get; set; } = null!;
    
    public bool IsAvailable { get; set; }

    public bool CertificatAdr { get; set; }
    
    public int HoursWorkedPerWeek { get; set; }

    public string BranchAddress { get; set; } = null!;

    public virtual Branch Branch { get; set; } = null!;
    
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}