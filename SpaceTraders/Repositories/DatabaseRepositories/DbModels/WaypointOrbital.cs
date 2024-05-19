namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class WaypointOrbital
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;

    public SpaceTraders.Api.Models.WaypointOrbital ToApiModel()
    {
        return new SpaceTraders.Api.Models.WaypointOrbital
        {
            Symbol = Symbol
        };
    }
}

public static class ApiModelWaypointOrbitalExtensions
{
    public static WaypointOrbital ToDbModel(this SpaceTraders.Api.Models.WaypointOrbital input)
    {
        return new WaypointOrbital
        {
            Symbol = input.Symbol
        };
    }
}
