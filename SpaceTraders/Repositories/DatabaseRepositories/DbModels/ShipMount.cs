using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ShipMount
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public int? Strength { get; set; }
    public List<string>? Deposits { get; set; } = null;

    
    public ShipRequirements Requirements { get; set; } = null!;

    [ForeignKey(nameof(ShipRequirements))]
    public int RequirementsId { get; set; }

    [ForeignKey(nameof(Ship))]
    public int ShipId { get; set; }

    public SpaceTraders.Api.Models.ShipMount ToApiModel()
    {
        return new SpaceTraders.Api.Models.ShipMount
        {
            Symbol = Symbol,
            Name = Name,
            Description = Description,
            Strength = Strength,
            Deposits = Deposits,
            Requirements = Requirements.ToApiModel()
        };
    }

}

public static class ApiModelShipMountExtensions
{
    public static ShipMount ToDbModel(this SpaceTraders.Api.Models.ShipMount shipMount)
    {
        return new ShipMount
        {
            Symbol = shipMount.Symbol,
            Name = shipMount.Name,
            Description = shipMount.Description,
            Strength = shipMount.Strength,
            Deposits = shipMount.Deposits,
            Requirements = shipMount.Requirements.ToDbModel()
        };
    }
}