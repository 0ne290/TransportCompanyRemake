using Application.Interfaces;

namespace Application.Commands.Driver;

public class Work : ICommand<Domain.Entities.Driver>
{
    public void Execute(Domain.Entities.Driver entity) => entity.Work();
}