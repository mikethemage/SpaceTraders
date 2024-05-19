namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ShipRegistration
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FactionSymbol { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;

    public SpaceTraders.Api.Models.ShipRegistration ToApiModel()
    {
        return new Api.Models.ShipRegistration
        {
            Name = Name,
            FactionSymbol = FactionSymbol,
            Role = Enum.Parse<Api.Models.ShipRole>(Role)
        };
    }
}

public static class ApiModelShipRegistrationExtensions
{
    public static ShipRegistration ToDbModel(this SpaceTraders.Api.Models.ShipRegistration registration)
    {
        return new ShipRegistration
        {
            Name = registration.Name,
            FactionSymbol = registration.FactionSymbol,
            Role = registration.Role.ToString()
        };
    }
}