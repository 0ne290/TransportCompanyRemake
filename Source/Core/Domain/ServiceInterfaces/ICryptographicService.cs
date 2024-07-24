namespace Domain.ServiceInterfaces;

public interface ICryptographicService
{
    string EncryptAndHash(string value);
}