using BarberFlow.Domain.Abstractions;
using BarberFlow.Domain.Common;

namespace BarberFlow.Infrastructure.Persistence;

internal sealed class UnitOfWork(BarberFlowDbContext dbContext) : IUnitOfWork
{
    public async Task InsertAsync<TEntity>(
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : Entity
    {
        await dbContext.Set<TEntity>().AddAsync(
            entity,
            cancellationToken);
    }

    public void Update<TEntity>(TEntity entity)
        where TEntity : Entity
    {
        dbContext.Set<TEntity>().Update(entity);
    }

    public void Delete<TEntity>(TEntity entity)
        where TEntity : Entity
    {
        dbContext.Set<TEntity>().Remove(entity);
    }

    public async Task CommitAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
