namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ShipFuelConsumed
{
    public int Id { get; set; }
    public int Amount { get; set; }
    public DateTime Timestamp { get; set; }

    public SpaceTraders.Api.Models.ShipFuelConsumed ToApiModel()
    {
        return new Api.Models.ShipFuelConsumed
        {
            Amount = Amount,
            Timestamp = Timestamp
        };
    }
}

public static class ApiModelShipFuelConsumedExtensions
{
    public static ShipFuelConsumed ToDbModel(this SpaceTraders.Api.Models.ShipFuelConsumed consumed)
    {
        return new ShipFuelConsumed
        {
            Amount = consumed.Amount,
            Timestamp = consumed.Timestamp
        };
    }
}
