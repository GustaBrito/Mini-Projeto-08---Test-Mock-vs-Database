using App.Api.Models;

namespace App.Api.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product, CancellationToken cancellationToken);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Product>> GetPagedAsync(int skip, int take, CancellationToken cancellationToken);
    Task<int> CountAsync(CancellationToken cancellationToken);
}
