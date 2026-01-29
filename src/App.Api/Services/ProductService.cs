using App.Api.Dtos;
using App.Api.Models;
using App.Api.Repositories;

namespace App.Api.Services;

public sealed class ProductService : IProductService
{
    private const int MaxPageSize = 50;
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var normalizedName = NormalizeName(request.Name, nameof(request.Name));
        EnsureValidPrice(request.Price, nameof(request.Price));

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            Description = NormalizeDescription(request.Description),
            Price = request.Price,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _repository.AddAsync(product, cancellationToken);
        return ProductDto.FromEntity(product);
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : ProductDto.FromEntity(product);
    }

    public async Task<PagedResult<ProductDto>> ListAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        EnsureValidPagination(page, pageSize);

        var totalItems = await _repository.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        var items = await _repository.GetPagedAsync((page - 1) * pageSize, pageSize, cancellationToken);

        return new PagedResult<ProductDto>
        {
            Items = items.Select(ProductDto.FromEntity).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
    }

    private static string NormalizeName(string name, string paramName)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name is required.", paramName);
        }

        return name.Trim();
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    private static void EnsureValidPrice(decimal price, string paramName)
    {
        if (price <= 0)
        {
            throw new ArgumentException("Price must be greater than zero.", paramName);
        }
    }

    private static void EnsureValidPagination(int page, int pageSize)
    {
        if (page < 1 || pageSize < 1 || pageSize > MaxPageSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                $"Page must be at least 1 and pageSize must be between 1 and {MaxPageSize}.");
        }
    }
}
