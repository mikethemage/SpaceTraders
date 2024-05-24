using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ShipReactor
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Condition { get; set; }
    public double Integrity { get; set; }
    public int PowerOutput { get; set; }

    
    public ShipRequirements Requirements { get; set; } = null!;

    [ForeignKey(nameof(ShipRequirements))]
    public int RequirementsId { get; set; }

    [ForeignKey(nameof(Ship))]
    public int ShipId { get; set; }

    public SpaceTraders.Api.Models.ShipReactor ToApiModel()
    {
        return new Api.Models.ShipReactor
        {
            Symbol = Symbol,
            Name = Name,
            Description = Description,
            Condition = Condition,
            Integrity = Integrity,
            PowerOutput = PowerOutput,
            Requirements = Requirements.ToApiModel()
        };
    }
}

public static class ApiModelShipReactorExtensions
{
    public static ShipReactor ToDbModel(this SpaceTraders.Api.Models.ShipReactor reactor)
    {
        return new ShipReactor
        {
            Symbol = reactor.Symbol,
            Name = reactor.Name,
            Description = reactor.Description,
            Condition = reactor.Condition,
            Integrity = reactor.Integrity,
            PowerOutput = reactor.PowerOutput,
            Requirements = reactor.Requirements.ToDbModel()
        };
    }
}
