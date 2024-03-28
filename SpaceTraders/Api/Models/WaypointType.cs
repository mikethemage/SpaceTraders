using System.Text.Json.Serialization;

namespace SpaceTraders.Api.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WaypointType
{
    PLANET,
    GAS_GIANT,
    MOON,
    ORBITAL_STATION,
    JUMP_GATE,
    ASTEROID_FIELD,
    ASTEROID,
    ENGINEERED_ASTEROID,
    ASTEROID_BASE,
    NEBULA,
    DEBRIS_FIELD,
    GRAVITY_WELL,
    ARTIFICIAL_GRAVITY_WELL,
    FUEL_STATION
}
