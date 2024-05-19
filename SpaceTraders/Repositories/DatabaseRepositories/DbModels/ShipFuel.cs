namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ShipFuel
{
    public int Id { get; set; }
    public int Current { get; set; }
    public int Capacity { get; set; }
    public ShipFuelConsumed? Consumed { get; set; } = null;

    public SpaceTraders.Api.Models.ShipFuel ToApiModel()
    {
        return new Api.Models.ShipFuel
        {
            Current = Current,
            Capacity = Capacity,
            Consumed = Consumed?.ToApiModel()
        };
    }
}

public static class ApiModelShipFuelExtensions
{
    public static ShipFuel ToDbModel(this SpaceTraders.Api.Models.ShipFuel fuel)
    {
        return new ShipFuel
        {
            Current = fuel.Current,
            Capacity = fuel.Capacity,
            Consumed = fuel.Consumed?.ToDbModel()
        };
    }
}
