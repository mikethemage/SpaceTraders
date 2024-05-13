using SpaceTraders.Api.Models;
using SpaceTraders.Models;

namespace SpaceTraders.Services;

internal interface IContractService
{
    Task EnsureAllContractsLoaded();
    //Task<List<string>> GetAllContracts();
    Task<List<CargoWithDestination>> GetAllAcceptedContractCargo();
    //List<ContractWithCargo> GetAcceptedCargoForWaypoint(string waypointSymbol);
    Task<Contract?> GetFirstAcceptedContract();
    Task AddOrUpdateContract(Contract contract);
    Task<Contract?> GetFirstContract();
    Task<Contract> GetCurrentContract();
    Task Clear();
}
