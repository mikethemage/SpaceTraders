namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class Ship
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public ShipNav Nav { get; set; } = null!;
    public ShipCrew Crew { get; set; } = null!;
    public ShipFuel Fuel { get; set; } = null!;
    public Cooldown Cooldown { get; set; } = null!;
    public ShipFrame Frame { get; set; } = null!;
    public ShipReactor Reactor { get; set; } = null!;
    public ShipEngine Engine { get; set; } = null!;
    public List<ShipModule> Modules { get; set; } = new List<ShipModule>();
    public List<ShipMount> Mounts { get; set; } = new List<ShipMount>();
    public ShipRegistration Registration { get; set; } = null!;
    public ShipCargo Cargo { get; set; } = null!;

    public SpaceTraders.Api.Models.Ship ToApiModel()
    {
        return new Api.Models.Ship
        {
            Symbol = Symbol,
            Nav = Nav.ToApiModel(),
            Crew = Crew.ToApiModel(),
            Fuel = Fuel.ToApiModel(),
            Cooldown = Cooldown.ToApiModel(),
            Frame = Frame.ToApiModel(),
            Reactor = Reactor.ToApiModel(),
            Engine = Engine.ToApiModel(),
            Modules = Modules.Select(m => m.ToApiModel()).ToList(),
            Mounts = Mounts.Select(m => m.ToApiModel()).ToList(),
            Registration = Registration.ToApiModel(),
            Cargo = Cargo.ToApiModel()
        };
    }
}

public static class ApiModelShipExtensions
{
    public static Ship ToDbModel(this SpaceTraders.Api.Models.Ship ship)
    {
        return new Ship
        {
            Symbol = ship.Symbol,
            Nav = ship.Nav.ToDbModel(),
            Crew = ship.Crew.ToDbModel(),
            Fuel = ship.Fuel.ToDbModel(),
            Cooldown = ship.Cooldown.ToDbModel(),
            Frame = ship.Frame.ToDbModel(),
            Reactor = ship.Reactor.ToDbModel(),
            Engine = ship.Engine.ToDbModel(),
            Modules = ship.Modules.Select(m => m.ToDbModel()).ToList(),
            Mounts = ship.Mounts.Select(m => m.ToDbModel()).ToList(),
            Registration = ship.Registration.ToDbModel(),
            Cargo = ship.Cargo.ToDbModel()
        };
    }
}
