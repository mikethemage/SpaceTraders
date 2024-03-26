namespace SpaceTraders.ApiModels.Models;

public class Chart
{
    public string WaypointSymbol { get; set; } = string.Empty;
    public string SubmittedBy { get; set; } = string.Empty;
    public DateTime SubmittedOn { get; set; }
}