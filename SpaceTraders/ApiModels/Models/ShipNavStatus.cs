using System.Text.Json.Serialization;

namespace SpaceTraders.ApiModels.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ShipNavStatus
{
    IN_TRANSIT,
    IN_ORBIT,
    DOCKED
}
