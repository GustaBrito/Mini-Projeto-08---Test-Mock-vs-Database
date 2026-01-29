using App.Api.Models;

namespace App.Api.Dtos;

public sealed class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public static ProductDto FromEntity(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CreatedAtUtc = product.CreatedAtUtc
        };
    }
}
