using SpaceTraders.Models;
using SpaceTraders.Repositories;
using SpaceTraders.Api.Models;

namespace SpaceTraders.Services;

internal class ContractService : IContractService
{
    private readonly IContractRepository _contractRepository;
    private readonly ISpaceTradersApiService _spaceTradersApiService;

    public ContractService(IContractRepository contractRepository, ISpaceTradersApiService spaceTradersApiService)
    {
        _contractRepository = contractRepository;
        _spaceTradersApiService = spaceTradersApiService;
    }

    public async Task EnsureAllContractsLoaded()
    {
        if (_contractRepository.GetContractsCount() == 0)
        {
            List<Contract> contracts = await _spaceTradersApiService.GetAllFromStarTradersApi<Contract>("my/contracts");
            _contractRepository.AddOrUpdateContracts(contracts);
        }
    }

    public async Task<List<string>> GetAllContracts()
    {
        await EnsureAllContractsLoaded();

        return _contractRepository.GetAllContracts();
    }

    public async Task<List<CargoWithDestination>> GetAllAcceptedContractCargo()
    {
        await EnsureAllContractsLoaded();
        return _contractRepository.GetAllAcceptedContractCargo();
    }

    public List<ContractWithCargo> GetAcceptedCargoForWaypoint(string waypointSymbol)
    {
        return _contractRepository.GetAcceptedCargoForWaypoint(waypointSymbol);
    }

    public Contract? GetFirstAcceptedContract()
    {
        return _contractRepository.GetFirstAcceptedContract();
    }

    public void AddOrUpdateContract(Contract contract)
    {
        _contractRepository.AddOrUpdateContract(contract);
    }

    public Contract? GetFirstContract()
    {
        return _contractRepository.GetFirstContract();
    }
}
