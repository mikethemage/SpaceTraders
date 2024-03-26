using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Repositories;
internal class TokenRepository : ITokenRepository
{
    public string Token { get; set; } = string.Empty;

    public async Task LoadTokenFromFile()
    {
        try
        {
            Token = await File.ReadAllTextAsync("Token.txt");
        }
        catch
        {
            Token = string.Empty;
        }
    }

    public async Task SaveTokenToFile()
    {
        await File.WriteAllTextAsync("Token.txt", Token);
    }
}
