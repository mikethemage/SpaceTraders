using System.Text.Json.Serialization;

namespace SpaceTraders.ApiModels.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ShipNavFlightMode
{
    CRUISE, 
    DRIFT,
    STEALTH,
    BURN,
}
