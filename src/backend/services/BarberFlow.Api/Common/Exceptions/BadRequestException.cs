using BarberFlow.Domain.Results;

namespace BarberFlow.Api.Common.Exceptions;

public abstract class BadRequestException : Exception
{
    protected BadRequestException(string message, params Error[] errors)
        : base(message)
    {
        Errors = errors ?? [];
    }

    public Error[] Errors { get; }
}
