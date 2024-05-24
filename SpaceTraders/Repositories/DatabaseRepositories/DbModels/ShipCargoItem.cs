using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ShipCargoItem
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Units { get; set; }

    [ForeignKey(nameof(ShipCargo))]
    public int ShipCargoId { get; set; }

    public SpaceTraders.Api.Models.ShipCargoItem ToApiModel()
    {
        return new SpaceTraders.Api.Models.ShipCargoItem
        {
            Symbol = Symbol,
            Name = Name,
            Description = Description,
            Units = Units
        };
    }
}

public static class ApiModelShipCargoItemExtensions
{
    public static ShipCargoItem ToDbModel(this SpaceTraders.Api.Models.ShipCargoItem item)
    {
        return new ShipCargoItem
        {
            Symbol = item.Symbol,
            Name = item.Name,
            Description = item.Description,
            Units = item.Units
        };
    }
}