namespace SpaceTraders.ApiModels.Models;

public class Faction
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Headquarters { get; set; } = string.Empty;
    public List<FactionTrait> Traits { get; set; } = new List<FactionTrait>();
    public bool IsRecruiting { get; set; }
}
