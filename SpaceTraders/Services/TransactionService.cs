using Microsoft.Extensions.Logging;
using SpaceTraders.Api.Models;
using SpaceTraders.Api.Requests;
using SpaceTraders.Api.Responses.ResponseData;
using SpaceTraders.Api.Responses.ResponseData.Errors;
using SpaceTraders.Exceptions;

namespace SpaceTraders.Services;

internal class TransactionService : ITransactionService
{
    private readonly IShipService _shipService;
    private readonly IAgentService _agentService;    
    private readonly ILogger<TransactionService> _logger;
    private readonly ISpaceTradersApiService _spaceTradersApiService;

    public TransactionService(IShipService shipService, ILogger<TransactionService> logger, ISpaceTradersApiService spaceTradersApiService, IAgentService agentService)
    {
        _shipService = shipService;
        _logger = logger;
        _spaceTradersApiService = spaceTradersApiService;
        _agentService = agentService;
    }


    public async Task SellCargo(string shipSymbol, ShipCargo cargo)
    {
        if (cargo.Inventory.Count > 0)
        {
            CargoRequest sellCargoRequest = new CargoRequest
            {
                Symbol = cargo.Inventory.First().Symbol,
                Units = cargo.Inventory.First().Units
            };
            
            try
            {
                BuySellCargoResponseData sellCargoResponseData = await _spaceTradersApiService.PostToStarTradersApiWithPayload<BuySellCargoResponseData, CargoRequest>($"my/ships/{shipSymbol}/sell", sellCargoRequest);
                _shipService.UpdateCargo(shipSymbol, sellCargoResponseData.Cargo);
                _agentService.UpdateAgent(sellCargoResponseData.Agent);
                _logger.LogInformation("Ship {shipSymbol} has sold {sellCargoResponseDataTransactionUnits} of {sellCargoResponseDataTransactionTradeSymbol}", shipSymbol, sellCargoResponseData.Transaction.Units, sellCargoResponseData.Transaction.TradeSymbol);
            }
            catch (StarTradersErrorResponseException ex)
            {
                if (ex.ErrorResponseData is ErrorResponseData<MarketTradeNotSoldError> errorResponseData)
                {
                    _logger.LogError("Ship {shipSymbol} has failed to sell {tradeSymbol} at {waypointSymbol}.  Error message: \"{errorMessage}\".", shipSymbol, errorResponseData.Data.TradeSymbol, errorResponseData.Data.WaypointSymbol, errorResponseData.Message);
                    await _shipService.JettisonCargo(shipSymbol, errorResponseData.Data.TradeSymbol);
                }
                else
                {
                    _logger.LogError("Unknown error code:{errorCode} Message: \"{errorMessage}\" Payload: {errorPayload}.", ex.ErrorResponseData.Code, ex.ErrorResponseData.Message, ex.ErrorResponseData.ErrorText);
                }
            }
        }
    }

    public async Task RefuelShip(string shipSymbol)
    {
        RefuelResponseData refuelResponse = await _spaceTradersApiService.PostToStarTradersApi<RefuelResponseData>($"my/ships/{shipSymbol}/refuel");
        _shipService.UpdateFuel(shipSymbol, refuelResponse.Fuel);
        _agentService.UpdateAgent(refuelResponse.Agent);
    }
}
