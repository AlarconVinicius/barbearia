using FluentValidation;

namespace BarberFlow.Api.Common.Validation;

public static class FluentValidationExtensions
{
    public static IServiceCollection AddApiValidators(
        this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(ApiAssembly.Instance);

        return services;
    }
}
