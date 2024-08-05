using System.ComponentModel;
using Domain.Entities;
using Newtonsoft.Json;

namespace Application.Dtos;

public class DriverUpdateRequest : PropertiesSetFactCheckBase
{
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

    private readonly bool _reinstate;
}