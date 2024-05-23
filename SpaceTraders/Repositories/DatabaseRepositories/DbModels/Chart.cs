using System.ComponentModel.DataAnnotations.Schema;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;

public class Chart
{
    public int Id { get; set; }
    public string WaypointSymbol { get; set; } = string.Empty;
    public string SubmittedBy { get; set; } = string.Empty;
    public DateTime SubmittedOn { get; set; }

    [ForeignKey(nameof(Waypoint))]
    public int WaypointId { get; set; }

    public SpaceTraders.Api.Models.Chart ToApiModel()
    {
        return new SpaceTraders.Api.Models.Chart
        {
            WaypointSymbol = WaypointSymbol,
            SubmittedBy = SubmittedBy,
            SubmittedOn = SubmittedOn
        };
    }
}

public static class ApiModelChartExtensions
{
    public static Chart ToDbModel(this SpaceTraders.Api.Models.Chart input)
    {
        return new Chart
        {
            WaypointSymbol = input.WaypointSymbol,
            SubmittedBy = input.SubmittedBy,
            SubmittedOn = input.SubmittedOn
        };
    }
}