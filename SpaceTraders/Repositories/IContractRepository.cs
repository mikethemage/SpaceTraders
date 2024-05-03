using SpaceTraders.Api.Models;
using SpaceTraders.Models;

namespace SpaceTraders.Repositories;
internal interface IContractRepository
{
    void AddOrUpdateContract(Contract contract);
    void AddOrUpdateContracts(List<Contract> contracts);

    List<ContractWithCargo> GetAcceptedCargoForWaypoint(string waypointSymbol);
    List<CargoWithDestination> GetAllAcceptedContractCargo();
    List<string> GetAllContracts();
    int GetContractsCount();
    Contract? GetFirstAcceptedContract();
    Contract? GetFirstContract();
}