using System.ComponentModel.DataAnnotations;

namespace App.Api.Dtos;

public sealed class CreateProductRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Range(0.01, 1000000)]
    public decimal Price { get; set; }
}
