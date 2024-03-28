using System.Text.Json.Serialization;

namespace SpaceTraders.Api.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ActivityLevel
{
    WEAK,
    GROWING,
    STRONG,
    RESTRICTED
}
