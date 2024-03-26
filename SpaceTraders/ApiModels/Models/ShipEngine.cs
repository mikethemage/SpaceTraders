﻿namespace SpaceTraders.ApiModels.Models;

public class ShipEngine
{
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double Condition { get; set; }
    public double Integrity { get; set; }
    public int Speed { get; set; }
    public ShipRequirements Requirements { get; set; } = null!;
}
