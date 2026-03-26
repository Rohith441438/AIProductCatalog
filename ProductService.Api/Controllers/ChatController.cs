using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Api.Data;
using ProductService.Api.Dtos;
using ProductService.Api.Models;

namespace ProductService.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ChatController(AppDbContext dbContext) : ControllerBase
{
    [HttpPost("commands")]
    public async Task<ActionResult<ChatCommandResponse>> Execute(ChatCommandRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        if (!TryParseAddProductCommand(request.Command, out var productRequest, out var error))
        {
            return BadRequest(new ChatCommandResponse
            {
                Success = false,
                Message = error
            });
        }

        var product = new Product
        {
            Name = productRequest!.Name,
            Weight = productRequest.Weight,
            Dimensions = productRequest.Dimensions,
            Price = productRequest.Price,
            UserId = userId
        };

        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return Ok(new ChatCommandResponse
        {
            Success = true,
            Message = "Product added via chat command.",
            Product = ProductsController.Map(product)
        });
    }

    private static bool TryParseAddProductCommand(
        string command,
        out AddProductRequest? request,
        out string error)
    {
        // Supported format:
        // add product name=Milk weight=1.2 size=10x10x20cm price=4.99
        request = null;
        error = string.Empty;

        var normalized = command.Trim();
        if (!normalized.StartsWith("add product", StringComparison.OrdinalIgnoreCase))
        {
            error = "Unsupported command. Use: add product name=<name> weight=<weight> size=<dimensions> price=<price>";
            return false;
        }

        var payload = normalized["add product".Length..].Trim();
        var tokens = payload.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var keyValues = tokens
            .Select(t => t.Split('=', 2))
            .Where(parts => parts.Length == 2)
            .ToDictionary(parts => parts[0].Trim().ToLowerInvariant(), parts => parts[1].Trim());

        if (!keyValues.TryGetValue("name", out var name) || string.IsNullOrWhiteSpace(name))
        {
            error = "name is required.";
            return false;
        }

        if (!keyValues.TryGetValue("weight", out var weightText) ||
            !decimal.TryParse(weightText, NumberStyles.Number, CultureInfo.InvariantCulture, out var weight) ||
            weight <= 0)
        {
            error = "weight must be a positive number.";
            return false;
        }

        var sizeKey = keyValues.ContainsKey("size") ? "size" : "dimensions";
        if (!keyValues.TryGetValue(sizeKey, out var dimensions) || string.IsNullOrWhiteSpace(dimensions))
        {
            error = "size (dimensions) is required.";
            return false;
        }

        if (!keyValues.TryGetValue("price", out var priceText) ||
            !decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.InvariantCulture, out var price) ||
            price <= 0)
        {
            error = "price must be a positive number.";
            return false;
        }

        request = new AddProductRequest
        {
            Name = name,
            Weight = weight,
            Dimensions = dimensions,
            Price = price
        };

        return true;
    }
}
