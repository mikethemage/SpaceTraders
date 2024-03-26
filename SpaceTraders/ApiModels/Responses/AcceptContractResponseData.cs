using SpaceTraders.ApiModels.Models;

namespace SpaceTraders.ApiModels.Responses;

internal class AcceptContractResponseData
{
    public Agent Agent { get; set; } = null!;
    public Contract Contract { get; set; } = null!;
}