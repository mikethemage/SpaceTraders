using SpaceTraders.Api.Models;
using SpaceTraders.Models;

namespace SpaceTraders.Services;

internal interface IContractService
{
    Task EnsureAllContractsLoaded();
    Task<List<string>> GetAllContracts();
    Task<List<CargoWithDestination>> GetAllAcceptedContractCargo();
    List<ContractWithCargo> GetAcceptedCargoForWaypoint(string waypointSymbol);
    Contract? GetFirstAcceptedContract();
    void AddOrUpdateContract(Contract contract);
    Contract? GetFirstContract();
    Task<Contract> GetCurrentContract();
}
