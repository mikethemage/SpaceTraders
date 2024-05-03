namespace SpaceTraders.Repositories;

public class TokenRepository : ITokenRepository
{
    const string filename = "Token.txt";
    private string? _token = null;
    private bool fileLoaded = false;

    public async Task<string?> GetTokenAsync()
    {
        if (_token == null && fileLoaded == false)
        {
            await LoadTokenFromFile();
            fileLoaded = true;
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
            _token = await File.ReadAllTextAsync(filename);
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
            await File.WriteAllTextAsync(filename, _token);
        }
        else
        {
            File.Delete(filename);
        }
    }
}
