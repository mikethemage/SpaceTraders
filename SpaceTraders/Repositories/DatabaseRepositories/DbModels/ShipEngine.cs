using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ShipEngine
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Condition { get; set; }
    public double Integrity { get; set; }
    public int Speed { get; set; }

    
    public ShipRequirements Requirements { get; set; } = null!;

    [ForeignKey(nameof(ShipRequirements))]
    public int RequirementsId { get; set; }

    [ForeignKey(nameof(Ship))]
    public int ShipId { get; set; }

    public SpaceTraders.Api.Models.ShipEngine ToApiModel()
    {
        return new Api.Models.ShipEngine
        {
            Symbol = Symbol,
            Name = Name,
            Description = Description,
            Condition = Condition,
            Integrity = Integrity,
            Speed = Speed,
            Requirements = Requirements.ToApiModel()
        };
    }
}

public static class ApiModelShipEngineExtensions
{
    public static ShipEngine ToDbModel(this SpaceTraders.Api.Models.ShipEngine engine)
    {
        return new ShipEngine
        {
            Symbol = engine.Symbol,
            Name = engine.Name,
            Description = engine.Description,
            Condition = engine.Condition,
            Integrity = engine.Integrity,
            Speed = engine.Speed,
            Requirements = engine.Requirements.ToDbModel()
        };
    }
}
