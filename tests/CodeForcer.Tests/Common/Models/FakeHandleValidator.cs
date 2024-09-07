using CodeForcer.Features.Students.Common.Interfaces;

namespace CodeForcer.Tests.Common.Models;

public sealed class FakeHandleValidator : IHandleValidator
{
    public static List<string> ValidHandles { get; } = [];

    public Task<bool> IsValid(string handle) =>
        Task.FromResult(ValidHandles.Contains(handle));
}
