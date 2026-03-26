using System.ComponentModel.DataAnnotations;

namespace ProductService.Api.Dtos;

public class AddProductRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Weight { get; set; }

    [Required]
    [MaxLength(120)]
    public string Dimensions { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
}

public class ProductResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Weight { get; set; }
    public string Dimensions { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
