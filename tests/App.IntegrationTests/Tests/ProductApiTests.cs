using System.Net;
using System.Net.Http.Json;
using App.Api.Dtos;
using App.IntegrationTests.Fixtures;
using FluentAssertions;

namespace App.IntegrationTests.Tests;

public sealed class ProductApiTests : IClassFixture<ApiTestFixture>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly ApiTestFixture _fixture;

    public ProductApiTests(ApiTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.Client;
    }

    public Task InitializeAsync()
    {
        return _fixture.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Create_returns_201_with_created_product()
    {
        var request = new CreateProductRequest
        {
            Name = "Mechanical Keyboard",
            Description = "Compact layout with tactile switches",
            Price = 329.90m
        };

        var response = await _client.PostAsJsonAsync("/api/products", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProduct.Should().NotBeNull();
        createdProduct!.Name.Should().Be(request.Name);
        createdProduct.Price.Should().Be(request.Price);
        createdProduct.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Create_with_invalid_payload_returns_400()
    {
        var request = new CreateProductRequest
        {
            Name = "",
            Price = -10m
        };

        var response = await _client.PostAsJsonAsync("/api/products", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_returns_404_when_product_is_missing()
    {
        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task List_returns_paged_result_with_metadata()
    {
        await _client.PostAsJsonAsync("/api/products", new CreateProductRequest
        {
            Name = "Mouse",
            Description = "Wireless with USB receiver",
            Price = 120m
        });
        await _client.PostAsJsonAsync("/api/products", new CreateProductRequest
        {
            Name = "Monitor",
            Description = "27-inch IPS panel",
            Price = 1200m
        });
        await _client.PostAsJsonAsync("/api/products", new CreateProductRequest
        {
            Name = "Headset",
            Description = "Noise-canceling microphone",
            Price = 499m
        });

        var response = await _client.GetAsync("/api/products?page=1&pageSize=2");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var pageResult = await response.Content.ReadFromJsonAsync<PagedResult<ProductDto>>();
        pageResult.Should().NotBeNull();
        pageResult!.Items.Should().HaveCount(2);
        pageResult.Page.Should().Be(1);
        pageResult.PageSize.Should().Be(2);
        pageResult.TotalItems.Should().Be(3);
        pageResult.TotalPages.Should().Be(2);
        pageResult.Items.Should().BeInAscendingOrder(item => item.CreatedAtUtc);
    }
}
