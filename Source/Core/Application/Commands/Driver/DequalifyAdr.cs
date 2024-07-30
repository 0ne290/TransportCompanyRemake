using Application.Interfaces;

namespace Application.Commands.Driver;

public class DequalifyAdr : ICommand<Domain.Entities.Driver>
{
    public void Execute(Domain.Entities.Driver entity) => entity.DequalifyAdr();
}