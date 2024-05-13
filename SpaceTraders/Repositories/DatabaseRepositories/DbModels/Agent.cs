namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;
public class Agent
{
    public int Id { get; set; }
    public string? AccountId { get; set; } = null;
    public string Symbol { get; set; } = string.Empty;
    public string Headquarters { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string StartingFaction { get; set; } = string.Empty;
    public int ShipCount { get; set; }

    public SpaceTraders.Api.Models.Agent ToApiModel()
    {
        return new Api.Models.Agent
        {
            AccountId = AccountId,
            Symbol = Symbol,
            Headquarters = Headquarters,
            Credits = Credits,
            StartingFaction = StartingFaction,
            ShipCount = ShipCount
        };
    }    
}

public static class ApiModelAgentExtensions
{
    public static Agent ToDbModel(this SpaceTraders.Api.Models.Agent input)
    {
        return new Agent
        {
            AccountId = input.AccountId,
            Symbol = input.Symbol,
            Headquarters = input.Headquarters,
            Credits = input.Credits,
            StartingFaction = input.StartingFaction,
            ShipCount = input.ShipCount
        };
    }
}