using System.Linq.Expressions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EntityStorageServices;

public class EntityFrameworkEntityStorageService<TEntity>(TransportCompanyContext dbContext) : IEntityStorageService<TEntity> where TEntity : class
{
    public async Task CreateRange(IEnumerable<TEntity> entities)
    {
        await dbContext.Set<TEntity>().AddRangeAsync(entities);
        await dbContext.SaveChangesAsync();
    }

    public async Task<TEntity?> Find(Expression<Func<TEntity, bool>> filter, string includedData = "")
    {
        var includedProperties = includedData.Split(";");
        
        var query = dbContext.Set<TEntity>().AsQueryable();
        query = includedProperties.Where(includedProperty => !string.IsNullOrEmpty(includedProperty))
            .Aggregate(query, (current, includedProperty) => current.Include(includedProperty));

        return await query.SingleOrDefaultAsync(filter);
    }

    public async Task<ICollection<TEntity>> FindAll(Expression<Func<TEntity, bool>> filter, string includedData = "")
    {
        var includedProperties = includedData.Split(";");
        
        var query = dbContext.Set<TEntity>().AsQueryable();
        query = includedProperties.Where(includedProperty => !string.IsNullOrEmpty(includedProperty))
            .Aggregate(query, (current, includedProperty) => current.Include(includedProperty));

        return await query.Where(filter).ToListAsync();
    }

    public async Task UpdateRange(ICollection<TEntity> entities)
    {
        var entityStore = dbContext.Set<TEntity>();
        entityStore.AttachRange(entities);
        foreach (var entity in entities)
            dbContext.Entry(entity).State = EntityState.Modified;

        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveRange(IEnumerable<TEntity> entities)
    {
        dbContext.Set<TEntity>().RemoveRange(entities);
        await dbContext.SaveChangesAsync();
    }
}