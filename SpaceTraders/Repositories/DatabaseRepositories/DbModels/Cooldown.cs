namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class Cooldown
{
    public int Id { get; set; }
    public string ShipSymbol { get; set; } = string.Empty;
    public int TotalSeconds { get; set; }
    public int RemainingSeconds { get; set; }
    public DateTime Expiration { get; set; }

    public SpaceTraders.Api.Models.Cooldown ToApiModel()
    {
        return new Api.Models.Cooldown
        {
            TotalSeconds = TotalSeconds,
            RemainingSeconds = RemainingSeconds,
            Expiration = Expiration
        };
    }
}

public static class ApiModelCooldownExtensions
{
    public static Cooldown ToDbModel(this SpaceTraders.Api.Models.Cooldown cooldown)
    {
        return new Cooldown
        {
            TotalSeconds = cooldown.TotalSeconds,
            RemainingSeconds = cooldown.RemainingSeconds,
            Expiration = cooldown.Expiration
        };
    }
}