using System.Text;
using Application.Commands.Driver;
using Application.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Application.Commands;

public class DriverMacroCommand
{
    public static DriverMacroCommand? FromJson(string macroCommandJson)
    {
        try
        {
            var macroCommandJToken = JsonConvert.DeserializeObject<JToken>(macroCommandJson);
            if (macroCommandJToken == null)
                return null;

            var driverGuid = (string?)macroCommandJToken["driverGuid"];
            if (driverGuid == null)
                return null;

            if (macroCommandJToken["commands"] is not JArray commandsJArray)
                return null;

            var commands = new List<ICommand<Domain.Entities.Driver>>(commandsJArray.Count);
            foreach (var commandJToken in commandsJArray)
            {
                var commandName = (string?)commandJToken["commandName"];
                ICommand<Domain.Entities.Driver>? command;
                switch (commandName)
                {
                    case AddHoursWorked.CommandNameConstant:
                        command = commandJToken.ToObject<AddHoursWorked>();
                        break;
                    case DequalifyAdr.CommandNameConstant:
                        command = commandJToken.ToObject<DequalifyAdr>();
                        break;
                    case DequalifyAdrTank.CommandNameConstant:
                        command = commandJToken.ToObject<DequalifyAdrTank>();
                        break;
                    case Dismiss.CommandNameConstant:
                        command = commandJToken.ToObject<Dismiss>();
                        break;
                    case QualifyAdr.CommandNameConstant:
                        command = commandJToken.ToObject<QualifyAdr>();
                        break;
                    case QualifyAdrTank.CommandNameConstant:
                        command = commandJToken.ToObject<QualifyAdrTank>();
                        break;
                    case Reinstate.CommandNameConstant:
                        command = commandJToken.ToObject<Reinstate>();
                        break;
                    case ResetHoursWorkedPerWeek.CommandNameConstant:
                        command = commandJToken.ToObject<ResetHoursWorkedPerWeek>();
                        break;
                    case Rest.CommandNameConstant:
                        command = commandJToken.ToObject<Rest>();
                        break;
                    case SetBranch.CommandNameConstant:
                        command = commandJToken.ToObject<SetBranch>();
                        break;
                    case SetName.CommandNameConstant:
                        command = commandJToken.ToObject<SetName>();
                        break;
                    case Work.CommandNameConstant:
                        command = commandJToken.ToObject<Work>();
                        break;
                    default:
                        return null;
                }

                if (command == null)
                    return null;

                commands.Add(command);
            }

            return new DriverMacroCommand { DriverGuid = driverGuid, Commands = commands };
        }
        catch (JsonSerializationException)
        {
            return null;
        }
    }
    
    public void Execute(Domain.Entities.Driver driver)
    {
        foreach (var command in Commands)
            command.Execute(driver);
    }
    
    public required string DriverGuid { get; init; }
    
    public required IEnumerable<ICommand<Domain.Entities.Driver>> Commands { get; init; }
}