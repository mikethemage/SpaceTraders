
namespace SpaceTraders.Repositories;

internal interface ITokenRepository
{
    string Token { get; set; }

    Task LoadTokenFromFile();
    Task SaveTokenToFile();
}