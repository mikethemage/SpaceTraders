using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class FactionTrait
{
    public int Id { get; set; }

    [ForeignKey(nameof(Faction))]
    public int FactionId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public SpaceTraders.Api.Models.FactionTrait ToApiModel()
    {
        return new SpaceTraders.Api.Models.FactionTrait
        {
            Symbol = Enum.Parse<Api.Models.FactionTraitSymbol>(Symbol),
            Name = Name,
            Description = Description
        };
    }
}

public static class ApiModelFactionTraitExtensions
{
    public static FactionTrait ToDbModel(this SpaceTraders.Api.Models.FactionTrait input)
    {
        return new FactionTrait
        {
            Symbol = input.Symbol.ToString(),
            Name = input.Name,
            Description = input.Description
        };
    }
}