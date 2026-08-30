using BarberFlow.Domain.Common;

namespace BarberFlow.Domain.Abstractions;

public interface IUnitOfWork
{
    Task InsertAsync<TEntity>(
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : Entity;

    void Update<TEntity>(TEntity entity)
        where TEntity : Entity;

    void Delete<TEntity>(TEntity entity)
        where TEntity : Entity;

    Task CommitAsync(CancellationToken cancellationToken = default);
}
