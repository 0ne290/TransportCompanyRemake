using Application.Interfaces;

namespace Application.Commands.Driver;

public class SetName : ICommand<Domain.Entities.Driver>
{
    public void Execute(Domain.Entities.Driver entity) => entity.SetName(Name);
    
    public required string Name { get; init; }
}