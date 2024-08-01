using Application.Interfaces;
using Newtonsoft.Json;

namespace Application.Commands.Driver;

public class QualifyAdrTank : ICommand<Domain.Entities.Driver>
{
    public void Execute(Domain.Entities.Driver driver) => driver.QualifyAdrTank();
    
    [JsonProperty(Required = Required.Always, PropertyName = "commandName")]
    public required string CommandName
    {
        get => _commandName;
        init
        {
            if (value != CommandNameConstant)
                throw new ArgumentOutOfRangeException(nameof(value), value, $"The command name must be {CommandNameConstant}.");

            _commandName = value;
        }
    }

    public const string CommandNameConstant = "QualifyAdrTankToDriver";

    private readonly string _commandName = null!;
}