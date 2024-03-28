using SpaceTraders.Api.Models;

namespace SpaceTraders.Api.Responses.ResponseData;

internal class AcceptContractResponseData
{
    public Agent Agent { get; set; } = null!;
    public Contract Contract { get; set; } = null!;
}