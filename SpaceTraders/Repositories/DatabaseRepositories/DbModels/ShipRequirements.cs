namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ShipRequirements
{
    public int Id { get; set; }
    public int? ShipEngineId { get; set; }
    public int? ShipFrameId { get; set; }
    public int? ShipModuleId { get; set; }
    public int? ShipMountId { get; set; }
    public int? ShipReactorId { get; set; }


    public int? Power { get; set; }
    public int? Crew { get; set; }
    public int? Slots { get; set; }

    public SpaceTraders.Api.Models.ShipRequirements ToApiModel()
    {
        return new Api.Models.ShipRequirements
        {
            Power = Power,
            Crew = Crew,
            Slots = Slots
        };
    }
}

public static class ApiModelShipRequirementsExtensions
{
    public static ShipRequirements ToDbModel(this SpaceTraders.Api.Models.ShipRequirements requirements)
    {
        return new ShipRequirements
        {
            Power = requirements.Power,
            Crew = requirements.Crew,
            Slots = requirements.Slots
        };
    }
}