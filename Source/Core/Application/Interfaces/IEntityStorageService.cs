using System.Linq.Expressions;

namespace Application.Interfaces;

// TODO: Этот сервис является жалким подобием паттерна "Repository". А в идеале он должен быть "UnitOfWork". Ну либо
// сделай каким-нибудь чудом рабочий AsNoTracking() в методах Find() и FindAll() EF-реализации этого сервиса
public interface IEntityStorageService<TEntity>
{
    Task CreateRange(IEnumerable<TEntity> entities);
    
    Task<TEntity> Find(Expression<Func<TEntity, bool>> filter, string includedData = "");
    
    Task<ICollection<TEntity>> FindAll(Expression<Func<TEntity, bool>> filter, string includedData = "");
    
    Task<bool> Exists(Expression<Func<TEntity, bool>> filter);
    
    Task UpdateRange(ICollection<TEntity> entities);
    
    Task RemoveRange(IEnumerable<TEntity> entities);
}