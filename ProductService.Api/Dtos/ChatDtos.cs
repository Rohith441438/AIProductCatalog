using System.ComponentModel.DataAnnotations;

namespace ProductService.Api.Dtos;

public class ChatCommandRequest
{
    [Required]
    public string Command { get; set; } = string.Empty;
}

public class ChatCommandResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public ProductResponse? Product { get; set; }
}
