using Microsoft.EntityFrameworkCore;
using SpaceTraders.Repositories.DatabaseRepositories.DbContexts;
using SpaceTraders.Repositories.DatabaseRepositories.DbModels;
using SpaceTraders.Repositories.MemoryOnlyRepositories;

namespace SpaceTraders.Repositories.DatabaseRepositories;

public class TokenDatabaseRepository : ITokenRepository
{

    private readonly ITokenMemoryOnlyRepository _tokenMemoryOnlyRepository;
    private readonly RepositoryDbContext _repositoryDbContext;

    public TokenDatabaseRepository(ITokenMemoryOnlyRepository tokenMemoryOnlyRepository, RepositoryDbContext repositoryDbContext)
    {
        _tokenMemoryOnlyRepository = tokenMemoryOnlyRepository;
        _repositoryDbContext = repositoryDbContext;
    }

    public async Task<string?> GetTokenAsync()
    {
        if (!_tokenMemoryOnlyRepository.IsLoaded)
        {
            Token? token = await _repositoryDbContext.Tokens.SingleOrDefaultAsync();
            _tokenMemoryOnlyRepository.UpdateToken(token?.TokenValue);
            _tokenMemoryOnlyRepository.IsLoaded = true;
        }
        return _tokenMemoryOnlyRepository.GetToken();
    }

    public async Task UpdateTokenAsync(string? token)
    {
        _tokenMemoryOnlyRepository.UpdateToken(token);

        if (token == null)
        {
            _repositoryDbContext.RemoveRange(_repositoryDbContext.Tokens);
        }
        else
        {
            Token? existingToken = await _repositoryDbContext.Tokens.SingleOrDefaultAsync();
            if (existingToken == null)
            {
                existingToken = new Token();
                _repositoryDbContext.Add(existingToken);
            }
            existingToken.TokenValue = token;
        }
        await _repositoryDbContext.SaveChangesAsync();
    }
}
