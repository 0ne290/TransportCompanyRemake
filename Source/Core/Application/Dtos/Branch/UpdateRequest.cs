using System.ComponentModel;
using Newtonsoft.Json;

namespace Application.Dtos.Branch;

public class UpdateRequest : PropertiesSetFactCheckBase
{
    [JsonProperty(Required = Required.Always)]
    public required string Guid { get; init; }
    
    [DefaultValue("")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required string SetAddress
    {
        get => _setAddress;
        init
        {
            if (value != "")
                SetProperty(nameof(SetAddress));

            _setAddress = value;
        }
    }

    [DefaultValue((double?)null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required double? SetLatitude
    {
        get => _setLatitude;
        init
        {
            if (value != null)
                SetProperty(nameof(SetLatitude));

            _setLatitude = value;
        }
    }

    [DefaultValue((double?)null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required double? SetLongitude
    {
        get => _setLongitude;
        init
        {
            if (value != null)
                SetProperty(nameof(SetLongitude));

            _setLongitude = value;
        }
    }
    
    private readonly string _setAddress;

    private readonly double? _setLatitude;

    private readonly double? _setLongitude;
}