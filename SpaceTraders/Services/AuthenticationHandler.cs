using Microsoft.Extensions.Logging;
using SpaceTraders.Repositories;
using System.Net.Http.Headers;

namespace SpaceTraders.Services;

public class AuthenticationHandler : DelegatingHandler
{
    private readonly ITokenRepository _tokenRepository;
    private readonly ILogger<AuthenticationHandler> _logger;

    public AuthenticationHandler(ITokenRepository tokenRepository, ILogger<AuthenticationHandler> logger)
    {
        _tokenRepository = tokenRepository;
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? accessToken = await _tokenRepository.GetTokenAsync();

        if (accessToken != null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
