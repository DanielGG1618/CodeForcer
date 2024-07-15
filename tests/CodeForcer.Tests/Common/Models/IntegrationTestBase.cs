using CodeForcer.Features.Students.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CodeForcer.Tests.Common.Models;

public abstract class IntegrationTestBase : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly HttpClient Client;
    protected readonly IStudentsRepository StudentsRepository;

    protected IntegrationTestBase(IntegrationTestWebAppFactory factory)
    {
        Client = factory.CreateClient();

        var scope = factory.Services.CreateScope();
        StudentsRepository = scope.ServiceProvider.GetRequiredService<IStudentsRepository>();
    }
}