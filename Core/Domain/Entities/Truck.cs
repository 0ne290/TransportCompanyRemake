namespace Domain.Entities;

public class Truck
{
    public override string ToString() => Number;

    public decimal CalculateOrderPrice(Order order)
    {
        var weightPricePerKilometer = WeightPrice * order.CargoWeight;
        var volumePricePerKilometer = VolumePrice * order.CargoVolume;
        var totalPricePerKilometer = (weightPricePerKilometer + volumePricePerKilometer) * PricePerKilometer;
        var orderPrice = totalPricePerKilometer * (decimal)order.DistanceInKm;

        return orderPrice;
    }

    public string Number { get; set; } = null!;

    public string TypeAdr { get; set; } = null!;
    
    public bool IsAvailable { get; set; }

    public decimal VolumeMax { get; set; }
    
    public decimal VolumePrice { get; set; }

    public decimal WeightMax { get; set; }
    
    public decimal WeightPrice { get; set; }
    
    public decimal PricePerKilometer { get; set; }
    
    public string BranchAddress { get; set; } = null!;

    public virtual Branch Branch { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}