using Microsoft.Extensions.Logging;
using SpaceTraders.Api.Models;
using SpaceTraders.Api.Responses.ResponseData;
using SpaceTraders.Models;
using SpaceTraders.Repositories;

namespace SpaceTraders.Services;

internal class ContractService : IContractService
{
    private readonly ILogger<ContractService> _logger;
    private readonly IContractRepository _contractRepository;
    private readonly ISpaceTradersApiService _spaceTradersApiService;
    private readonly IAgentRepository _agentRepository;

    public ContractService(IContractRepository contractRepository, ISpaceTradersApiService spaceTradersApiService, ILogger<ContractService> logger, IAgentRepository agentRepository)
    {
        _contractRepository = contractRepository;
        _spaceTradersApiService = spaceTradersApiService;
        _logger = logger;
        _agentRepository = agentRepository;
    }

    public async Task<Contract> GetCurrentContract()
    {
        Contract? currentContract = GetFirstAcceptedContract();
        if (currentContract == null)
        {
            Contract? nextContract = GetFirstContract();
            if (nextContract == null)
            {
                //We have no contracts!!
                throw new Exception("We have no contracts!!!");
            }
            AcceptContractResponseData acceptContractResponseData = await _spaceTradersApiService.PostToStarTradersApi<AcceptContractResponseData>($"my/contracts/{nextContract.Id}/accept");
            _agentRepository.Agent = acceptContractResponseData.Agent;
            currentContract = acceptContractResponseData.Contract;
            AddOrUpdateContract(currentContract);
            _logger.LogInformation("Accepted new contract");
        }

        return currentContract;
    }

    public async Task EnsureAllContractsLoaded()
    {
        if (_contractRepository.GetContractsCount() == 0)
        {
            List<Contract> contracts = await _spaceTradersApiService.GetAllFromStarTradersApi<Contract>("my/contracts");
            _contractRepository.AddOrUpdateContracts(contracts);
        }
    }

    //public async Task<List<string>> GetAllContracts()
    //{
    //    await EnsureAllContractsLoaded();

    //    return _contractRepository.GetAllContracts();
    //}

    public async Task<List<CargoWithDestination>> GetAllAcceptedContractCargo()
    {
        await EnsureAllContractsLoaded();
        return _contractRepository.GetAllAcceptedContractCargo();
    }

    //public List<ContractWithCargo> GetAcceptedCargoForWaypoint(string waypointSymbol)
    //{
    //    return _contractRepository.GetAcceptedCargoForWaypoint(waypointSymbol);
    //}

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
