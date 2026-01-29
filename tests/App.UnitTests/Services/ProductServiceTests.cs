using App.Api.Dtos;
using App.Api.Models;
using App.Api.Repositories;
using App.Api.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace App.UnitTests.Services;

public sealed class ProductServiceTests
{
    [Fact]
    public async Task CreateAsync_WithValidRequest_NormalizesFieldsAndPersists()
    {
        var repository = new Mock<IProductRepository>();
        Product? captured = null;
        repository
            .Setup(repo => repo.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((product, _) => captured = product)
            .Returns(Task.CompletedTask);

        var service = new ProductService(repository.Object);
        var request = new CreateProductRequest
        {
            Name = "  Mechanical Keyboard  ",
            Description = "  Compact layout  ",
            Price = 329.90m
        };

        var result = await service.CreateAsync(request, CancellationToken.None);

        result.Name.Should().Be("Mechanical Keyboard");
        result.Description.Should().Be("Compact layout");
        result.Price.Should().Be(329.90m);
        result.Id.Should().NotBe(Guid.Empty);

        captured.Should().NotBeNull();
        captured!.Name.Should().Be("Mechanical Keyboard");
        captured.Description.Should().Be("Compact layout");
        captured.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        repository.Verify(repo => repo.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidName_Throws()
    {
        var repository = new Mock<IProductRepository>();
        var service = new ProductService(repository.Object);
        var request = new CreateProductRequest
        {
            Name = "   ",
            Price = 100m
        };

        var action = () => service.CreateAsync(request, CancellationToken.None);

        await action.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("Name");
    }

    [Fact]
    public async Task CreateAsync_WithInvalidPrice_Throws()
    {
        var repository = new Mock<IProductRepository>();
        var service = new ProductService(repository.Object);
        var request = new CreateProductRequest
        {
            Name = "Mouse",
            Price = 0m
        };

        var action = () => service.CreateAsync(request, CancellationToken.None);

        await action.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("Price");
    }

    [Fact]
    public async Task GetByIdAsync_WhenMissing_ReturnsNull()
    {
        var repository = new Mock<IProductRepository>();
        repository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var service = new ProductService(repository.Object);

        var result = await service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task ListAsync_WithInvalidPagination_Throws()
    {
        var repository = new Mock<IProductRepository>();
        var service = new ProductService(repository.Object);

        var action = () => service.ListAsync(0, 100, CancellationToken.None);

        await action.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Fact]
    public async Task ListAsync_WithValidPagination_ReturnsPagedResult()
    {
        var repository = new Mock<IProductRepository>();
        var products = new List<Product>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Mouse",
                Description = "Wireless",
                Price = 120m,
                CreatedAtUtc = new DateTime(2025, 1, 10, 10, 0, 0, DateTimeKind.Utc)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Monitor",
                Description = "27-inch",
                Price = 1200m,
                CreatedAtUtc = new DateTime(2025, 1, 11, 10, 0, 0, DateTimeKind.Utc)
            }
        };

        repository.Setup(repo => repo.CountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(5);
        repository.Setup(repo => repo.GetPagedAsync(2, 2, It.IsAny<CancellationToken>())).ReturnsAsync(products);

        var service = new ProductService(repository.Object);

        var result = await service.ListAsync(2, 2, CancellationToken.None);

        result.Page.Should().Be(2);
        result.PageSize.Should().Be(2);
        result.TotalItems.Should().Be(5);
        result.TotalPages.Should().Be(3);
        result.Items.Should().HaveCount(2);
        result.Items.Should().ContainSingle(item => item.Name == "Mouse");
    }
}
