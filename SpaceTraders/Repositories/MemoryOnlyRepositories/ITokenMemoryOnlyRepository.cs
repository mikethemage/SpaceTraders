namespace SpaceTraders.Repositories.MemoryOnlyRepositories;

public interface ITokenMemoryOnlyRepository
{
    string? GetToken();
    void UpdateToken(string? token);
}