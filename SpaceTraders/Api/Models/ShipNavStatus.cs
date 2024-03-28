using System.Text.Json.Serialization;

namespace SpaceTraders.Api.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ShipNavStatus
{
    IN_TRANSIT,
    IN_ORBIT,
    DOCKED
}
