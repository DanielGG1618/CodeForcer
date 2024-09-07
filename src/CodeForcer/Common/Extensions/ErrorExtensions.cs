namespace CodeForcer.Common.Extensions;

public static class ErrorExtensions
{
    public static ArgumentException ToArgumentException(this Error error) =>
        new(message: error.Description);
}
