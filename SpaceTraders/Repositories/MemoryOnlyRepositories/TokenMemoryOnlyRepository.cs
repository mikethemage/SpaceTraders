﻿namespace SpaceTraders.Repositories.MemoryOnlyRepositories;

public class TokenMemoryOnlyRepository : ITokenMemoryOnlyRepository
{    
    private string? _token = null;

    public bool IsLoaded { get; set; } = false;

    public string? GetToken()
    {
        return _token;
    }

    public void UpdateToken(string? token)
    {
        _token = token;
        IsLoaded = true;
    }
}
