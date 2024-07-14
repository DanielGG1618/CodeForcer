namespace CodeForcer.Tests.Common.Models;

public abstract class IntegrationTestBase : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly HttpClient Client;

    protected IntegrationTestBase(IntegrationTestWebAppFactory factory)
    {
        Client = factory.CreateClient();
    }
}