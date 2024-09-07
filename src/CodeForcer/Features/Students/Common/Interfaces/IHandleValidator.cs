namespace CodeForcer.Features.Students.Common.Interfaces;

public interface IHandleValidator
{
    public Task<bool> IsValid(string handle);
}
