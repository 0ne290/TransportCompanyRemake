using Domain.Constants;

namespace Domain.Entities;

public class Truck
{
    private Truck() { }

    public static Truck New(string number, bool trailerIsTank, decimal volumeMax, decimal volumePrice, decimal weightMax,
        decimal weightPrice, decimal pricePerKm, Branch branch, int? permittedHazardClassesFlags)
    {
        if (permittedHazardClassesFlags != null && !HazardClassesFlags.IsFlagCombination(permittedHazardClassesFlags.Value))
            throw new ArgumentOutOfRangeException(nameof(permittedHazardClassesFlags), permittedHazardClassesFlags,
                "The PermittedHazardClassesFlags describe 20 hazard subclasses. This means that the " +
                "value of their combination must be in the range [1; 2^20 (1048576)).");

        return new Truck
        {
            Number = number, TrailerIsTank = trailerIsTank, VolumeMax = volumeMax, VolumePrice = volumePrice,
            WeightMax = weightMax, WeightPrice = weightPrice, PricePerKm = pricePerKm, BranchGuid = branch.Guid,
            Branch = branch, PermittedHazardClassesFlags = permittedHazardClassesFlags
        };
    }

    public void Recommission()
    {
        if (DecommissionedDate == null)
            throw new InvalidOperationException($"Truck {Guid}. Only a decommissioned truck can use the recommission operation.");
        
        IsAvailable = true;
        DecommissionedDate = null;
    }

    public void Decommission()
    {
        if (DecommissionedDate != null)
            throw new InvalidOperationException($"Truck {Guid}. A decommissioned truck can only use the recommission operation.");
        
        IsAvailable = false;
        DecommissionedDate = DateTime.Now;
    }
    
    // В соответствии с действующим на момент 01.07.2024 ГОСТ Р 57479, существует 20 подклассов опасности грузов. Для
    // перевозки груза с тем или иным подклассом опасности, фура и ее полуприцеп должны быть сертифицированы на
    // соответствие всем требованиям перевозки грузов с данным подклассом опасности. Флаговое свойство ниже как раз и
    // моделирует наличие/отсутствие у совокупности Фура-Полуприцеп сертификата по всем 20 подклассам. Например, если
    // 16-ый и 17-ый биты равны 1, то Фура-Полуприцеп имеет сертификат по 16-му 17-му подклассам (в терминах ГОСТ Р
    // 57479 это будут подклассы 6.1 "Токсичные вещества" и 6.2 "Инфекционные вещества")
    public void SetPermittedHazardClassesFlags(int? permittedHazardClassesFlags)
    {
        if (DecommissionedDate != null)
            throw new InvalidOperationException($"Truck {Guid}. A decommissioned truck can only use the recommission operation.");
        if (permittedHazardClassesFlags != null && !HazardClassesFlags.IsFlagCombination(permittedHazardClassesFlags.Value))
            throw new ArgumentOutOfRangeException(nameof(permittedHazardClassesFlags), permittedHazardClassesFlags,
                $"Truck {Guid}. The PermittedHazardClassesFlags describe 20 hazard subclasses. This means that the " +
                "value of their combination must be in the range [1; 2^20 (1048576)).");
            
        PermittedHazardClassesFlags = permittedHazardClassesFlags;
    }

    public void SetBranch(Branch branch)
    {
        if (DecommissionedDate != null)
            throw new InvalidOperationException($"Truck {Guid}. A decommissioned truck can only use the recommission operation.");
        
        BranchGuid = branch.Guid;
        Branch = branch;
    }
    
    public void SetNumber(string number)
    {
        if (DecommissionedDate != null)
            throw new InvalidOperationException($"Truck {Guid}. A decommissioned truck can only use the recommission operation.");
        
        Number = number;
    }
    
    public void SetIsAvailable(bool isAvailable)
    {
        if (DecommissionedDate != null)
            throw new InvalidOperationException($"Truck {Guid}. A decommissioned truck can only use the recommission operation.");
        
        IsAvailable = isAvailable;
    }
    
    public void SetTrailerIsTank(bool trailerIsTank)
    {
        if (DecommissionedDate != null)
            throw new InvalidOperationException($"Truck {Guid}. A decommissioned truck can only use the recommission operation.");
        
        TrailerIsTank = trailerIsTank;
    }
    
    public void SetVolumeMax(decimal volumeMax)
    {
        if (DecommissionedDate != null)
            throw new InvalidOperationException($"Truck {Guid}. A decommissioned truck can only use the recommission operation.");
            
        VolumeMax = volumeMax;
    }

    public void SetVolumePrice(decimal volumePrice)
    {
        if (DecommissionedDate != null)
            throw new InvalidOperationException($"Truck {Guid}. A decommissioned truck can only use the recommission operation.");
            
        VolumePrice = volumePrice;
    }
    
    public void SetWeightMax(decimal weightMax)
    {
        if (DecommissionedDate != null)
            throw new InvalidOperationException($"Truck {Guid}. A decommissioned truck can only use the recommission operation.");
            
        WeightMax = weightMax;
    }
    
    public void SetWeightPrice(decimal weightPrice)
    {
        if (DecommissionedDate != null)
            throw new InvalidOperationException($"Truck {Guid}. A decommissioned truck can only use the recommission operation.");
            
        WeightPrice = weightPrice;
    }
    
    public void SetPricePerKm(decimal pricePerKm)
    {
        if (DecommissionedDate != null)
            throw new InvalidOperationException($"Truck {Guid}. A decommissioned truck can only use the recommission operation.");
        
        PricePerKm = pricePerKm;
    }

    public decimal CalculateOrderPricePerKm(Order order)
    {
        var weightPricePerKilometer = WeightPrice * order.CargoWeight;
        var volumePricePerKilometer = VolumePrice * order.CargoVolume;
        
        return (weightPricePerKilometer + volumePricePerKilometer) * PricePerKm;
    }

    public override string ToString() => $"Number = {Number}";

    public string Guid { get; private set; } = System.Guid.NewGuid().ToString();

    public DateTime CommissionedDate { get; private set; } = DateTime.Now;
    
    public DateTime? DecommissionedDate { get; private set; }
    
    public int? PermittedHazardClassesFlags { get; private set; }
    
    public string BranchGuid { get; private set; } = null!;
    
    public string Number { get; private set; } = null!;

    public bool IsAvailable { get; private set; } = true;
    
    public bool TrailerIsTank { get; private set; }

    public decimal VolumeMax { get; private set; }
    
    public decimal VolumePrice { get; private set; }

    public decimal WeightMax { get; private set; }
    
    public decimal WeightPrice { get; private set; }
    
    public decimal PricePerKm { get; private set; }
    
    public virtual Branch Branch { get; private set; } = null!;

    public virtual ICollection<Order> Orders { get; private set; } = new List<Order>();
}