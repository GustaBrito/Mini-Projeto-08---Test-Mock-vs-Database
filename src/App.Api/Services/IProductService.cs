using App.Api.Dtos;

namespace App.Api.Services;

public interface IProductService
{
    Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PagedResult<ProductDto>> ListAsync(int page, int pageSize, CancellationToken cancellationToken);
}
