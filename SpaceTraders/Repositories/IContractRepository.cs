using SpaceTraders.Api.Models;
using SpaceTraders.Models;

namespace SpaceTraders.Repositories;
internal interface IContractRepository
{
    Task AddOrUpdateContract(Contract contract);
    Task AddOrUpdateContracts(List<Contract> contracts);
    Task Clear();
    Task<List<ContractWithCargo>> GetAcceptedCargoForWaypoint(string waypointSymbol);
    Task<List<CargoWithDestination>> GetAllAcceptedContractCargo();
    Task<List<string>> GetAllContracts();
    Task<Contract> GetContract(string contractId);
    Task<int> GetContractsCount();
    Task<Contract?> GetFirstAcceptedContract();
    Task<Contract?> GetFirstContract();
}