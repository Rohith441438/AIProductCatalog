namespace ProductService.Api.Models;

public class ApplicationUser
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
