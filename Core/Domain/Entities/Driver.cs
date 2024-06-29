using Domain.Interfaces;

namespace Domain.Entities;

public class Driver
{
    private Driver() { }

    private Driver(DateTime? dismissalDate)
    {
        _dismissalDate = dismissalDate;
    }

    public static Driver New(IBranchRepository branchRepository, string name, bool certificatAdr, string branchAddress)
    {
        var branch = branchRepository.Find(b => b.Address == branchAddress);
        if (branch == null)
            throw new ArgumentOutOfRangeException(nameof(branchAddress), branchAddress,
                "The branch repository does not contain a branch with the specified address.");

        return new Driver(null)
        {
            Guid = System.Guid.NewGuid().ToString(), HireDate = DateTime.Now, Name = name, IsAvailable = false,
            CertificatAdr = certificatAdr, HoursWorkedPerWeek = 0, TotalHoursWorked = 0, BranchAddress = branchAddress,
            Branch = branch
        };
    }

    public static Driver New(IBranchRepository branchRepository, string name, bool certificatAdr, Branch branch)
    {
        if (!branchRepository.Exists(b => b.Address == branch.Address))
            throw new ArgumentOutOfRangeException(nameof(branch), branch,
                "The branch repository does not contain the specified branch.");

        return new Driver(null)
        {
            Guid = System.Guid.NewGuid().ToString(), HireDate = DateTime.Now, Name = name, IsAvailable = false,
            CertificatAdr = certificatAdr, HoursWorkedPerWeek = 0, TotalHoursWorked = 0, BranchAddress = branch.Address,
            Branch = branch
        };
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

    public string Name { get; private set; } = null!;
    
    public bool IsAvailable { get; private set; }

    public bool CertificatAdr { get; private set; }
    
    public int HoursWorkedPerWeek { get; private set; }
    
    public int TotalHoursWorked { get; private set; }

    public string BranchAddress { get; private set; } = null!;

    public virtual Branch Branch { get; private set; } = null!;
    
    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();
    
    private DateTime? _dismissalDate;
}