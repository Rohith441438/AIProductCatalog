using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Api.Data;
using ProductService.Api.Dtos;
using ProductService.Api.Models;

namespace ProductService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProductsController(AppDbContext dbContext) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ProductResponse>> Add(AddProductRequest request)
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var product = new Product
        {
            Name = request.Name.Trim(),
            Weight = request.Weight,
            Dimensions = request.Dimensions.Trim(),
            Price = request.Price,
            UserId = userId.Value
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMine), new { }, Map(product));
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ProductResponse>>> GetMine()
    {
        var userId = GetUserId();
        if (userId is null)
        {
            return Unauthorized();
        }

        var products = await dbContext.Products
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAtUtc)
            .Select(p => Map(p))
            .ToListAsync();

        return Ok(products);
    }

    internal static ProductResponse Map(Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Weight = product.Weight,
        Dimensions = product.Dimensions,
        Price = product.Price,
        CreatedAtUtc = product.CreatedAtUtc
    };

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
