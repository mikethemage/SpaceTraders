using System.ComponentModel.DataAnnotations;

namespace SpaceTraders.Models;

public class ClientConfig
{
    [Required]
    public string? TokenFilePath { get; set; }

    [Required]
    public string? ApiUrl { get; set; }
}
