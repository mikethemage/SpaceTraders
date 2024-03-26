﻿using SpaceTraders.ApiModels.Models;

namespace SpaceTraders.ApiModels.Responses;

public class RegisterResponseData
{
    public string Token { get; set; } = string.Empty;
    public Agent Agent { get; set; } = null!;
    public Contract Contract { get; set; } = null!;
    public Faction Faction { get; set; } = null!;
    public Ship Ship { get; set; } = null!;
}

