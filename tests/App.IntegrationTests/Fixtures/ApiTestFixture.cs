using App.Api.Data;
using App.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace App.IntegrationTests.Fixtures;

public sealed class ApiTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;

    public ApiTestFixture()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("app_tests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();
    }

    public CustomWebApplicationFactory Factory { get; private set; } = null!;
    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        Factory = new CustomWebApplicationFactory(_dbContainer.GetConnectionString());
        Client = Factory.CreateClient();
        await Factory.EnsureDatabaseMigratedAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Products.RemoveRange(db.Products);
        await db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        await Factory.DisposeAsync();
        await _dbContainer.DisposeAsync();
    }
}
