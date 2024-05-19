namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class Waypoint
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string SystemSymbol { get; set; } = string.Empty;
    public int X { get; set; }
    public int Y { get; set; }
    public List<WaypointOrbital> Orbitals { get; set; } = new List<WaypointOrbital>();
    public string Orbits { get; set; } = string.Empty;
    public WaypointFaction? Faction { get; set; } = null;
    public List<WaypointTrait> Traits { get; set; } = new List<WaypointTrait>();
    public List<WaypointModifier> Modifiers { get; set; } = new List<WaypointModifier>();
    public Chart Chart { get; set; } = null!;
    public bool IsUnderConstruction { get; set; }

    public SpaceTraders.Api.Models.Waypoint ToApiModel()
    {
        return new SpaceTraders.Api.Models.Waypoint
        {            
            Symbol = Symbol,
            Type = Enum.Parse<SpaceTraders.Api.Models.WaypointType>(Type),
            SystemSymbol = SystemSymbol,
            X = X,
            Y = Y,
            Orbitals = Orbitals.Select(o => o.ToApiModel()).ToList(),
            Orbits = Orbits,
            Faction = Faction?.ToApiModel(),
            Traits = Traits.Select(t => t.ToApiModel()).ToList(),
            Modifiers = Modifiers.Select(m => m.ToApiModel()).ToList(),
            Chart = Chart.ToApiModel(),
            IsUnderConstruction = IsUnderConstruction
        };
    }
}

public static class ApiModelWaypointExtensions
{
    public static Waypoint ToDbModel(this SpaceTraders.Api.Models.Waypoint input)
    {
        return new Waypoint
        {
            Symbol = input.Symbol,
            Type = input.Type.ToString(),
            SystemSymbol = input.SystemSymbol,
            X = input.X,
            Y = input.Y,
            Orbitals = input.Orbitals.Select(o => o.ToDbModel()).ToList(),
            Orbits = input.Orbits,
            Faction = input.Faction?.ToDbModel(),
            Traits = input.Traits.Select(t => t.ToDbModel()).ToList(),
            Modifiers = input.Modifiers.Select(m => m.ToDbModel()).ToList(),
            Chart = input.Chart.ToDbModel(),
            IsUnderConstruction = input.IsUnderConstruction
        };
    }    
}