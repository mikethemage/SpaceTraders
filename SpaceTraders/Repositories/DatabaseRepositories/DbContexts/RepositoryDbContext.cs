using Microsoft.EntityFrameworkCore;
using SpaceTraders.Repositories.DatabaseRepositories.DbModels;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbContexts;

public class RepositoryDbContext : DbContext
{
    public RepositoryDbContext(DbContextOptions<RepositoryDbContext> options) : base(options)
    {
    }

    public RepositoryDbContext()
    {
    }

    public DbSet<Token> Tokens { get; set; }
    public DbSet<Agent> Agents { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractTerms> ContractTerms { get; set; }
    public DbSet<ContractPayment> ContractPayment { get; set; }
    public DbSet<ContractDeliverGood> ContractDeliverGood { get; set; }
    public DbSet<Faction> Factions { get; set; }
    public DbSet<FactionTrait> FactionTrait { get; set; }
    public DbSet<Market> Markets { get; set; }  
    public DbSet<TradeGood> TradeGood { get; set; }
    public DbSet<MarketTransaction> MarketTransaction { get; set; }
    public DbSet<MarketTradeGood> MarketTradeGood { get; set; }
    public DbSet<Waypoint> Waypoints { get; set; }
    public DbSet<WaypointOrbital> WaypointOrbital { get; set; }
    public DbSet<WaypointFaction> WaypointFaction { get; set; }
    public DbSet<WaypointTrait> WaypointTrait { get; set; }
    public DbSet<WaypointModifier> WaypointModifier { get; set; }
    public DbSet<Chart> Chart { get; set; }
    public DbSet<Ship> Ships { get; set; }
    public DbSet<ShipNav> ShipNav { get; set; }
    public DbSet<ShipNavRoute> ShipNavRoute { get; set; }
    public DbSet<ShipNavRouteWaypoint> ShipNavRouteWaypoint { get; set; }
    public DbSet<ShipCrew> ShipCrew { get; set; }
    public DbSet<ShipFuel> ShipFuel { get; set; }
    public DbSet<ShipFuelConsumed> ShipFuelConsumed { get; set; }
    public DbSet<Cooldown> Cooldown { get; set; }
    public DbSet<ShipFrame> ShipFrame { get; set; }
    public DbSet<ShipReactor> ShipReactor { get; set; }
    public DbSet<ShipEngine> ShipEngine { get; set; }
    public DbSet<ShipModule> ShipModule { get; set; }
    public DbSet<ShipMount> ShipMount { get; set; }
    public DbSet<ShipRequirements> ShipRequirements { get; set; }
    public DbSet<ShipRegistration> ShipRegistration { get; set; }
    public DbSet<ShipCargo> ShipCargo { get; set; }
    public DbSet<ShipCargoItem> ShipCargoItem { get; set; }


}
