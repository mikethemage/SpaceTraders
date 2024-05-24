using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ShipCargo
{
    public int Id { get; set; }
    public int Capacity { get; set; }
    public int Units { get; set; }
    public List<ShipCargoItem> Inventory { get; set; } = new List<ShipCargoItem>();

    [ForeignKey(nameof(Ship))]
    public int ShipId { get; set; }

    public SpaceTraders.Api.Models.ShipCargo ToApiModel()
    {
        return new Api.Models.ShipCargo
        {
            Capacity = Capacity,
            Units = Units,
            Inventory = Inventory.Select(i => i.ToApiModel()).ToList()
        };
    }
}

public static class ApiModelShipCargoExtensions
{
    public static ShipCargo ToDbModel(this SpaceTraders.Api.Models.ShipCargo cargo)
    {
        return new ShipCargo
        {
            Capacity = cargo.Capacity,
            Units = cargo.Units,
            Inventory = cargo.Inventory.Select(i => i.ToDbModel()).ToList()
        };
    }
}
