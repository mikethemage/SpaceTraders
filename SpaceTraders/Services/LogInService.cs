using Microsoft.Extensions.Logging;
using SpaceTraders.Api.Models;
using SpaceTraders.Api.Requests;
using SpaceTraders.Api.Responses.ResponseData;
using SpaceTraders.Exceptions;
using SpaceTraders.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Services;
internal class LogInService : ILogInService
{
    private readonly ILogger<LogInService> _logger;
    private readonly ITokenRepository _tokenRepository;
    private readonly IContractService _contractService;
    private readonly IFactionRepository _factionRepository;
    private readonly ISpaceTradersApiService _spaceTradersApiService;
    private readonly IAgentRepository _agentRepository;

    public LogInService(ILogger<LogInService> logger, ITokenRepository tokenRepository, IContractService contractService, IFactionRepository factionRepository, ISpaceTradersApiService spaceTradersApiService, IAgentRepository agentRepository)
    {
        _logger = logger;
        _tokenRepository = tokenRepository;
        _contractService = contractService;
        _factionRepository = factionRepository;
        _spaceTradersApiService = spaceTradersApiService;
        _agentRepository = agentRepository;
    }

    public async Task LogIn()
    {
        if (await _tokenRepository.GetTokenAsync() != null)
        {
            //_spaceTradersApiService.UpdateToken();
            _logger.LogInformation("Logging In");

            try
            {
                await GetAgent();
                await _contractService.EnsureAllContractsLoaded();
            }
            catch (StarTradersErrorResponseException ex)
            {
                if (ex.ErrorResponseData.Code == 401)
                {
                    //Unauthorised so try clearing token:
                    await _tokenRepository.UpdateTokenAsync(null);
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

    private async Task GetAgent()
    {
        try
        {
            Agent agent = await _spaceTradersApiService.GetFromStarTradersApi<Agent>("my/agent");
            _agentRepository.Agent = agent;
            _logger.LogInformation("Loaded Agent Details");
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

    private async Task Register()
    {
        RegisterRequest request = new RegisterRequest { Symbol = "MiketheMage", Faction = "COSMIC" };

        try
        {
            RegisterResponseData registerResponseData = await _spaceTradersApiService.PostToStarTradersApiWithPayload<RegisterResponseData, RegisterRequest>("register", request);
            _agentRepository.Agent = registerResponseData.Agent;
            await _tokenRepository.UpdateTokenAsync(registerResponseData.Token);
            _factionRepository.Factions.Remove(registerResponseData.Faction.Symbol);
            _factionRepository.Factions.Add(registerResponseData.Faction.Symbol, registerResponseData.Faction);
            _contractService.AddOrUpdateContract(registerResponseData.Contract);
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
