using SpaceTraders.Api.Models;

namespace SpaceTraders.Api.Responses.ResponseData;

public class RegisterResponseData
{
    public string Token { get; set; } = string.Empty;
    public Agent Agent { get; set; } = null!;
    public Contract Contract { get; set; } = null!;
    public Faction Faction { get; set; } = null!;
    public Ship Ship { get; set; } = null!;
}

