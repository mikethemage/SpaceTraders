namespace SpaceTraders.Repositories.MemoryOnlyRepositories;

public class TokenRepository : ITokenRepository
{
    const string _filename = "Token.txt";
    private string? _token = null;
    private bool _fileLoaded = false;

    public async Task<string?> GetTokenAsync()
    {
        if (_token == null && _fileLoaded == false)
        {
            await LoadTokenFromFile();
            _fileLoaded = true;
        }

        return _token;
    }

    public async Task UpdateTokenAsync(string? token)
    {
        _token = token;
        await SaveTokenToFile();
    }

    private async Task LoadTokenFromFile()
    {
        try
        {
            _token = await File.ReadAllTextAsync(_filename);
        }
        catch
        {
            _token = null;
        }
    }

    private async Task SaveTokenToFile()
    {
        if (_token != null)
        {
            await File.WriteAllTextAsync(_filename, _token);
        }
        else
        {
            File.Delete(_filename);
        }
    }
}
