using System.Security.Cryptography;
using System.Text;
using Domain.Interfaces;

namespace Domain.Services;

public class DefaultCryptographicService : ICryptographicService
{
    public string EncryptAndHash(string value)
    {
        var sha512OfEncryptedValue = SHA512.HashData(Encrypt(value));
            
        var stringBuilder = new StringBuilder();
            
        foreach (var b in sha512OfEncryptedValue)
            stringBuilder.Append(b.ToString("x2"));

        return stringBuilder.ToString();
    }

    private static byte[] Encrypt(string value) => Encoding.UTF8
        .GetBytes(value).Select(b =>
        {
            if (b < 127)
                return (byte)(b + 43);
            return (byte)(b - 21);
        }).ToArray();
}