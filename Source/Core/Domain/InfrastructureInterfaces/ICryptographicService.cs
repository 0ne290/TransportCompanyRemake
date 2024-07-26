namespace Domain.InfrastructureInterfaces;

public interface ICryptographicService
{
    string EncryptAndHash(string value);
}