using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceTraders.Api.Models;
using SpaceTraders.Models;
using SpaceTraders.Services;

namespace SpaceTraders.Repositories;
internal class ContractRepository : IContractRepository
{
    private readonly Dictionary<string, Contract> _contracts = new Dictionary<string, Contract>();
    private readonly ISpaceTradersApiService _spaceTradersApiService;

    public ContractRepository(ISpaceTradersApiService spaceTradersApiService)
    {
        _spaceTradersApiService = spaceTradersApiService;
    }

    public void AddOrUpdateContract(Contract contract)
    {
        if(_contracts.ContainsKey(contract.Id))
        {
            _contracts.Remove(contract.Id);
        }
        _contracts.Add(contract.Id, contract);
    }

    public void AddOrUpdateContracts(List<Contract> contracts)
    {
        foreach (Contract contract in contracts)
        {
            AddOrUpdateContract(contract);
        }
    }

    public Contract GetContract(string contractId)
    {
        return _contracts[contractId];
    }

    private async Task EnsureAllContractsLoaded()
    {
        if (_contracts.Count == 0)
        {
            List<Contract> contracts = await _spaceTradersApiService.GetAllFromStarTradersApi<Contract>("my/contracts");
            AddOrUpdateContracts(contracts);
        }        
    }

    public async Task<List<string>> GetAllContracts()
    {
        await EnsureAllContractsLoaded();

        return _contracts.Keys.ToList();
    }

    public async Task<List<CargoWithDestination>> GetAllAcceptedContractCargo()
    {
        await EnsureAllContractsLoaded();
        var acceptedContacts = _contracts.Values.Where(c => c.Accepted == true);
        var allCargo = acceptedContacts.SelectMany(x => x.Terms.Deliver);
        var goods = allCargo.Where(x=>x.UnitsFulfilled < x.UnitsRequired).Select(x=>new CargoWithDestination { TradeSymbol= x.TradeSymbol, DestinationWaypointSymbol=x.DestinationSymbol }).ToList();
        return goods;
    }

    public List<ContractWithCargo> GetAcceptedCargoForWaypoint(string waypointSymbol)
    {
        //return
        return _contracts.SelectMany(x => 
            x.Value.Terms.Deliver.Where(y => y.DestinationSymbol == waypointSymbol).Select(z=>
            new ContractWithCargo
            {
                ContractId = x.Key,
                TradeSymbol=z.TradeSymbol
            })               
        ).ToList();
    }
}
