using System.Text.Json.Serialization;

namespace SpaceTraders.Api.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ShipNavFlightMode
{
    CRUISE,
    DRIFT,
    STEALTH,
    BURN,
}
