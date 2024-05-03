namespace SpaceTraders.Models;
internal class ShipInfo
{
    public string ShipSymbol { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public ShipInfoRole Role { get; set; } = ShipInfoRole.None;
}

public enum ShipInfoRole
{
    Miner,
    Surveyor,
    Transport,
    Command,
    None
}
