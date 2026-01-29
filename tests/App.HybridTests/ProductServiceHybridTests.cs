using App.Api.Data;
using App.Api.Dtos;
using App.Api.Repositories;
using App.Api.Services;
using App.HybridTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace App.HybridTests;

[Collection("postgres")]
public sealed class ProductServiceHybridTests
{
    private readonly PostgresContainerFixture _fixture;

    public ProductServiceHybridTests(PostgresContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [DockerFact]
    public async Task CreateAndGetById_UsesRealDatabase()
    {
        var connectionString = await _fixture.CreateDatabaseAsync();
        try
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            await using var dbContext = new AppDbContext(options);
            await dbContext.Database.MigrateAsync();

            var repository = new ProductRepository(dbContext);
            var service = new ProductService(repository);

            var created = await service.CreateAsync(new CreateProductRequest
            {
                Name = "Mechanical Keyboard",
                Description = "Compact layout",
                Price = 329.90m
            }, CancellationToken.None);

            var fetched = await service.GetByIdAsync(created.Id, CancellationToken.None);

            fetched.Should().NotBeNull();
            fetched!.Id.Should().Be(created.Id);
            fetched.Name.Should().Be("Mechanical Keyboard");
            fetched.Description.Should().Be("Compact layout");
        }
        finally
        {
            await _fixture.DropDatabaseAsync(connectionString);
        }
    }

    [DockerFact]
    public async Task ListAsync_ReturnsPagedResult()
    {
        var connectionString = await _fixture.CreateDatabaseAsync();
        try
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            await using var dbContext = new AppDbContext(options);
            await dbContext.Database.MigrateAsync();

            var repository = new ProductRepository(dbContext);
            var service = new ProductService(repository);

            await service.CreateAsync(new CreateProductRequest
            {
                Name = "Mouse",
                Description = "Wireless",
                Price = 120m
            }, CancellationToken.None);
            await service.CreateAsync(new CreateProductRequest
            {
                Name = "Monitor",
                Description = "27-inch",
                Price = 1200m
            }, CancellationToken.None);
            await service.CreateAsync(new CreateProductRequest
            {
                Name = "Headset",
                Description = "Noise-canceling",
                Price = 499m
            }, CancellationToken.None);

            var result = await service.ListAsync(1, 2, CancellationToken.None);

            result.Page.Should().Be(1);
            result.PageSize.Should().Be(2);
            result.TotalItems.Should().Be(3);
            result.TotalPages.Should().Be(2);
            result.Items.Should().HaveCount(2);
        }
        finally
        {
            await _fixture.DropDatabaseAsync(connectionString);
        }
    }
}
