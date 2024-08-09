using System.ComponentModel;
using Newtonsoft.Json;

namespace Application.Dtos.Driver;

public class UpdateRequest : PropertiesSetFactCheckBase
{
    [JsonProperty(Required = Required.Always)]
    public required string Guid { get; init; }
    
    [DefaultValue(false)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required bool Reinstate
    {
        get => _reinstate;
        init
        {
            if (value)
                SetProperty(nameof(Reinstate));

            _reinstate = value;
        }
    }

    [DefaultValue(false)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required bool Dismiss
    {
        get => _dismiss;
        init
        {
            if (value)
                SetProperty(nameof(Dismiss));

            _dismiss = value;
        }
    }

    [DefaultValue(0d)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required double AddHoursWorked
    {
        get => _addHoursWorked;
        init
        {
            if (value != 0)
                SetProperty(nameof(AddHoursWorked));

            _addHoursWorked = value;
        }
    }

    [DefaultValue(false)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required bool ResetHoursWorkedPerWeek
    {
        get => _resetHoursWorkedPerWeek;
        init
        {
            if (value)
                SetProperty(nameof(ResetHoursWorkedPerWeek));

            _resetHoursWorkedPerWeek = value;
        }
    }

    [DefaultValue("Unknown flag")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public required string? SetAdrQualificationFlag
    {
        get => _setAdrQualificationFlag;
        init
        {
            if (value != "Unknown flag")
                SetProperty(nameof(SetAdrQualificationFlag));

            _setAdrQualificationFlag = value;
        }
    }
    
    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, Required = Required.DisallowNull)]
    public required bool? SetAdrQualificationOfTank
    {
        get => _setAdrQualificationOfTank;
        init
        {
            if (value != null)
                SetProperty(nameof(SetAdrQualificationOfTank));

            _setAdrQualificationOfTank = value;
        }
    }
    
    [DefaultValue("")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, Required = Required.DisallowNull)]
    public required string SetBranch
    {
        get => _setBranch;
        init
        {
            if (value != "")
                SetProperty(nameof(SetBranch));

            _setBranch = value;
        }
    }
    
    [DefaultValue("")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, Required = Required.DisallowNull)]
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
    
    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, Required = Required.DisallowNull)]
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

    private readonly bool _reinstate;

    private readonly bool _dismiss;

    private readonly double _addHoursWorked;

    private readonly bool _resetHoursWorkedPerWeek;

    private readonly string? _setAdrQualificationFlag = "Unknown flag";

    private readonly bool? _setAdrQualificationOfTank;
    
    private readonly string _setBranch = "";
    
    private readonly string _setName = "";

    private readonly bool? _setIsAvailable;
}