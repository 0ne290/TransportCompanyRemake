using Application.Interfaces;

namespace Application.Commands;

public class MacroCommand<TEntity>
{
    public void Execute(TEntity entity)
    {
        foreach (var command in Commands)
            command.Execute(entity);
    }
    
    public required string EntityGuid { get; init; }
    
    public required IEnumerable<ICommand<TEntity>> Commands { get; init; }
}