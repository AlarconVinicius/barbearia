using BarberFlow.Domain.ValueObjects;

namespace BarberFlow.Domain.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(
        Email email,
        CancellationToken cancellationToken);

    Task<bool> ExistsByCpfAsync(
        Cpf cpf,
        CancellationToken cancellationToken);
}
