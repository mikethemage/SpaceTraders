using System.Text.Json.Serialization;

namespace SpaceTraders.Api.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WaypointModifierSymbol
{
    STRIPPED,
    UNSTABLE,
    RADIATION_LEAK,
    CRITICAL_LIMIT,
    CIVIL_UNREST
}
