using SpaceTraders.Api.Models;
using SpaceTraders.Models;

namespace SpaceTraders.Repositories;
internal interface IContractRepository
{
    void AddOrUpdateContract(Contract contract);
    void AddOrUpdateContracts(List<Contract> contracts);
    Task EnsureAllContractsLoaded();
    List<ContractWithCargo> GetAcceptedCargoForWaypoint(string waypointSymbol);
    Task<List<CargoWithDestination>> GetAllAcceptedContractCargo();
    Contract? GetFirstAcceptedContract();
    Contract? GetFirstContract();
}