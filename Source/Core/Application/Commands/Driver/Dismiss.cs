using Application.Interfaces;

namespace Application.Commands.Driver;

public class Dismiss : ICommand<Domain.Entities.Driver>
{
    public void Execute(Domain.Entities.Driver entity) => entity.Dismiss();
}