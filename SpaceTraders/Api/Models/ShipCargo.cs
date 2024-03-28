namespace SpaceTraders.Api.Models;

public class ShipCargo
{
    public int Capacity { get; set; }
    public int Units { get; set; }
    public List<ShipCargoItem> Inventory { get; set; } = new List<ShipCargoItem>();
}
