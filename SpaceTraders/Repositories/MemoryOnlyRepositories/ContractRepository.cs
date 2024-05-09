using SpaceTraders.Api.Models;
using SpaceTraders.Models;

namespace SpaceTraders.Repositories.MemoryOnlyRepositories;
internal class ContractRepository : IContractRepository
{
    private readonly Dictionary<string, Contract> _contracts = new Dictionary<string, Contract>();

    public ContractRepository()
    {

    }

    public void AddOrUpdateContract(Contract contract)
    {
        if (_contracts.ContainsKey(contract.Id))
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

    public Contract? GetFirstAcceptedContract()
    {
        return _contracts.Where(c => c.Value.Accepted == true).Select(c => c.Value).FirstOrDefault();
    }

    public Contract? GetFirstContract()
    {
        return _contracts.Select(c => c.Value).FirstOrDefault();
    }

    public int GetContractsCount()
    {
        return _contracts.Count;
    }

    public List<string> GetAllContracts()
    {
        return _contracts.Keys.ToList();
    }

    public List<CargoWithDestination> GetAllAcceptedContractCargo()
    {
        var acceptedContacts = _contracts.Values.Where(c => c.Accepted == true);
        var allCargo = acceptedContacts.SelectMany(x => x.Terms.Deliver);
        var goods = allCargo.Where(x => x.UnitsFulfilled < x.UnitsRequired).Select(x => new CargoWithDestination { TradeSymbol = x.TradeSymbol, DestinationWaypointSymbol = x.DestinationSymbol }).ToList();
        return goods;
    }

    public List<ContractWithCargo> GetAcceptedCargoForWaypoint(string waypointSymbol)
    {
        return _contracts.SelectMany(x =>
            x.Value.Terms.Deliver.Where(y => y.DestinationSymbol == waypointSymbol).Select(z =>
            new ContractWithCargo
            {
                ContractId = x.Key,
                TradeSymbol = z.TradeSymbol
            })
        ).ToList();
    }


}
