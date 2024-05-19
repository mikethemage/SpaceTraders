namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class Contract
{
    public int Id { get; set; }
    public string ContractId { get; set; } = string.Empty;
    public string FactionSymbol { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public ContractTerms Terms { get; set; } = null!;
    public bool Accepted { get; set; }
    public bool Fulfilled { get; set; }
    public DateTime? DeadlineToAccept { get; set; }

    public SpaceTraders.Api.Models.Contract ToApiModel()
    {
        return new Api.Models.Contract
        {
            Id = ContractId,
            FactionSymbol = FactionSymbol,
            Type = Type,
            Terms = Terms.ToApiModel(),
            Accepted = Accepted,
            Fulfilled = Fulfilled,
            DeadlineToAccept = DeadlineToAccept
        };
    }
}

public static class ApiModelContractExtensions
{
    public static Contract ToDbModel(this SpaceTraders.Api.Models.Contract input)
    {
        return new Contract
        {
            ContractId = input.Id,
            FactionSymbol=input.FactionSymbol,
            Type = input.Type,
            Terms=input.Terms.ToDbModel(),
            Accepted = input.Accepted,
            Fulfilled=input.Fulfilled,
            DeadlineToAccept=input.DeadlineToAccept
        };
    }
}