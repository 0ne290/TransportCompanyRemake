using Application.Interfaces;

namespace Application.Commands.Driver;

public class ResetHoursWorkedPerWeek : ICommand<Domain.Entities.Driver>
{
    public void Execute(Domain.Entities.Driver entity) => entity.ResetHoursWorkedPerWeek();
}