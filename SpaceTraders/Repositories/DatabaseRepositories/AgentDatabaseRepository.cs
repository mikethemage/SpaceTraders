using Microsoft.EntityFrameworkCore;
using SpaceTraders.Repositories.DatabaseRepositories.DbModels;
using SpaceTraders.Repositories.DatabaseRepositories.DbContexts;
using SpaceTraders.Repositories.MemoryOnlyRepositories;

namespace SpaceTraders.Repositories.DatabaseRepositories;

internal class AgentDatabaseRepository : IAgentRepository
{
    private readonly IAgentMemoryOnlyRepository _agentMemoryOnlyRepository;
    private readonly RepositoryDbContext _repositoryDbContext;

    public AgentDatabaseRepository(IAgentMemoryOnlyRepository agentMemoryOnlyRepository, RepositoryDbContext repositoryDbContext)
    {
        _agentMemoryOnlyRepository = agentMemoryOnlyRepository;
        _repositoryDbContext = repositoryDbContext;
    }

    public async Task<SpaceTraders.Api.Models.Agent?> GetAgentAsync()
    {
        if (!_agentMemoryOnlyRepository.IsLoaded)
        {
            Agent? agent = await _repositoryDbContext.Agents.SingleOrDefaultAsync();
            _agentMemoryOnlyRepository.UpdateAgent(agent?.ToApiModel());
            _agentMemoryOnlyRepository.IsLoaded=true;
        }
        return _agentMemoryOnlyRepository.GetAgent();
    }

    public async Task UpdateAgentAsync(SpaceTraders.Api.Models.Agent? agent)
    {
        _agentMemoryOnlyRepository?.UpdateAgent(agent);

        _repositoryDbContext.RemoveRange(_repositoryDbContext.Agents);
        if (agent != null )        
        {
            _repositoryDbContext.Add(agent.ToDbModel());
        }
        await _repositoryDbContext.SaveChangesAsync();
    }
}
