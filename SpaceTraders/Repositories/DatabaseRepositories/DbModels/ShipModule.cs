using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ShipModule
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? Capacity { get; set; }
    public int? Range { get; set; }

   
    public ShipRequirements Requirements { get; set; } = null!;

    [ForeignKey(nameof(ShipRequirements))]
    public int RequirementsId { get; set; }

    [ForeignKey(nameof(Ship))]
    public int ShipId { get; set; }

    public SpaceTraders.Api.Models.ShipModule ToApiModel()
    {
        return new Api.Models.ShipModule
        {
            Symbol = Symbol,
            Name = Name,
            Description = Description,
            Capacity = Capacity,
            Range = Range,
            Requirements = Requirements.ToApiModel()
        };
    }
}

public static class ApiModelShipModuleExtensions
{
    public static ShipModule ToDbModel(this SpaceTraders.Api.Models.ShipModule shipModule)
    {
        return new ShipModule
        {
            Symbol = shipModule.Symbol,
            Name = shipModule.Name,
            Description = shipModule.Description,
            Capacity = shipModule.Capacity,
            Range = shipModule.Range,
            Requirements = shipModule.Requirements.ToDbModel()
        };
    }
}
