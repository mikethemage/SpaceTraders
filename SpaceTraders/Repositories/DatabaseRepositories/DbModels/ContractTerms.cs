using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class ContractTerms
{
    public int Id { get; set; }

    [ForeignKey("Contract")]
    public int ContractId { get; set; }

    public DateTime Deadline { get; set; }
    
    public ContractPayment Payment { get; set; } = null!;
    public List<ContractDeliverGood> Deliver { get; set; } = new List<ContractDeliverGood>();

    public SpaceTraders.Api.Models.ContractTerms ToApiModel()
    {
        return new Api.Models.ContractTerms
        {
            Deadline = Deadline,
            Payment = Payment.ToApiModel(),
            Deliver = Deliver.Select(x => x.ToApiModel()).ToList()
        };
    }
}

public static class ApiModelContractTermsExtensions
{
    public static ContractTerms ToDbModel(this SpaceTraders.Api.Models.ContractTerms input)
    {
        return new ContractTerms
        {
            Deadline = input.Deadline,
            Payment = input.Payment.ToDbModel(),
            Deliver = input.Deliver.Select(x => x.ToDbModel()).ToList()
        };
    }
}
