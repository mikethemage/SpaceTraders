namespace SpaceTraders.Api.Models;
public class Waypoint
{
    public string Symbol { get; set; } = string.Empty;
    public WaypointType Type { get; set; }
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
}