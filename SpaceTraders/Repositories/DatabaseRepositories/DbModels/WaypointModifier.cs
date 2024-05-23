namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class WaypointModifier
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int WaypointId { get; set; }

    public SpaceTraders.Api.Models.WaypointModifier ToApiModel()
    {
        return new SpaceTraders.Api.Models.WaypointModifier
        {
            Symbol = Enum.Parse<SpaceTraders.Api.Models.WaypointModifierSymbol>(Symbol),
            Name = Name,
            Description = Description
        };
    }
}

public static class ApiModelWaypointModifierExtensions
{
    public static WaypointModifier ToDbModel(this SpaceTraders.Api.Models.WaypointModifier input)
    {
        return new WaypointModifier
        {
            Symbol = input.Symbol.ToString(),
            Name = input.Name,
            Description = input.Description
        };
    }
}