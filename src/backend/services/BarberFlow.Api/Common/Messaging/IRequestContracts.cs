using BarberFlow.Domain.Results;

namespace BarberFlow.Api.Common.Messaging;

public interface IRequest;

public interface ICommand : IRequest;

public interface ICommand<out TResult> : ICommand;

public interface IQuery<out TResult> : IRequest;

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<Result<TResult>> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken);
}

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> HandleAsync(
        TQuery query,
        CancellationToken cancellationToken);
}
