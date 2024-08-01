using Domain.Constants;

namespace Domain.Entities;

public class Truck
{
    private Truck() { }

    public static Truck New(string number, bool tank, decimal volumeMax, decimal volumePrice, decimal weightMax,
        decimal weightPrice, decimal pricePerKm, string branchGuid, int? permittedHazardClassesFlags)
    {
        var truck = new Truck
        {
            Guid = System.Guid.NewGuid().ToString(), WriteOnDate = DateTime.Now, WriteOffDate = null,
            PermittedHazardClassesFlags = null, BranchGuid = branchGuid, Number = number, IsAvailable = true,
            TrailerIsTank = tank, VolumeMax = volumeMax, VolumePrice = volumePrice, WeightMax = weightMax,
            WeightPrice = weightPrice, PricePerKm = pricePerKm
        };
        if (permittedHazardClassesFlags != null)
            truck.SetPermittedHazardClassesFlags(permittedHazardClassesFlags.Value);

        return truck;
    }

    public void Reinstate()
    {
        IsAvailable = true;
        WriteOffDate = null;
    }

    public void Dismiss()
    {
        IsAvailable = false;
        WriteOffDate = DateTime.Now;
    }
    
    // В соответствии с действующим на момент 01.07.2024 ГОСТ Р 57479, существует 20 подклассов опасности грузов. Для
    // перевозки груза с тем или иным подклассом опасности, фура и ее полуприцеп должны быть сертифицированы на
    // соответствие всем требованиям перевозки грузов с данным подклассом опасности. Флаговое свойство ниже как раз и
    // моделирует наличие/отсутствие у совокупности Фура-Полуприцеп сертификата по всем 20 подклассам. Например, если
    // 16-ый и 17-ый биты равны 1, то Фура-Полуприцеп имеет сертификат по 16-му 17-му подклассам (в терминах ГОСТ Р
    // 57479 это будут подклассы 6.1 "Токсичные вещества" и 6.2 "Инфекционные вещества")
    public void SetPermittedHazardClassesFlags(int permittedHazardClassesFlags)
    {
        if (!HazardClassesFlags.IsFlagCombination(permittedHazardClassesFlags))
            throw new ArgumentOutOfRangeException(nameof(permittedHazardClassesFlags), permittedHazardClassesFlags,
                "The PermittedHazardClassesFlags describe 20 hazard subclasses. This means that the " +
                "value of their combination must be in the range [1; 2^20 (1048576)).");
            
        PermittedHazardClassesFlags = permittedHazardClassesFlags;
    }

    public void ResetPermittedHazardClassesFlags() => PermittedHazardClassesFlags = null;

    public void SetBranch(Branch branch)
    {
        Branch = branch;
        BranchGuid = branch.Guid;
    }

    public decimal CalculateOrderPricePerKm(Order order)
    {
        var weightPricePerKilometer = WeightPrice * order.CargoWeight;
        var volumePricePerKilometer = VolumePrice * order.CargoVolume;
        
        return (weightPricePerKilometer + volumePricePerKilometer) * PricePerKm;
    }

    public override string ToString() => $"Number = {Number}";

    public string Guid { get; private set; } = null!;
    
    public DateTime WriteOnDate { get; private set; }
    
    public DateTime? WriteOffDate { get; private set; }
    
    public int? PermittedHazardClassesFlags { get; private set; }
    
    public string BranchGuid { get; private set; } = null!;
    
    public string Number { get; private set; } = null!;

    public bool IsAvailable { get; private set; }
    
    public bool TrailerIsTank { get; private set; }

    public decimal VolumeMax { get; private set; }
    
    public decimal VolumePrice { get; private set; }

    public decimal WeightMax { get; private set; }
    
    public decimal WeightPrice { get; private set; }
    
    public decimal PricePerKm { get; private set; }
    
    public virtual Branch Branch { get; private set; } = null!;

    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();
}