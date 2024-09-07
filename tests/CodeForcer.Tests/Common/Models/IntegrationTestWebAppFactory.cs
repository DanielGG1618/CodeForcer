using CodeForcer;
using CodeForcer.Features.Students.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace CodeForcer.Tests.Common.Models;

public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<IApiMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.ConfigureServices(services => services
            .AddSingleton<IStudentsRepository, InMemoryStudentsRepository>()
            .AddSingleton<IHandleValidator, FakeHandleValidator>()
        );
}
