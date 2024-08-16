namespace CodeForcer.Common.Models;

public abstract class EndpointBase : ICarterModule
{
    public abstract void AddRoutes(IEndpointRouteBuilder app);

    protected static IResult Problem(IEnumerable<Error> errors) =>
        Problem(errors.First());

    protected static IResult Problem(Error error) =>
        Results.Problem(
            statusCode: error.Type switch
            {
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            },
            title: error.Description
        );
}
