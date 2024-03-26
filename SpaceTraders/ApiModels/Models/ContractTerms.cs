namespace SpaceTraders.ApiModels.Models;

public class ContractTerms
{
    public DateTime Deadline { get; set; }
    public ContractPayment Payment { get; set; } = null!;
    public List<ContractDeliverGood> Deliver { get; set; } = new List<ContractDeliverGood>();
}
