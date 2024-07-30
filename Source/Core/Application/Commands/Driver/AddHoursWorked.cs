using Application.Interfaces;

namespace Application.Commands.Driver;

public class AddHoursWorked : ICommand<Domain.Entities.Driver>
{
    public void Execute(Domain.Entities.Driver entity) => entity.AddHoursWorked(HoursWorked);
    
    public required double HoursWorked { get; init; }
}