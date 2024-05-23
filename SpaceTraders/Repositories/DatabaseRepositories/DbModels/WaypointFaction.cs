using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class WaypointFaction
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;

    [ForeignKey(nameof(Waypoint))]
    public int WaypointId { get; set; }

    public SpaceTraders.Api.Models.WaypointFaction ToApiModel()
    {
        return new SpaceTraders.Api.Models.WaypointFaction
        {
            Symbol = Symbol
        };
    }
}

public static class ApiModelWaypointFactionExtensions
{
    public static WaypointFaction ToDbModel(this SpaceTraders.Api.Models.WaypointFaction model)
    {
        return new WaypointFaction
        {
            Symbol = model.Symbol
        };
    }
}