namespace CodeForcer.Features.Students;

public abstract class EndpointBase : ICarterModule
{
    public abstract void AddRoutes(IEndpointRouteBuilder app);

    protected static IResult Problem(IEnumerable<Error> errors)
    {
        var firstError = errors.First();
        int statusCode = firstError.Type switch
        {
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        return Results.Problem(statusCode: statusCode, title: firstError.Description);
    }
}