using System.ComponentModel;
using Newtonsoft.Json;

namespace Application.Dtos.User;

public class UpdateRequest : PropertiesSetFactCheckBase
{
    [JsonProperty(Required = Required.Always)]
    public required string Guid { get; init; }
    
    [DefaultValue("")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required string SetLogin
    {
        get => _setLogin;
        init
        {
            if (value != "")
                SetProperty(nameof(SetLogin));

            _setLogin = value;
        }
    }
    
    [DefaultValue("")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required string SetPassword
    {
        get => _setPassword;
        init
        {
            if (value != "")
                SetProperty(nameof(SetPassword));

            _setPassword = value;
        }
    }
    
    [DefaultValue("")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required string SetName
    {
        get => _setName;
        init
        {
            if (value != "")
                SetProperty(nameof(SetName));

            _setName = value;
        }
    }
    
    [DefaultValue("")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required string SetContact
    {
        get => _setContact;
        init
        {
            if (value != "")
                SetProperty(nameof(SetContact));

            _setContact = value;
        }
    }
    
    private readonly string _setLogin = null!;
    
    private readonly string _setPassword = null!;
    
    private readonly string _setName = null!;
    
    private readonly string _setContact = null!;
}