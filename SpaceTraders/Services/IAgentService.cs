using SpaceTraders.Api.Models;

namespace SpaceTraders.Services;
internal interface IAgentService
{
    Task<Agent?> GetAgent();
    Task<int> GetShipCount();
    Task UpdateAgent(Agent? agent);
}