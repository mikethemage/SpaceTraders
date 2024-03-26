using System.Text.Json.Serialization;

namespace SpaceTraders.ApiModels.Models;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ShipRole
{
    FABRICATOR,
    HARVESTER,
    HAULER,
    INTERCEPTOR,
    EXCAVATOR,
    TRANSPORT,
    REPAIR,
    SURVEYOR,
    COMMAND,
    CARRIER,
    PATROL,
    SATELLITE,
    EXPLORER,
    REFINERY
}
