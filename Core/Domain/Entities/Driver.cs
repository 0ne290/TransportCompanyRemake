using Domain.Interfaces;

namespace Domain.Entities;

public class Driver
{
    private Driver() { }

    private Driver(DateTime? dismissalDate)
    {
        _dismissalDate = dismissalDate;
    }

    public static Driver New(string name, bool certificatAdr, Branch branch) => new Driver(null)
    {
        Guid = System.Guid.NewGuid().ToString(), HireDate = DateTime.Now, Name = name, IsAvailable = true,
        CertificatAdr = certificatAdr, HoursWorkedPerWeek = 0, TotalHoursWorked = 0, BranchAddress = branch.Address,
        Branch = branch
    };

    public void SetBranch(Branch branch)
    {
        Branch = branch;
        BranchAddress = branch.Address;
    }
    
    public override string ToString() => Name;

    public string Guid { get; private set; } = null!;
    
    public DateTime HireDate { get; private set; }
    
    public DateTime? DismissalDate 
    {
        get => _dismissalDate;
        set
        {
            IsAvailable = value == null;
            _dismissalDate = value;
        }
    }

    public string Name { get; set; } = null!;
    
    public bool IsAvailable { get; set; }

    public bool CertificatAdr { get; set; }
    
    public int HoursWorkedPerWeek { get; set; }
    
    public int TotalHoursWorked { get; set; }

    public string BranchAddress { get; private set; } = null!;

    public virtual Branch Branch { get; private set; } = null!;
    
    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();
    
    private DateTime? _dismissalDate;
}