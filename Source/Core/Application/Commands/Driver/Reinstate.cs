using Application.Interfaces;

namespace Application.Commands.Driver;

public class Reinstate : ICommand<Domain.Entities.Driver>
{
    public void Execute(Domain.Entities.Driver entity) => entity.Reinstate();
}