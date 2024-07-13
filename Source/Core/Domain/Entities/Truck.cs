using Domain.Constants;

namespace Domain.Entities;

public class Truck
{
    private Truck() { }

    public static Truck New(string number, bool tank, decimal volumeMax,
        decimal volumePrice, decimal weightMax, decimal weightPrice, decimal pricePerKm,
        int? permittedHazardClassesFlags, Branch branch)
    {
        var truck = new Truck
        {
            Guid = System.Guid.NewGuid().ToString(), WriteOnDate = DateTime.Now, Number = number, Tank = tank,
            VolumeMax = volumeMax, VolumePrice = volumePrice, WeightMax = weightMax, WeightPrice = weightPrice,
            PricePerKm = pricePerKm
        };
        truck.Reinstate();
        truck.SetPermittedHazardClassesFlags(permittedHazardClassesFlags);
        truck.SetBranch(branch);

        return truck;
    }

    public void WriteOff()
    {
        IsAvailable = false;
        WriteOffDate = DateTime.Now;
    }

    public void Reinstate()
    {
        IsAvailable = true;
        WriteOffDate = null;
    }
    
    // В соответствии с действующим на момент 01.07.2024 ГОСТ Р 57479, существует 20 подклассов опасности грузов. Для
    // перевозки груза с тем или иным подклассом опасности, фура и ее полуприцеп должны быть сертифицированы на
    // соответствие всем требованиям перевозки грузов с данным подклассом опасности. Флаговое свойство ниже как раз и
    // моделирует наличие/отсутствие у совокупности Фура-Полуприцеп сертификата по всем 20 подклассам. Например, если
    // 16-ый и 17-ый биты равны 1, то Фура-Полуприцеп имеет сертификат по 16-му 17-му подклассам (в терминах ГОСТ Р
    // 57479 это будут подклассы 6.1 "Токсичные вещества" и 6.2 "Инфекционные вещества")
    public void SetPermittedHazardClassesFlags(int? permittedHazardClassesFlags)
    {
        if (permittedHazardClassesFlags != null)
            if (!HazardClassesFlags.IsFlagCombination(permittedHazardClassesFlags.Value))
                throw new ArgumentOutOfRangeException(nameof(permittedHazardClassesFlags), permittedHazardClassesFlags,
                    "The PermittedHazardClassesFlags describe 20 hazard subclasses. This means that the " +
                    "value of their combination must be in the range [1; 2^20 (1048576)).");
            
        PermittedHazardClassesFlags = permittedHazardClassesFlags;
    }

    public void SetBranch(Branch branch)
    {
        Branch = branch;
        BranchGuid = branch.Guid;
    }

    public decimal CalculateOrderPrice(Order order)
    {
        var weightPricePerKilometer = WeightPrice * order.CargoWeight;
        var volumePricePerKilometer = VolumePrice * order.CargoVolume;
        var totalPricePerKilometer = (weightPricePerKilometer + volumePricePerKilometer) * PricePerKm;
        var orderPrice = totalPricePerKilometer * (decimal)order.DistanceInKm;

        return orderPrice;
    }

    public override string ToString() => $"Number = {Number}";

    public string Guid { get; private set; } = null!;
    
    public DateTime WriteOnDate { get; private set; }
    
    public DateTime? WriteOffDate { get; private set; }
    
    public int? PermittedHazardClassesFlags { get; private set; }
    
    public string BranchGuid { get; private set; } = null!;

    public virtual Branch Branch { get; private set; } = null!;

    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();
    
    public string Number { get; set; } = null!;

    public bool IsAvailable { get; set; }
    
    public bool Tank { get; set; }

    public decimal VolumeMax { get; set; }
    
    public decimal VolumePrice { get; set; }

    public decimal WeightMax { get; set; }
    
    public decimal WeightPrice { get; set; }
    
    public decimal PricePerKm { get; set; }
}
