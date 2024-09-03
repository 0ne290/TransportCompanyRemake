using System.Linq.Expressions;

namespace Application.Interfaces;

// TODO: Этот сервис является жалким подобием паттерна "Repository". А в идеале он должен быть "UnitOfWork". Ну либо
// сделай каким-нибудь чудом рабочий AsNoTracking() в методах Find() и FindAll() EF-реализации этого сервиса
// TODO: Текущий вид этого сервиса не позволяет фильтровать связанные данные. Замени либо "string includedData" на что-то, либо
// саму концепцию "Максимальная универсальность" на "Конкретные юзкейсы". Первый вариант, очевидно, очень сложен, т. к. каждая
// реализация этого сервиса должна будет иметь некоторый "Модуль перевода" с языка этого сервиса на язык конкретного поставщика
// данных. Второй вариант проще, т. к. никакого "языка сервиса" не будет, а запросы конкретных поставщиков будут формироваться
// из конкретных типов данных, но тогда этот общий интерфейс должен "расщепиться" на несколько конкретных интерфейсов - по
// одному на каждую сущность в домене. Например, IOrderStorageService { Order GetByDateCreated(DateTime lowerLimit, DateTime upperLimit, int? count = null) }
public interface IEntityStorageService<TEntity>
{
    Task CreateRange(IEnumerable<TEntity> entities);
    
    Task<TEntity> Find(Expression<Func<TEntity, bool>> filter, string includedData = "");
    
    Task<ICollection<TEntity>> FindAll(Expression<Func<TEntity, bool>> filter, string includedData = "");
    
    Task<bool> Exists(Expression<Func<TEntity, bool>> filter);
    
    Task UpdateRange(ICollection<TEntity> entities);
    
    Task RemoveRange(IEnumerable<TEntity> entities);
}