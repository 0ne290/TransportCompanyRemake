namespace Domain.Interfaces;

public interface ICryptographicService
{
    string EncryptAndHash(string value);
}