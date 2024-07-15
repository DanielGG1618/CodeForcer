using CodeForcer.Features.Students.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CodeForcer.Tests.Common.Models;

public abstract class IntegrationTestBase : IClassFixture<IntegrationTestWebAppFactory>, IAsyncDisposable
{
    protected readonly HttpClient Client;
    protected readonly IStudentsRepository StudentsRepository;

    protected IntegrationTestBase(IntegrationTestWebAppFactory factory)
    {
        Client = factory.CreateClient();

        StudentsRepository = factory.Services.GetService<IStudentsRepository>()!;
    }

    public async ValueTask DisposeAsync()
    {
        await StudentsRepository.Clear();
    }
}