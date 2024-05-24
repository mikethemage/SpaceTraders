using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ShipCrew
{
    public int Id { get; set; }
    public int Current { get; set; }
    public int Capacity { get; set; }
    public int Required { get; set; }
    public string Rotation { get; set; } = string.Empty;
    public int Morale { get; set; }
    public int Wages { get; set; }

    [ForeignKey(nameof(Ship))]
    public int ShipId { get; set; }

    public SpaceTraders.Api.Models.ShipCrew ToApiModel()
    {
        return new Api.Models.ShipCrew
        {
            Current = Current,
            Capacity = Capacity,
            Required = Required,
            Rotation = Rotation,
            Morale = Morale,
            Wages = Wages
        };
    }
}

public static class ApiModelShipCrewExtensions
{
    public static ShipCrew ToDbModel(this SpaceTraders.Api.Models.ShipCrew crew)
    {
        return new ShipCrew
        {
            Current = crew.Current,
            Capacity = crew.Capacity,
            Required = crew.Required,
            Rotation = crew.Rotation,
            Morale = crew.Morale,
            Wages = crew.Wages
        };
    }
}
