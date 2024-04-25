using Microsoft.Extensions.Logging;
using SpaceTraders.Api.Models;
using SpaceTraders.Api.Requests;
using SpaceTraders.Api.Responses.ResponseData;
using SpaceTraders.Api.Responses.ResponseData.Errors;
using SpaceTraders.Exceptions;
using SpaceTraders.Models;
using SpaceTraders.Services;
using System.Text.Json;

namespace SpaceTraders.Repositories;
internal class ShipManager
{
    private readonly IShipRepository _shipRepository;
    private readonly IShipInfoRepository _shipInfoRepository;
    private readonly IWaypointRepository _waypointRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IMarketRepository _marketRepository;
    private readonly ISpaceTradersApiService _spaceTradersApiService;
    private readonly ILogger<ShipManager> _logger;
    private readonly IAgentRepository _agentRepository;

    public ShipManager(IShipRepository shipRepository, IShipInfoRepository shipInfoRepository, IWaypointRepository waypointRepository, IContractRepository contractRepository, IMarketRepository marketRepository, ISpaceTradersApiService spaceTradersApiService, ILogger<ShipManager> logger, IAgentRepository agentRepository)
    {
        _shipRepository = shipRepository;
        _shipInfoRepository = shipInfoRepository;
        _waypointRepository = waypointRepository;
        _contractRepository = contractRepository;
        _marketRepository = marketRepository;
        _spaceTradersApiService = spaceTradersApiService;
        _logger = logger;
        _agentRepository = agentRepository;
    }

    public async Task RunMiningShip(string shipSymbol)
    {
        ShipNav? nav = await _shipRepository.GetShipNav(shipSymbol);
        if (nav == null)
        {
            throw new Exception("Unable to get ship nav!");
        }
        ShipCargo? cargo = await _shipRepository.GetShipCargo(shipSymbol);
        if (cargo == null)
        {
            throw new Exception("Unable to get ship cargo!");
        }
        WaypointType? currentWaypointType = _waypointRepository.GetWaypointType(nav.SystemSymbol, nav.WaypointSymbol);
        
        if(currentWaypointType == WaypointType.ENGINEERED_ASTEROID && cargo.Units < cargo.Capacity)
        {
            await MineAtCurrentWaypoint(shipSymbol, nav.Status);
        }
        else if(currentWaypointType != WaypointType.ENGINEERED_ASTEROID && cargo.Units == 0)
        {
            await NavigateToNearestWaypointOfType(shipSymbol, nav.SystemSymbol, nav.WaypointSymbol, WaypointType.ENGINEERED_ASTEROID);
        }
        else
        {
            List<CargoWithDestination> contractCargo = await _contractRepository.GetAllAcceptedContractCargo();
            var contractDestinations = contractCargo.Where(x => cargo.Inventory.Any(c => c.Symbol == x.TradeSymbol)).Select(x => x.DestinationWaypointSymbol).Distinct();

            var nonContractCargo = cargo.Inventory.Where(x => !contractCargo.Any(c => c.TradeSymbol == x.Symbol)).Select(x=>x.Symbol).Distinct();

            List<string> nonContractDestinations = new List<string>();
            foreach (var item in nonContractCargo)
            {
                var market = await _marketRepository.GetNearestMarketBuyingGood(nav.SystemSymbol, nav.WaypointSymbol, item);
                if(market != null)
                {
                    nonContractDestinations.Add(market.WaypointSymbol);
                }                
            }
            nonContractDestinations = nonContractDestinations.Distinct().ToList();
            
            if (contractDestinations.Contains(nav.WaypointSymbol))            
            {
                await DeliverContractGoodsAtCurrentWaypoint(shipSymbol, nav);
            }
            else if (nonContractDestinations.Contains(nav.WaypointSymbol))
            {
                await SellGoodsAtCurrentWaypoint(shipSymbol, nav.Status);
            }
            else
            {
                List<string> allDestinations = new List<string>(contractDestinations);
                allDestinations.AddRange(nonContractDestinations);
                await NavigateToNearestWaypoint(shipSymbol, nav.SystemSymbol, nav.WaypointSymbol, allDestinations);
            }            
        }
    }

    private async Task SellGoodsAtCurrentWaypoint(string shipSymbol, ShipNavStatus status)
    {
        if(status!=ShipNavStatus.DOCKED)
        {
            await DockAtCurrentWaypoint(shipSymbol);
        }
        else
        {
            ShipCargo? cargo = await _shipRepository.GetShipCargo(shipSymbol);
            List<CargoWithDestination> contractCargo = await _contractRepository.GetAllAcceptedContractCargo();
            if (cargo!=null)
            {
                foreach (ShipCargoItem cargoItem in cargo.Inventory)
                {
                    if(!contractCargo.Any(x=>x.TradeSymbol==cargoItem.Symbol))
                    {
                        CargoRequest sellCargoRequest = new CargoRequest
                        {
                            Symbol = cargoItem.Symbol,
                            Units = cargoItem.Units
                        };

                        await SellCargo(shipSymbol, sellCargoRequest);
                    }
                }
            }

        }
    }

