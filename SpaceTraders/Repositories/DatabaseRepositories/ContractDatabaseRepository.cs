using Microsoft.EntityFrameworkCore;
using SpaceTraders.Models;
using SpaceTraders.Repositories.DatabaseRepositories.DbContexts;
using SpaceTraders.Repositories.DatabaseRepositories.DbModels;
using SpaceTraders.Repositories.MemoryOnlyRepositories;

namespace SpaceTraders.Repositories.DatabaseRepositories;

internal class ContractDatabaseRepository : IContractRepository
{
    private readonly IContractMemoryOnlyRepository _contractMemoryOnlyRepository;
    private readonly RepositoryDbContext _repositoryDbContext;

    public ContractDatabaseRepository(IContractMemoryOnlyRepository contractMemoryOnlyRepository, RepositoryDbContext repositoryDbContext)
    {
        _contractMemoryOnlyRepository = contractMemoryOnlyRepository;
        _repositoryDbContext = repositoryDbContext;
    }

    public async Task AddOrUpdateContract(SpaceTraders.Api.Models.Contract contract)
    {
        _contractMemoryOnlyRepository.AddOrUpdateContract(contract);

        IQueryable<Contract> existing = _repositoryDbContext.Contracts.Where(c => c.ContractId == contract.Id);
        if (existing.Any())
        {
            _repositoryDbContext.RemoveRange(existing);
        }
        _repositoryDbContext.Add(contract.ToDbModel());
        await _repositoryDbContext.SaveChangesAsync();
    }

    public async Task AddOrUpdateContracts(List<SpaceTraders.Api.Models.Contract> contracts)
    {
        _contractMemoryOnlyRepository.AddOrUpdateContracts(contracts);

        List<Contract> existing = await _repositoryDbContext.Contracts.Where(c => contracts.Select(x=>x.Id).Contains(c.ContractId)).ToListAsync();
        if (existing.Any())
        {
            _repositoryDbContext.RemoveRange(existing);
        }
        _repositoryDbContext.AddRange(contracts.Select(x => x.ToDbModel()));
        await _repositoryDbContext.SaveChangesAsync();
    }

    public async Task<SpaceTraders.Api.Models.Contract> GetContract(string contractId)
    {
        await EnsureAllContractsLoadedFromDb();

        return _contractMemoryOnlyRepository.GetContract(contractId);
    }

    private async Task EnsureAllContractsLoadedFromDb()
    {
        if (!_contractMemoryOnlyRepository.IsLoaded)
        {
            List<Contract> contracts = await _repositoryDbContext.Contracts
                .Include(x=>x.Terms)
                .ThenInclude(x=>x.Payment)
                .Include(x => x.Terms)
                .ThenInclude(x => x.Deliver)
                .ToListAsync();            

            _contractMemoryOnlyRepository.AddOrUpdateContracts(contracts.Select(x => x.ToApiModel()).ToList());
            _contractMemoryOnlyRepository.IsLoaded = true;
        }
    }

    public async Task<SpaceTraders.Api.Models.Contract?> GetFirstAcceptedContract()
    {
        await EnsureAllContractsLoadedFromDb();
        return _contractMemoryOnlyRepository.GetFirstAcceptedContract();
    }

    public async Task<SpaceTraders.Api.Models.Contract?> GetFirstContract()
    {
        await EnsureAllContractsLoadedFromDb();
        return _contractMemoryOnlyRepository.GetFirstContract();
    }

    public async Task<int> GetContractsCount()
    {
        await EnsureAllContractsLoadedFromDb();
        return _contractMemoryOnlyRepository.GetContractsCount();
    }

    public async Task<List<string>> GetAllContracts()
    {
        await EnsureAllContractsLoadedFromDb();
        return _contractMemoryOnlyRepository.GetAllContracts();
    }

    public async Task<List<CargoWithDestination>> GetAllAcceptedContractCargo()
    {
        await EnsureAllContractsLoadedFromDb();
        return _contractMemoryOnlyRepository.GetAllAcceptedContractCargo();
    }

    public async Task<List<ContractWithCargo>> GetAcceptedCargoForWaypoint(string waypointSymbol)
    {
        await EnsureAllContractsLoadedFromDb();
        return _contractMemoryOnlyRepository.GetAcceptedCargoForWaypoint(waypointSymbol);
    }

    public async Task Clear()
    {
        _contractMemoryOnlyRepository.Clear();

        _repositoryDbContext.RemoveRange(_repositoryDbContext.Contracts);

        await _repositoryDbContext.SaveChangesAsync();
    }
}
