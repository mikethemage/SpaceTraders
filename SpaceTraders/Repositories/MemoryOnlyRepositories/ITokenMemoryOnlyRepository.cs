namespace SpaceTraders.Repositories.MemoryOnlyRepositories;

public interface ITokenMemoryOnlyRepository
{
    bool IsLoaded { get; set; }

    string? GetToken();
    void UpdateToken(string? token);
}