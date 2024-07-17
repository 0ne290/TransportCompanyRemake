using Domain.Interfaces;
using Moq;

namespace DomainTests.Tests.Stubs;

public static class CryptographicServiceStub
{
    public static ICryptographicService Create()
    {
        var mock = new Mock<ICryptographicService>();
        mock.Setup(cs => cs.EncryptAndHash(It.IsAny<string>())).Returns(StubOfEncryptAndHash);

        return mock.Object;
    }
    
    private static string StubOfEncryptAndHash(string value) => $"EncryptAndHashOf{value}";
}