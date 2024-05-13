using System.ComponentModel.DataAnnotations;

namespace SpaceTraders.Models;

public class ClientConfig
{    
    [Required]
    public string? ApiUrl { get; set; }

    [Required]
    public string? AgentSymbol { get; set;}

    [Required]
    public string? FactionSymbol { get; set;}
}
