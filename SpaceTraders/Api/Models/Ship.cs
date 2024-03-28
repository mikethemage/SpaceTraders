namespace SpaceTraders.Api.Models;

public class Ship
{
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
}
