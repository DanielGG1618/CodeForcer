using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CodeForcer.Tests.Common.Models;

public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<IApiMarker>
{
    private const string TestDbPath = @"C:\Daniel's Files\students.db";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => builder.UseSetting("ConnectionString:CodeForcerDb", "");

    public IntegrationTestWebAppFactory() =>
        File.Create(TestDbPath);

    protected override void Dispose(bool disposing) => 
        File.Delete(TestDbPath);
}
