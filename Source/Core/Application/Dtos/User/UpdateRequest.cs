using System.ComponentModel;
using Newtonsoft.Json;

namespace Application.Dtos.User;

public class UpdateRequest : PropertiesSetFactCheckBase
{
    [JsonProperty(Required = Required.Always)]
    public required string Guid { get; init; }
    
    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required string? SetLogin
    {
        get => _setLogin;
        init
        {
            if (value != null)
                SetProperty(nameof(SetLogin));

            _setLogin = value;
        }
    }
    
    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required string? SetPassword
    {
        get => _setPassword;
        init
        {
            if (value != null)
                SetProperty(nameof(SetPassword));

            _setPassword = value;
        }
    }
    
    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required string? SetName
    {
        get => _setName;
        init
        {
            if (value != null)
                SetProperty(nameof(SetName));

            _setName = value;
        }
    }
    
    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required string? SetContact
    {
        get => _setContact;
        init
        {
            if (value != null)
                SetProperty(nameof(SetContact));

            _setContact = value;
        }
    }
    
    private readonly string? _setLogin;
    
    private readonly string? _setPassword;
    
    private readonly string? _setName;
    
    private readonly string? _setContact;
}