    private async Task SellCargo(string shipSymbol, CargoRequest sellCargoRequest)
    {
        try
        {
            BuySellCargoResponseData sellCargoResponseData = await _spaceTradersApiService.PostToStarTradersApiWithPayload<BuySellCargoResponseData, CargoRequest>($"my/ships/{shipSymbol}/sell", sellCargoRequest);
            await _shipRepository.UpdateCargo(shipSymbol, sellCargoResponseData.Cargo);
            _agentRepository.Agent = sellCargoResponseData.Agent;
            _logger.LogInformation("Ship {shipSymbol} has sold {sellCargoResponseDataTransactionUnits} of {sellCargoResponseDataTransactionTradeSymbol}", shipSymbol, sellCargoResponseData.Transaction.Units, sellCargoResponseData.Transaction.TradeSymbol);
        }
        catch (StarTradersErrorResponseException ex)
        {
            if (ex.ErrorResponseData is ErrorResponseData<MarketTradeNotSoldError> errorResponseData)
            {
                _logger.LogInformation("Ship {shipSymbol} has failed to sell {tradeSymbol} at {waypointSymbol}.  Error message: \"{errorMessage}\".", shipSymbol, errorResponseData.Data.TradeSymbol, errorResponseData.Data.WaypointSymbol, errorResponseData.Message);
                await JettisonCargo(shipSymbol, sellCargoRequest);
            }
            else
            {
                _logger.LogInformation("Unknown error code:{errorCode} Message: \"{errorMessage}\" Payload: {errorPayload}.", ex.ErrorResponseData.Code, ex.ErrorResponseData.Message, ex.ErrorResponseData.ErrorText);
            }
        }
    }

    private async Task JettisonCargo(string shipSymbol, CargoRequest jettisonCargoRequest)
    {
        JettisonCargoResponseData jettisonCargoResponseData = await _spaceTradersApiService.PostToStarTradersApiWithPayload<JettisonCargoResponseData, CargoRequest>($"my/ships/{shipSymbol}/jettison", jettisonCargoRequest);
        await _shipRepository.UpdateCargo(shipSymbol, jettisonCargoResponseData.Cargo);
    }

    private async Task DockAtCurrentWaypoint(string shipSymbol)
    {
        DockOrbitResponseData dockResponse = await _spaceTradersApiService.PostToStarTradersApi<DockOrbitResponseData>($"my/ships/{shipSymbol}/dock");
        await _shipRepository.UpdateNav(shipSymbol, dockResponse.Nav);
    }

    private async Task DeliverContractGoodsAtCurrentWaypoint(string shipSymbol, ShipNav nav)
    {
        if (nav.Status != ShipNavStatus.DOCKED)
        {
            await DockAtCurrentWaypoint(shipSymbol);
        }
        else
        {
            ShipCargo? cargo = await _shipRepository.GetShipCargo(shipSymbol);
            List<ContractWithCargo> contractCargo = _contractRepository.GetAcceptedCargoForWaypoint(nav.WaypointSymbol);
            if (cargo != null)
            {
                foreach (ShipCargoItem cargoItem in cargo.Inventory)
                {
                    var foundContracts = contractCargo.Where(x => x.TradeSymbol == cargoItem.Symbol);
                    if (foundContracts.Any())
                    {
                        //Deliver Contract Cargo Here!
                        ContractDeliverRequest contractDeliverRequest = new ContractDeliverRequest
                        {
                            shipSymbol = shipSymbol,
                            Symbol = cargoItem.Symbol,
                            Units = cargoItem.Units
                        };

                        string contractId = foundContracts.First().ContractId;

                        await DeliverCargo(contractId, contractDeliverRequest);
                    }
                }
            }
        }
    }

    private async Task DeliverCargo(string contractId, ContractDeliverRequest contractDeliverRequest)
    {
        DeliverCargoContractResponseData responseData = await _spaceTradersApiService.PostToStarTradersApiWithPayload<DeliverCargoContractResponseData, ContractDeliverRequest > (
        $"/my/contracts/{contractId}/deliver", contractDeliverRequest);
        _contractRepository.AddOrUpdateContract(responseData.Contract);
        await _shipRepository.UpdateCargo(contractDeliverRequest.shipSymbol, responseData.Cargo);
    }

