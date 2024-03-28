using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IContractRepository
{
    List<Contract> Contracts { get; set; }
}