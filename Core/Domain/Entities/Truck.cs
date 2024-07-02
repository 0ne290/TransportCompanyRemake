using System.Collections;
using Domain.Constants;

namespace Domain.Entities;

public class Truck
{
    private Truck() { }

    private Truck(DateTime? writeOffDate, int permittedHazardClassesFlags)
    {
        _writeOffDate = writeOffDate;
        _permittedHazardClassesFlags = permittedHazardClassesFlags;
    }

    public static Truck New(string number, int? permittedHazardClassesFlags, bool tank, decimal volumeMax, decimal volumePrice,
        decimal weightMax, decimal weightPrice, decimal pricePerKm, Branch branch) => new()
    {
        Guid = System.Guid.NewGuid().ToString(), HireDate = DateTime.Now, WriteOffDate = null, Number = number,
        PermittedHazardClassesFlags = permittedHazardClassesFlags, Tank = tank, VolumeMax = volumeMax,
        VolumePrice = volumePrice, WeightMax = weightMax, WeightPrice = weightPrice, PricePerKm = pricePerKm,
        BranchAddress = branch.Address, Branch = branch
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
    
    public DateTime WriteOnDate { get; private set; }
    
    public DateTime? WriteOffDate 
    {
        get => _writeOffDate;
        set
        {
            IsAvailable = value == null;
            _writeOffDate = value;
        }
    }

    public string Number { get; set; } = null!;

    // В соответствии с действующим на момент 01.07.2024 ГОСТ Р 57479, существует 20 подклассов опасности грузов. Для
    // перевозки груза с тем или иным подклассом опасности, фура и ее полуприцеп должны быть сертифицированы на
    // соответствие всем требованиям перевозки грузов с данным подклассом опасности. Флаговое свойство ниже как раз и
    // моделирует наличие/отсутствие у совокупности Фура-Полуприцеп сертификата по всем 20 подклассам. Например, если
    // 17-ый бит равен 1, то Фура-Полуприцеп имеет сертификат по 17 подклассу (в терминах ГОСТ Р 57479 это будет
    // подкласс 6.2 - "Инфекционные вещества")
    public int? PermittedHazardClassesFlags
    {
        get => _permittedHazardClassesFlags;
        set
        {
            if (value != null)
                if (!HazardClassesFlags.IsFlagCombination(value.Value))
                    throw new ArgumentOutOfRangeException(nameof(value), value,
                        "The flags describe 20 hazard subclasses. This means that the value of their combination must be in the range [0; 2^20 (1048576)).");
            
            _permittedHazardClassesFlags = value;
        }
    }

    public bool IsAvailable { get; set; }
    
    public bool Tank { get; set; }

    public decimal VolumeMax { get; set; }
    
    public decimal VolumePrice { get; set; }

    public decimal WeightMax { get; set; }
    
    public decimal WeightPrice { get; set; }
    
    public decimal PricePerKm { get; set; }
    
    public string BranchAddress { get; private set; } = null!;

    public virtual Branch Branch { get; private set; } = null!;

    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();
    
    private DateTime? _writeOffDate;

    private int? _permittedHazardClassesFlags;
}
