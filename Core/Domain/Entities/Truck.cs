namespace Domain.Entities;

public class Truck
{
    private Truck() { }

    private Truck(DateTime? dismissalDate)
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

    public decimal CalculateOrderPrice(Order order)
    {
        var weightPricePerKilometer = WeightPrice * order.CargoWeight;
        var volumePricePerKilometer = VolumePrice * order.CargoVolume;
        var totalPricePerKilometer = (weightPricePerKilometer + volumePricePerKilometer) * PricePerKm;
        var orderPrice = totalPricePerKilometer * (decimal)order.DistanceInKm;

        return orderPrice;
    }

    public override string ToString() => Number;

    public string Guid { get; private set; } = null!;

    public string Number { get; set; } = null!;

    public string TypeAdr
    {
        get => _typeAdr;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                
            }
            _typeAdr = value;
        }
    }

    public bool IsAvailable { get; set; }

    public decimal VolumeMax { get; set; }
    
    public decimal VolumePrice { get; set; }

    public decimal WeightMax { get; set; }
    
    public decimal WeightPrice { get; set; }
    
    public decimal PricePerKm { get; set; }
    
    public string BranchAddress { get; set; } = null!;

    public virtual Branch Branch { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    
    private string _typeAdr { get; set; } = null!;
}