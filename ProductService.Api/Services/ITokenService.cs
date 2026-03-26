using ProductService.Api.Models;

namespace ProductService.Api.Services;

public interface ITokenService
{
    string CreateToken(ApplicationUser user);
}
