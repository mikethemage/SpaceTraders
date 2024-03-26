namespace SpaceTraders.ApiModels.Models;

public class Contract
{
    public string Id { get; set; } = string.Empty;
    public string FactionSymbol { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public ContractTerms Terms { get; set; } = null!;
    public bool Accepted { get; set; }
    public bool Fulfilled { get; set; }
    [Obsolete("Expiration is deprecated, please use DeadlineToAccept instead.")]
    public DateTime Expiration { get; set; }
    public DateTime DeadlineToAccept { get; set; }
}
