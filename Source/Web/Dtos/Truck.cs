using System.Globalization;
using Application.Dtos.Truck;

namespace Web.Dtos;

public record Truck
{
    public Truck(Response truck)
    {
        Guid = truck.Guid;
        CommissionedDate = truck.CommissionedDate.ToString(CultureInfo.InvariantCulture);
        PermittedHazardClassesFlags = truck.PermittedHazardClassesFlags ?? "null";
        Number = truck.Number;
        IsAvailable = truck.IsAvailable ? "true" : "false";
        TrailerIsTank = truck.TrailerIsTank ? "true" : "false";
        VolumeMax = truck.VolumeMax.ToString("F6", CultureInfo.InvariantCulture);
        VolumePrice = truck.VolumePrice.ToString("F6", CultureInfo.InvariantCulture);
        WeightMax = truck.WeightMax.ToString("F6", CultureInfo.InvariantCulture);
        WeightPrice = truck.WeightPrice.ToString("F6", CultureInfo.InvariantCulture);
        PricePerKm = truck.PricePerKm.ToString("F6", CultureInfo.InvariantCulture);
        OrderPrice = truck.OrderPrice?.ToString("F6", CultureInfo.InvariantCulture) ?? "null";

        if (truck.DecommissionedDate == null)
        {
            DecommissionedDate = "Эксплуатируется";
            ButtonValue = "Decommission";
            ButtonText = "Вывести из эксплуатации";
        }
        else
        {
            DecommissionedDate = truck.DecommissionedDate.Value.ToString(CultureInfo.InvariantCulture);
            ButtonValue = "Recommission";
            ButtonText = "Вернуть в эксплуатацию";
        }
    }

    public string Guid { get; }
    
    public string CommissionedDate { get; }
    
    public string DecommissionedDate { get; }
    
    public string PermittedHazardClassesFlags { get; }
    
    public string Number { get; }
    
    public string IsAvailable { get; set; }
    
    public string TrailerIsTank { get; set; }
    
    public string VolumeMax { get; }
    
    public string VolumePrice { get; }
    
    public string WeightMax { get; }
    
    public string WeightPrice { get; }
    
    public string OrderPrice { get; }
    
    public string PricePerKm { get; }
    
    public string ButtonValue { get; }
    
    public string ButtonText { get; }
    
    public string? Color { get; set; }
}