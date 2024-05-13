namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ContractPayment
{
    public int Id { get; set; }
    public int OnAccepted { get; set; }
    public int OnFulfilled { get; set; }

    public SpaceTraders.Api.Models.ContractPayment ToApiModel()
    {
        return new Api.Models.ContractPayment
        {
            OnAccepted = OnAccepted,
            OnFulfilled = OnFulfilled
        };
    }
}

public static class ApiModelContractPaymentExtensions
{
    public static ContractPayment ToDbModel(this SpaceTraders.Api.Models.ContractPayment input)
    {
        return new ContractPayment
        {
            OnAccepted = input.OnAccepted,
            OnFulfilled = input.OnFulfilled,
        };
    }
}