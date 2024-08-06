using System.Linq.Expressions;

namespace Application.Interfaces;

public interface IEntityStorageService<TEntity> : IDisposable, IAsyncDisposable
{
    IEntityStorageService<TEntity> AsNoTracking();
    
    void CreateRange(IEnumerable<TEntity> entities);
    
    TEntity? Find(Expression<Func<TEntity, bool>> filter);
    
    IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> filter);
    
    void RemoveAll(IEnumerable<TEntity> entities);
    
    void SaveChanges();
}