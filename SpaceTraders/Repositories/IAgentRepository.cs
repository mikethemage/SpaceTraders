using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IAgentRepository
{   
    Task<Agent?> GetAgentAsync();
    Task UpdateAgentAsync(Agent? agent);
}