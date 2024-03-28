using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IAgentRepository
{
    Agent? Agent { get; set; }
}