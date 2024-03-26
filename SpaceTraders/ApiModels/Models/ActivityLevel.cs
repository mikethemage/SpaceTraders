using System.Text.Json.Serialization;

namespace SpaceTraders.ApiModels.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ActivityLevel
{
    WEAK,
    GROWING,
    STRONG,
    RESTRICTED
}
