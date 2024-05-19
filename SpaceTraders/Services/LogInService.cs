using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SpaceTraders.Api.Requests;
using SpaceTraders.Api.Responses.ResponseData;
using SpaceTraders.Exceptions;
using SpaceTraders.Models;
using SpaceTraders.Repositories;

namespace SpaceTraders.Services;

internal class LogInService : ILogInService
{
    private readonly ILogger<LogInService> _logger;
    private readonly ClientConfig _clientConfig;
    private readonly IAgentService _agentService;
    private readonly IContractService _contractService;
    private readonly IFactionService _factionService;
    private readonly IShipService _shipService;
    private readonly ISpaceTradersApiService _spaceTradersApiService;
    private readonly ITokenRepository _tokenRepository;
    private readonly IWaypointService _waypointService;

    public LogInService(
        ILogger<LogInService> logger,
        ITokenRepository tokenRepository,
        IContractService contractService,
        ISpaceTradersApiService spaceTradersApiService,
        IAgentService agentService,
        IFactionService factionService,
        IShipService shipService,
        IWaypointService waypointService,
        IOptions<ClientConfig> options)
    {
        _logger = logger;
        _tokenRepository = tokenRepository;
        _agentService = agentService;
        _contractService = contractService;
        _shipService = shipService;
        _spaceTradersApiService = spaceTradersApiService;
        _factionService = factionService;
        _waypointService = waypointService;
        _clientConfig = options.Value;
    }

    public async Task LogIn()
    {
        if (await _tokenRepository.GetTokenAsync() != null)
        {
            //_spaceTradersApiService.UpdateToken();
            _logger.LogInformation("Logging In");

            try
            {                
                await _contractService.EnsureAllContractsLoaded();
            }
            catch (StarTradersErrorResponseException ex)
            {
                if (ex.ErrorResponseData.Code == 401)
                {
                    //Unauthorised so clear token and data so we can start fresh:
                    await _tokenRepository.UpdateTokenAsync(null);
                    await _agentService.UpdateAgent(null); 
                    await _contractService.Clear();
                    await _factionService.Clear();                    
                    _shipService.Clear();
                    await _waypointService.Clear();
                }
                else
                {
                    //We don't know what to do with any other exceptions so rethrow:
                    throw;
                }
            }
        }

        if (await _tokenRepository.GetTokenAsync() == null)
        {
            _logger.LogInformation("Registering new agent");
            await Register();
            _logger.LogInformation("Agent registered");
        }
    }    

    private async Task Register()
    {
        if(_clientConfig.AgentSymbol is null || _clientConfig.FactionSymbol is null)
        {
            throw new Exception("Missing agent/faction details in config!");
        }

        RegisterRequest request = new RegisterRequest { Symbol = _clientConfig.AgentSymbol, Faction = _clientConfig.FactionSymbol };

        try
        {
            RegisterResponseData registerResponseData = await _spaceTradersApiService.PostToStarTradersApiWithPayload<RegisterResponseData, RegisterRequest>("register", request);
            await _tokenRepository.UpdateTokenAsync(registerResponseData.Token); 
            await _agentService.UpdateAgent(registerResponseData.Agent);            
            await _factionService.AddOrUpdateFaction(registerResponseData.Faction);            
            await _contractService.AddOrUpdateContract(registerResponseData.Contract);
        }
        catch (StarTradersResponseJsonException ex)
        {
            _logger.LogError("JSON Parse Failure: {exception}", ex.Message);
        }
        catch (StarTradersApiFailException ex)
        {
            _logger.LogError("API Call Failure: {exception}", ex.Message);
        }
    }
}
