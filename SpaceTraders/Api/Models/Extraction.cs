namespace SpaceTraders.Api.Models;

public class Extraction
{
    public string ShipSymbol { get; set; } = string.Empty;
    public ExtractionYield Yield { get; set; } = null!;
}