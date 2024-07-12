using System.Globalization;
using System.Text.RegularExpressions;
using Domain.Entities;
using Domain.Interfaces;
using Moq;

namespace DomainTests.Tests.Entities;

public partial class UserTest
{
    [Fact]
    public void User_New_ArgumentsIsValid_ReturnTheDefaultUser_Test()
    {
        // Arrange1
        var guidRegex = GuidRegex();
        var dynamicPartOfSaltRegex = DynamicPartOfSaltRegex();
        
        var mock = new Mock<ICryptographicService>();
        mock.Setup(cs => cs.EncryptAndHash(It.IsAny<string>())).Returns(StubOfEncryptAndHash);

        var stubOfCryptographicService = mock.Object;
        
        const string expectedName = "AnyName";
        const string expectedContact = "AnyContact";
        const string expectedLogin = "AnyLogin";
        const string password = "AnyPassword";

        const string staticPartOfSalt = "6d9ace9d25bca79be42c971f85a543b22dcee800101d9b39b9213741a5cdcf147b853dc142fa761f66b6cffb50e1a3c5183ae78013124fa58ff41a6edfc6e969";

        var expextedRegistrationDate = DateTime.Now;
        var expextedRegistrationDateError = TimeSpan.FromSeconds(10);
        
        // Act
        var user = User.New(expectedName, expectedContact, expectedLogin, password, stubOfCryptographicService);
        
        // Arrange2
        var dynamicPartOfSalt = user.DynamicPartOfSalt;
        var registrationDate = user.RegistrationDate.ToString(CultureInfo.InvariantCulture);
        var expectedPassword = StubOfEncryptAndHash(ExpectedSalt(password));

        // Assert
        Assert.Matches(guidRegex, user.Guid);
        Assert.Matches(dynamicPartOfSaltRegex, user.DynamicPartOfSalt);
        Assert.Equal(expextedRegistrationDate, user.RegistrationDate, expextedRegistrationDateError);
        Assert.Null(user.VkUserId);
        Assert.NotNull(user.Login);
        Assert.Equal(expectedLogin, user.Login);
        Assert.NotNull(user.Password);
        Assert.Equal(expectedPassword, user.Password);
        Assert.Equal(expectedName, user.Name);
        Assert.Equal(expectedContact, user.Contact);
        return;

        string StubOfEncryptAndHash(string value) => $"EncryptAndHashOf{value}";
        string ExpectedSalt(string value) => value + registrationDate + staticPartOfSalt + expectedLogin + value + dynamicPartOfSalt + expectedLogin;
    }
    
    [Fact]
    public void User_New_ArgumentsIsValid_ReturnTheVkUser_Test()
    {
        // Arrange1
        var guidRegex = GuidRegex();
        var dynamicPartOfSaltRegex = DynamicPartOfSaltRegex();
        
        const string expectedName = "AnyName";
        const string expectedContact = "AnyContact";
        const long expectedVkUserId = 26535;
        
        var expextedRegistrationDate = DateTime.Now;
        var expextedRegistrationDateError = TimeSpan.FromSeconds(10);
        
        // Act
        var user = User.New(expectedName, expectedContact, expectedVkUserId);

        // Assert
        Assert.Matches(guidRegex, user.Guid);
        Assert.Matches(dynamicPartOfSaltRegex, user.DynamicPartOfSalt);
        Assert.Equal(expextedRegistrationDate, user.RegistrationDate, expextedRegistrationDateError);
        Assert.Null(user.Login);
        Assert.Null(user.Password);
        Assert.NotNull(user.VkUserId);
        Assert.Equal(expectedVkUserId, user.VkUserId);
        Assert.Equal(expectedName, user.Name);
        Assert.Equal(expectedContact, user.Contact);
    }
    
    [GeneratedRegex(@"^(?i)[a-z\d]{8}-([a-z\d]{4}-){3}[a-z\d]{12}$", RegexOptions.None, "ru-RU")]
    private static partial Regex GuidRegex();
    
    [GeneratedRegex(@"^(?i)[a-z\d]{128}$", RegexOptions.None, "ru-RU")]
    private static partial Regex DynamicPartOfSaltRegex();
}