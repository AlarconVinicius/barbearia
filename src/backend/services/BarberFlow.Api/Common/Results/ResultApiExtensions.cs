using BarberFlow.Domain.Results;

namespace BarberFlow.Api.Common.Results;

public static class ResultApiExtensions
{
    public static IResult Match(this Result result)
    {
        return result.IsSuccess
            ? Microsoft.AspNetCore.Http.Results.Ok(ApiResponse.From(result))
            : ApiResults.Problem(result);
    }

    public static IResult Match<TValue>(this Result<TValue> result)
    {
        return result.IsSuccess
            ? Microsoft.AspNetCore.Http.Results.Ok(
                ApiResponse<TValue>.From(result))
            : ApiResults.Problem(result);
    }

    public static IResult Match<TValue>(
        this Result<TValue> result,
        Func<TValue, IResult> onSuccess)
    {
        return result.IsSuccess
            ? onSuccess(result.Value)
            : ApiResults.Problem(result);
    }
}
