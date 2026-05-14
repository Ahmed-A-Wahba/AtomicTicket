using AtomicTicket.SharedKernel.Results;

namespace AtomicTicket.Api.Endpoints;

public static class ApiResults
{
    public static IResult Problem(IEnumerable<Error> errors)
    {
        if (!errors.Any())
        {
            return Results.Problem();
        }

        if (errors.All(error => error.ErrorType == ErrorType.Validation))
        {
            return ValidationProblem(errors);
        }

        return Problem(errors.First());
    }

    public static IResult Problem(Error error)
    {
        var statusCode = error.ErrorType switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError,
        };

        return Results.Problem(
            statusCode: statusCode,
            title: error.Code,
            detail: error.Description);
    }

    private static IResult ValidationProblem(IEnumerable<Error> errors)
    {
        var modelState = errors
            .GroupBy(e => e.Code)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.Description).ToArray()
            );

        return Results.ValidationProblem(modelState);
    }
}