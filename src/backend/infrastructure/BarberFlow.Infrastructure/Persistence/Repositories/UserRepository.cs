using BarberFlow.Domain.Repositories;
using BarberFlow.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(BarberFlowDbContext dbContext)
    : IUserRepository
{
    public Task<bool> ExistsByEmailAsync(
        Email email,
        CancellationToken cancellationToken)
    {
        return dbContext.Users.AnyAsync(
            user => user.Email == email,
            cancellationToken);
    }

    public Task<bool> ExistsByCpfAsync(
        Cpf cpf,
        CancellationToken cancellationToken)
    {
        return dbContext.Users.AnyAsync(
            user => user.Cpf == cpf,
            cancellationToken);
    }
}
