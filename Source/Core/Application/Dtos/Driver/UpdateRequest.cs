using System.ComponentModel;
using Newtonsoft.Json;

namespace Application.Dtos.Driver;

public class UpdateRequest : PropertiesSetFactCheckBase
{
    [JsonProperty(Required = Required.Always)]
    public string Guid { get; init; } = null!;
    
    [DefaultValue(false)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool Reinstate
    {
        get => _reinstate;
        init
        {
            if (!value)
                return;

            _reinstate = value;
            SetProperty(nameof(Reinstate));
        }
    }

    [DefaultValue(false)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool Dismiss
    {
        get => _dismiss;
        init
        {
            if (!value)
                return;

            _dismiss = value;
            SetProperty(nameof(Dismiss));
        }
    }

    [DefaultValue(0d)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public double AddHoursWorked
    {
        get => _addHoursWorked;
        init
        {
            if (value == 0)
                return;

            _addHoursWorked = value;
            SetProperty(nameof(AddHoursWorked));
        }
    }

    [DefaultValue(false)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool ResetHoursWorkedPerWeek
    {
        get => _resetHoursWorkedPerWeek;
        init
        {
            if (!value)
                return;

            _resetHoursWorkedPerWeek = value;
            SetProperty(nameof(ResetHoursWorkedPerWeek));
        }
    }

    [DefaultValue(0)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public int? SetAdrQualificationFlag
    {
        get => _setAdrQualificationFlag;
        init
        {
            if (value == 0)
                return;

            _setAdrQualificationFlag = value;
            SetProperty(nameof(SetAdrQualificationFlag));
        }
    }
    
    [DefaultValue("")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, Required = Required.DisallowNull)]
    public string SetBranch
    {
        get => _setBranch;
        init
        {
            if (value == "")
                return;

            _setBranch = value;
            SetProperty(nameof(SetBranch));
        }
    }
    
    [DefaultValue("")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate, Required = Required.DisallowNull)]
    public string SetName
    {
        get => _setName;
        init
        {
            if (value == "")
                return;

            _setName = value;
            SetProperty(nameof(SetName));
        }
    }
    
    [DefaultValue(null)]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
    public bool? SetIsAvailable
    {
        get => _setIsAvailable;
        init
        {
            if (value == null)
                return;

            _setIsAvailable = value;
            SetProperty(nameof(SetIsAvailable));
        }
    }

    private readonly bool _reinstate;

    private readonly bool _dismiss;

    private readonly double _addHoursWorked;

    private readonly bool _resetHoursWorkedPerWeek;

    private readonly int? _setAdrQualificationFlag = 0;
    
    private readonly string _setBranch = "";
    
    private readonly string _setName = "";

    private readonly bool? _setIsAvailable;
}