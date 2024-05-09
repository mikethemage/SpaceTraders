using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SpaceTraders.Repositories.DatabaseRepositories.DbContexts;
using SpaceTraders.Repositories.DatabaseRepositories.DbModels;

namespace SpaceTraders.Repositories.DatabaseRepositories;

public class TokenDatabaseRepository : ITokenRepository
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
 
    private string? _token;
    private bool _isLoaded = false;
    

    public TokenDatabaseRepository(IServiceScopeFactory serviceScopeFactory)
    {        
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<string?> GetTokenAsync()
    {
        using var scope = _serviceScopeFactory.CreateAsyncScope();
        RepositoryDbContext dbContext = scope.ServiceProvider.GetRequiredService<RepositoryDbContext>();

        if (!_isLoaded)        
        {
            Token? token = (await dbContext.Tokens.ToListAsync()).FirstOrDefault();
            _token = token?.TokenValue;

            _isLoaded = true;            
        }
        return _token;               
    }

    public async Task UpdateTokenAsync(string? token)
    {
        using var scope = _serviceScopeFactory.CreateAsyncScope();
        RepositoryDbContext dbContext = scope.ServiceProvider.GetRequiredService<RepositoryDbContext>();

        _token = token;
        if(_token == null)
        {
            dbContext.RemoveRange(dbContext.Tokens);            
        }
        else 
        {
            Token? existingToken = dbContext.Tokens.FirstOrDefault();
            if (existingToken == null)
            {
                existingToken = new Token();
                dbContext.Add(existingToken);
            }
            existingToken.TokenValue = _token;            
        }
        await dbContext.SaveChangesAsync();
    }
}
