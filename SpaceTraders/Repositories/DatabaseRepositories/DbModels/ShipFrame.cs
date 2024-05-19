using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ShipFrame
{
    public int Id { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ModuleSlots { get; set; }
    public int MountingPoints { get; set; }
    public int FuelCapacity { get; set; }
    public double Condition { get; set; }
    public double Integrity { get; set; }

    [ForeignKey("ShipFrameId")]
    public ShipRequirements Requirements { get; set; } = null!;

    public SpaceTraders.Api.Models.ShipFrame ToApiModel()
    {
        return new Api.Models.ShipFrame
        {
            Symbol = Symbol,
            Name = Name,
            Description = Description,
            ModuleSlots = ModuleSlots,
            MountingPoints = MountingPoints,
            FuelCapacity = FuelCapacity,
            Condition = Condition,
            Integrity = Integrity,
            Requirements = Requirements.ToApiModel()
        };
    }
}

public static class ApiModelShipFrameExtensions
{
    public static ShipFrame ToDbModel(this SpaceTraders.Api.Models.ShipFrame frame)
    {
        return new ShipFrame
        {
            Symbol = frame.Symbol,
            Name = frame.Name,
            Description = frame.Description,
            ModuleSlots = frame.ModuleSlots,
            MountingPoints = frame.MountingPoints,
            FuelCapacity = frame.FuelCapacity,
            Condition = frame.Condition,
            Integrity = frame.Integrity,
            Requirements = frame.Requirements.ToDbModel()
        };
    }
}
