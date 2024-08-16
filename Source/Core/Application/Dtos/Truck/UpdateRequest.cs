using System.ComponentModel;
using Newtonsoft.Json;

namespace Application.Dtos.Truck;

public class UpdateRequest : PropertiesSetFactCheckBase
{
    [JsonProperty(Required = Required.Always)]
    public required string Guid { get; init; }
    
    [DefaultValue(false)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required bool Recommission
    {
        get => _recommission;
        init
        {
            if (value)
                SetProperty(nameof(Recommission));

            _recommission = value;
        }
    }

    [DefaultValue(false)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required bool Decommission
    {
        get => _decommission;
        init
        {
            if (value)
                SetProperty(nameof(Decommission));

            _decommission = value;
        }
    }

    [DefaultValue("Unknown flag combination")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required string? SetPermittedHazardClassesFlags
    {
        get => _setPermittedHazardClassesFlags;
        init
        {
            if (value != "Unknown flag combination")
                SetProperty(nameof(SetPermittedHazardClassesFlags));

            _setPermittedHazardClassesFlags = value;
        }
    }
    
    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required string? SetBranch
    {
        get => _setBranch;
        init
        {
            if (value != null)
                SetProperty(nameof(SetBranch));

            _setBranch = value;
        }
    }
    
    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required string? SetNumber
    {
        get => _setNumber;
        init
        {
            if (value != null)
                SetProperty(nameof(SetNumber));

            _setNumber = value;
        }
    }
    
    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required bool? SetIsAvailable
    {
        get => _setIsAvailable;
        init
        {
            if (value != null)
                SetProperty(nameof(SetIsAvailable));

            _setIsAvailable = value;
        }
    }
    
    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required bool? SetTrailerIsTank
    {
        get => _setTrailerIsTank;
        init
        {
            if (value != null)
                SetProperty(nameof(SetTrailerIsTank));

            _setTrailerIsTank = value;
        }
    }
    
    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required decimal? SetVolumeMax
    {
        get => _setVolumeMax;
        init
        {
            if (value != null)
                SetProperty(nameof(SetVolumeMax));

            _setVolumeMax = value;
        }
    }
    
    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required decimal? SetVolumePrice
    {
        get => _setVolumePrice;
        init
        {
            if (value != null)
                SetProperty(nameof(SetVolumePrice));

            _setVolumePrice = value;
        }
    }
    
    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required decimal? SetWeightMax
    {
        get => _setWeightMax;
        init
        {
            if (value != null)
                SetProperty(nameof(SetWeightMax));

            _setWeightMax = value;
        }
    }
    
    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required decimal? SetWeightPrice
    {
        get => _setWeightPrice;
        init
        {
            if (value != null)
                SetProperty(nameof(SetWeightPrice));

            _setWeightPrice = value;
        }
    }
    
    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required decimal? SetPricePerKm
    {
        get => _setPricePerKm;
        init
        {
            if (value != null)
                SetProperty(nameof(SetPricePerKm));

            _setPricePerKm = value;
        }
    }

    private readonly bool _recommission;

    private readonly bool _decommission;

    private readonly string? _setPermittedHazardClassesFlags = "Unknown flag combination";
    
    private readonly string? _setBranch;
    
    private readonly string? _setNumber;

    private readonly bool? _setIsAvailable;

    private readonly bool? _setTrailerIsTank;
    
    private readonly decimal? _setVolumeMax;
    
    private readonly decimal? _setVolumePrice;
    
    private readonly decimal? _setWeightMax;
    
    private readonly decimal? _setWeightPrice;
    
    private readonly decimal? _setPricePerKm;
}
