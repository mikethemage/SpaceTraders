using SpaceTraders.Api.Models;
using SpaceTraders.Models;

namespace SpaceTraders.Repositories;
internal interface IContractRepository
{
    void AddOrUpdateContract(Contract contract);
    List<ContractWithCargo> GetAcceptedCargoForWaypoint(string waypointSymbol);
    Task<List<CargoWithDestination>> GetAllAcceptedContractCargo();
}