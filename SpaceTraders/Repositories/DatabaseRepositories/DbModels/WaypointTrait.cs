using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class WaypointTrait
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [ForeignKey(nameof(Waypoint))]
    public int WaypointId { get; set; }

    public SpaceTraders.Api.Models.WaypointTrait ToApiModel()
    {
        return new SpaceTraders.Api.Models.WaypointTrait
        {
            Symbol = Enum.Parse<SpaceTraders.Api.Models.WaypointTraitSymbol>(Symbol),
            Name = Name,
            Description = Description
        };
    }
}

public static class ApiModelWaypointTraitExtensions
{
    public static WaypointTrait ToDbModel(this SpaceTraders.Api.Models.WaypointTrait input)
    {
        return new WaypointTrait
        {
            Symbol = input.Symbol.ToString(),
            Name = input.Name,
            Description = input.Description
        };
    }
}
