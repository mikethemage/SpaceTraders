
namespace SpaceTraders.Repositories;

public interface ITokenRepository
{
    Task<string?> GetTokenAsync();
    Task UpdateTokenAsync(string? token);
}