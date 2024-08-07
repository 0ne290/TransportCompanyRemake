using System.Linq.Expressions;

namespace Application.Interfaces;

public interface IEntityStorageService<TEntity> : IDisposable, IAsyncDisposable
{
    void CreateRange(IEnumerable<TEntity> entities);
    
    TEntity? Find(Expression<Func<TEntity, bool>> filter, string includedData = "");
    
    IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> filter, string includedData = "");
    
    void UpdateRange(IEnumerable<TEntity> entities);
    
    void RemoveRange(IEnumerable<TEntity> entities);
}