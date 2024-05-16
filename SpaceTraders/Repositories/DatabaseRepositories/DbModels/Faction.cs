namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class Faction
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Headquarters { get; set; } = string.Empty;
    public List<FactionTrait> Traits { get; set; } = new List<FactionTrait>();
    public bool IsRecruiting { get; set; }

    public SpaceTraders.Api.Models.Faction ToApiModel()
    {
        return new Api.Models.Faction
        {
            Symbol = Symbol,
            Name = Name,
            Description = Description,
            Headquarters = Headquarters,
            Traits = Traits.Select(x => x.ToApiModel()).ToList(),
            IsRecruiting = IsRecruiting
        };
    }
}

public static class ApiModelFactionExtensions
{
    public static Faction ToDbModel(this SpaceTraders.Api.Models.Faction input)
    {
        return new Faction
        {
            Symbol = input.Symbol,
            Name = input.Name,
            Description = input.Description,
            Headquarters = input.Headquarters,
            Traits = input.Traits.Select(x => x.ToDbModel()).ToList(),
            IsRecruiting = input.IsRecruiting
        };
    }
}