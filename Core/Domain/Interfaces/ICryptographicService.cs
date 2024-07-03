namespace Domain.Interfaces;

public interface ICryptographicService
{
    string Hash(string value);
}