    private async Task MineAtCurrentWaypoint(string shipSymbol, ShipNavStatus status)
    {
        if(status!=ShipNavStatus.IN_ORBIT)
        {
            await OrbitCurrentWaypoint(shipSymbol);
        }
        else
        {
            await ExtractAtCurrentWaypoint(shipSymbol);
        }
    }

    private async Task ExtractAtCurrentWaypoint(string shipSymbol)
    {
        ExtractResponseData extractResponseData = await _spaceTradersApiService.PostToStarTradersApi<ExtractResponseData>($"my/ships/{shipSymbol}/extract");
        await _shipRepository.UpdateCargo(shipSymbol, extractResponseData.Cargo);
        await _shipRepository.UpdateCooldown(shipSymbol, extractResponseData.Cooldown);
        _logger.LogInformation("Ship {shipSymbol} has extracted {extractResponseDataExtractionYieldUnits} {extractResponseDataExtractionYieldSymbol}", shipSymbol, extractResponseData.Extraction.Yield.Units, extractResponseData.Extraction.Yield.Symbol);
        _logger.LogInformation("Ship {shipSymbol} is on cooldown until {shipCooldownExpiration}", shipSymbol, extractResponseData.Cooldown.Expiration);

    }

    private async Task OrbitCurrentWaypoint(string shipSymbol)
    {
        DockOrbitResponseData orbitResponse = await _spaceTradersApiService.PostToStarTradersApi<DockOrbitResponseData>($"my/ships/{shipSymbol}/orbit");
        await _shipRepository.UpdateNav(shipSymbol, orbitResponse.Nav);
    }

    private async Task NavigateToNearestWaypoint(string shipSymbol, string systemSymbol, string waypointSymbol, List<string> allDestinations)
    {
        Waypoint? source = _waypointRepository.GetWaypoint(systemSymbol, waypointSymbol);
        if(source == null)
        {
            throw new Exception("Cannot find source waypoint!");
        }
        
        List<Waypoint> destinations = new List<Waypoint>();
        foreach (string destinationSymbol in allDestinations)
        {
            Waypoint? destination = _waypointRepository.GetWaypoint(systemSymbol, destinationSymbol);
            if(destination != null)
            {
                destinations.Add(destination);
            }
        }
        string? nearestDestination = destinations.Select(d => new WaypointWithDistance
        {
            WaypointSymbol = d.Symbol,
            Distance = Math.Sqrt(
                Math.Pow(d.X - source.X,2) +
                Math.Pow(d.Y - source.Y,2)
                )
        }).OrderBy(d=>d.Distance).Select(d=>d.WaypointSymbol).FirstOrDefault();
        if(nearestDestination == null)
        {
            throw new Exception("Cannot find nearest destination!");
        }
        await NavigateToWaypoint(shipSymbol, nearestDestination);
    }

    private async Task NavigateToWaypoint(string shipSymbol, string destinationWaypointSymbol)
    {
        ShipNav? nav = await _shipRepository.GetShipNav(shipSymbol);
        if (nav != null)
        {
            if(nav.Status==ShipNavStatus.DOCKED)
            {
                await OrbitCurrentWaypoint(shipSymbol);
            }
        }
        NavigateRequest navigateRequest = new NavigateRequest { WaypointSymbol = destinationWaypointSymbol };
        NavigateResponseData navigateResponse = await _spaceTradersApiService.PostToStarTradersApiWithPayload<NavigateResponseData, NavigateRequest>($"my/ships/{shipSymbol}/navigate", navigateRequest);
        await _shipRepository.UpdateFuel(shipSymbol, navigateResponse.Fuel);
        await _shipRepository.UpdateNav(shipSymbol, navigateResponse.Nav);
        foreach (ShipConditionEvent shipConditionEvent in navigateResponse.Events)
        {
            _logger.LogInformation("Ship: {shipSymbol} Ship Condition Event: {shipConditionEvent}", shipSymbol, JsonSerializer.Serialize(shipConditionEvent));
        }
        //ShipFuel? fuel = await _shipRepository.GetShipFuel(shipSymbol);
    }

    private async Task NavigateToNearestWaypointOfType(string shipSymbol, string systemSymbol, string sourceWaypointSymbol, WaypointType destinationWaypointType)
    {
        string? destinationSymbol = _waypointRepository.GetNearestWaypointOfType(systemSymbol, sourceWaypointSymbol, destinationWaypointType);
        if(destinationSymbol == null) 
        {
            throw new Exception("Could not find any waypoints!");
        }
        await NavigateToWaypoint(shipSymbol, destinationSymbol);
    }
}
