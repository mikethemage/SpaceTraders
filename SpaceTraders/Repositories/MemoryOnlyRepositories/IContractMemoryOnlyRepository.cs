using SpaceTraders.Api.Models;
using SpaceTraders.Models;

namespace SpaceTraders.Repositories.MemoryOnlyRepositories;
internal interface IContractMemoryOnlyRepository
{
    bool IsLoaded { get; set; }

    void AddOrUpdateContract(Contract contract);
    void AddOrUpdateContracts(List<Contract> contracts);
    void Clear();
    List<ContractWithCargo> GetAcceptedCargoForWaypoint(string waypointSymbol);    
    List<CargoWithDestination> GetAllAcceptedContractCargo();
    List<string> GetAllContracts();
    Contract GetContract(string contractId);
    int GetContractsCount();
    Contract? GetFirstAcceptedContract();
    Contract? GetFirstContract();
}