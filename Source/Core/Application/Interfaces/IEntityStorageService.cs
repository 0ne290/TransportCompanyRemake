using System.Linq.Expressions;

namespace Application.Interfaces;

public interface IEntityStorageService<TEntity>
{
    Task CreateRange(IEnumerable<TEntity> entities);
    
    Task<TEntity?> Find(Expression<Func<TEntity, bool>> filter, string includedData = "");
    
    Task<ICollection<TEntity>> FindAll(Expression<Func<TEntity, bool>> filter, string includedData = "");
    
    Task UpdateRange(ICollection<TEntity> entities);
    
    Task RemoveRange(IEnumerable<TEntity> entities);
}