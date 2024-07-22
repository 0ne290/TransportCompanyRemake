namespace Domain.Configs;

public record OrderConfig(string StartAddress, string EndAddress, string CargoDescription, (double Latitude, double Longitude) StartPoint, (double Latitude, double Longitude) EndPoint, decimal CargoVolume, decimal CargoWeight, bool Tank);