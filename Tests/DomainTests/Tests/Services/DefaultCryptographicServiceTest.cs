using Domain.Services;

namespace DomainTests.Tests.Services;

public class DefaultCryptographicServiceTest
{
    // Encryption algorithm: if the character code is less than 127, increase it by 43, otherwise decrease it by 21
    [Fact]
    public void EncryptAndHash_ReturnTheSha512HashOfEncryptedValue_Test()
    {
        // Arrange
        var defaultCryptographicService = new DefaultCryptographicService();
        const string value = "123_-_ AnyText =!=321";
        const string expectedHash =
            "41860a22721c48749a3ca670c40c526138b61bb0f179670a5361e9aaa5d1660b1db67b065832608a4ecbffa90f6a10e7ec5473f" +
            "ccb99a348c80ea36a2c07874c";

        // Act
        var actualHash = defaultCryptographicService.EncryptAndHash(value);

        // Assert
        Assert.Equal(expectedHash, actualHash);
    }
}