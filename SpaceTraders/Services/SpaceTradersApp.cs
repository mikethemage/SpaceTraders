using SpaceTraders.Exceptions;
using SpaceTraders.Repositories;
using SpaceTraders.Services;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpaceTraders.Api.Responses.ResponseData;
using SpaceTraders.Api.Responses.ResponseData.Errors;
using SpaceTraders.Api.Requests;
using SpaceTraders.Api.Models;
using SpaceTraders.Models;

internal class SpaceTradersApp : BackgroundService
{
    private readonly IAgentRepository _agentRepository;
    private readonly IShipRepository _shipRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IWaypointRepository _waypointRepository;
    private readonly ISpaceTradersApiService _spaceTradersApiService;
    private readonly ITokenRepository _tokenRepository;
    private readonly IFactionRepository _factionRepository;
    private readonly ILogger<SpaceTradersApp> _logger;
    private readonly IMarketRepository _marketRepository;
    private readonly IShipInfoRepository _shipInfoRepository;

    public SpaceTradersApp(
        IAgentRepository agentRepository,
        IShipRepository shipRepository,
        IContractRepository contractRepository,
        IWaypointRepository waypointRepository,
        ISpaceTradersApiService spaceTradersApiService,
        ITokenRepository tokenRepository,
        IFactionRepository factionRepository,
        ILogger<SpaceTradersApp> logger,
        IMarketRepository marketRepository,
        IShipInfoRepository shipInfoRepository)
    {
        _agentRepository = agentRepository;
        _shipRepository = shipRepository;
        _contractRepository = contractRepository;
        _waypointRepository = waypointRepository;
        _spaceTradersApiService = spaceTradersApiService;
        _tokenRepository = tokenRepository;
        _factionRepository = factionRepository;
        _logger = logger;
        _marketRepository = marketRepository;
        _shipInfoRepository = shipInfoRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Space Traders API");

        await _tokenRepository.LoadTokenFromFile();

        _shipInfoRepository.InitializeRepository();

        if (_tokenRepository.Token != string.Empty)
        {
            //_spaceTradersApiService.UpdateToken();
            _logger.LogInformation("Logging In");

            try
            {
                await GetAgent();
                await GetContracts();
                //await GetShips();  
            }
            catch (StarTradersErrorResponseException ex)
            {
                if(ex.ErrorResponseData.Code==401)
                {
                    //Unauthorised so try clearing token:
                    _tokenRepository.Token = string.Empty;
                    
                }
                else
                {
                    //We don't know what to do with any other exceptions so rethrow:
                    throw;
                }
            }                      
        }

        if (_tokenRepository.Token == string.Empty)
        {
            _logger.LogInformation("Registering new agent");
            await Register();
            _logger.LogInformation("Agent registered");
        }

        // Find current contract
        Contract? currentContract = _contractRepository.GetFirstAcceptedContract();
        if (currentContract == null)
        {
            Contract? nextContract = _contractRepository.GetFirstContract();
            if (nextContract == null)
            {
                //We have no contracts!!
                throw new Exception("We have no contracts!!!");
            }
            AcceptContractResponseData acceptContractResponseData = await _spaceTradersApiService.PostToStarTradersApi<AcceptContractResponseData>($"my/contracts/{nextContract.Id}/accept");
            _agentRepository.Agent = acceptContractResponseData.Agent;
            currentContract = acceptContractResponseData.Contract;
            _contractRepository.AddOrUpdateContract(currentContract); 
            _logger.LogInformation("Accepted new contract");
        }

        // By this point we should have an accepted contract

        _logger.LogInformation("Loading waypoints...");
        // Fetch waypoints for each system
        List<string> systemSymbols = await _shipRepository.GetAllSystemsWithShips();
        foreach (string systemSymbol in systemSymbols)
        {
            await GetWaypoints(systemSymbol);
        }

        _logger.LogInformation("All waypoints loaded");

        while (!cancellationToken.IsCancellationRequested)
        {
            // Check ship status
            foreach (string shipSymbol in await _shipRepository.GetAllIdleMiningShips())
            {
                await ProcessIdleShip(currentContract, shipSymbol);
            }

            DateTime nextActionTime = await _shipRepository.GetNextAvailabilityTimeForMiningShips();
            var timeToWait = nextActionTime - DateTime.UtcNow;
            if (timeToWait.TotalMilliseconds > 0)
            {
                await Task.Delay(timeToWait, cancellationToken);
            }
        }
    }

    private async Task ProcessIdleShip(Contract currentContract, string shipSymbol)
    {
        ShipNav? nav = await _shipRepository.GetShipNav(shipSymbol);
        if (nav == null)
        {
            throw new Exception("Error getting ship nav!");
        }


        ShipFuel? fuel = await _shipRepository.GetShipFuel(shipSymbol);
        if (fuel == null)
        {
            throw new Exception("Error getting ship fuel!");
        }

        if (fuel.Current * 4 < fuel.Capacity)
        {
            _logger.LogInformation("Ship {shipSymbol} is low on fuel.", shipSymbol);
            if (await _marketRepository.MarketSellsGood(nav.SystemSymbol, nav.WaypointSymbol, "FUEL"))
            {
                if (nav.Status == ShipNavStatus.DOCKED)
                {
                    //Refuel
                    await RefuelShip(shipSymbol);
                    _logger.LogInformation("Ship {shipSymbol} refueled.", shipSymbol);
                }
                else
                {
                    await DockShip(shipSymbol);
                    _logger.LogInformation("Ship {shipSymbol} docked at refueling station.", shipSymbol);
                }
            }
            else
            {
                if (nav.Status == ShipNavStatus.DOCKED)
                {
                    //Need to undock
                    await OrbitShip(shipSymbol);
                    _logger.LogInformation("Undocked");

                }
                else
                {
                    nav = await SendShipToRefuellingStation(shipSymbol, nav, fuel);
                }
            }
            return;
        }

        ShipCargo? cargo = await _shipRepository.GetShipCargo(shipSymbol);
        if (cargo == null)
        {
            throw new Exception("Error getting ship cargo!");
        }

        bool needToSellCargo = false;
        //Ship is not busy and doesn't need to refuel?:
        //Are we at a Mining Site?
        if (_waypointRepository.GetWaypointType(nav.SystemSymbol, nav.WaypointSymbol) == WaypointType.ENGINEERED_ASTEROID)
        {
            //Yes:
            //Is Cargo Bay Full?
            if (cargo.Units == cargo.Capacity)
            {
                //Yes:
                //Go to sell cargo
                _logger.LogInformation("Ship {shipSymbol} needs to sell cargo", shipSymbol);
                needToSellCargo = true;
            }
            else
            {
                //No:
                //Keep Mining:
                if (nav.Status == ShipNavStatus.IN_ORBIT)
                {
                    //Extract
                    await ExtractWithShip(shipSymbol);

                }
                else
                {
                    //Orbit
                    await OrbitShip(shipSymbol);
                    _logger.LogInformation("Ship {ship} entered orbit, ready to mine at {waypoint}", shipSymbol, nav.WaypointSymbol);

                }
            }
        }
        else
        {
            //No:
            //Is Cargo Bay Empty and is it a cargo ship????
            if (cargo.Units == 0 && cargo.Capacity > 0)
            {
                //Yes:                           
                if (nav.Status == ShipNavStatus.DOCKED)
                {
                    //Need to undock
                    await OrbitShip(shipSymbol);
                    _logger.LogInformation("Undocked");

                }
                else
                {
                    string? targetMiningSite = _waypointRepository.GetNearestWaypointOfType(nav.SystemSymbol, nav.WaypointSymbol, WaypointType.ENGINEERED_ASTEROID);
                    if (targetMiningSite == null)
                    {
                        throw new Exception("Can't find mining site!");
                    }

                    //Go to Mining Site:
                    await NavigateShip(shipSymbol, targetMiningSite);
                    nav = await _shipRepository.GetShipNav(shipSymbol);
                    if (nav == null)
                    {
                        throw new Exception("Error getting ship nav!");
                    }
                    _logger.LogInformation("Ship {shipSymbol} going to mine at {targetMiningSiteSymbol}. Eta: {ship.NavRouteArrival}", shipSymbol, targetMiningSite, nav.Route.Arrival);

                }
            }
            else
            {
                //No:
                //Go to sell cargo
                _logger.LogInformation("Ship {shipSymbol} needs to sell cargo", shipSymbol);
                needToSellCargo = true;
            }
        }
        if (needToSellCargo)
        {
            string? destinationWaypointSymbol = null;
            bool haveContractCargo = false;
            //Do we have any of contract cargo???
            foreach (ContractDeliverGood contractDeliverable in currentContract.Terms.Deliver)
            {
                if (cargo.Inventory.Any(x => x.Symbol == contractDeliverable.TradeSymbol))
                {
                    haveContractCargo = true;
                    destinationWaypointSymbol = contractDeliverable.DestinationSymbol;
                    break;
                }
            }

            List<ShipCargoItem> cargoToSell = cargo.Inventory;
            if (haveContractCargo)
            {
                cargoToSell = cargoToSell.Where(x => currentContract.Terms.Deliver.Select(y => y.TradeSymbol).Contains(x.Symbol)).ToList();
            }
            else
            {
                WaypointWithDistance? destinationWaypointWithDistance = await _marketRepository.GetNearestMarketSellingGood(nav.SystemSymbol, nav.WaypointSymbol, cargoToSell.First().Symbol);
                if (destinationWaypointWithDistance == null)
                {
                    destinationWaypointSymbol = null;
                }
                else
                {
                    if (destinationWaypointWithDistance.Distance > fuel.Current)
                    {
                        if (fuel.Current < fuel.Capacity)
                        {
                            nav = await SendShipToRefuellingStation(shipSymbol, nav, fuel);
                            return;
                        }
                        else
                        {
                            await JettisonCargo(shipSymbol, cargoToSell.First().Symbol);
                            return;
                        }
                    }
                    else
                    {
                        destinationWaypointSymbol = destinationWaypointWithDistance.WaypointSymbol;
                    }
                }
            }

            if (destinationWaypointSymbol == null)
            {
                throw new Exception("No marketplaces!");
            }

            //Waypoint? destination = _waypointRepository.GetWaypoint(destinationWaypoint);
            if (nav.WaypointSymbol == destinationWaypointSymbol)
            {
                //Dock and sell cargo
                _logger.LogInformation("{shipSymbol} needs to sell cargo!", shipSymbol);

                if (nav.Status != ShipNavStatus.DOCKED)
                {
                    //need to dock
                    await DockShip(shipSymbol);
                }
                else
                {
                    if (cargoToSell.Count > 0)
                    {
                        //Sell Cargo
                        CargoRequest sellCargoRequest = new CargoRequest
                        {
                            Symbol = cargoToSell.First().Symbol,
                            Units = cargoToSell.First().Units
                        };

                        await SellCargo(shipSymbol, sellCargoRequest);
                    }
                }
            }
            else
            {
                if (nav.Status == ShipNavStatus.DOCKED)
                {
                    //Need to undock
                    await OrbitShip(shipSymbol);
                    _logger.LogInformation("Undocked");
                }
                else
                {
                    await NavigateShip(shipSymbol, destinationWaypointSymbol);
                    nav = await _shipRepository.GetShipNav(shipSymbol);
                    if (nav == null)
                    {
                        throw new Exception("Error getting ship nav!");
                    }
                    _logger.LogInformation("Ship {shipSymbol} going to sell cargo at {destinationSymbol}. Eta: {shipNavRouteArrival}", shipSymbol, destinationWaypointSymbol, nav.Route.Arrival);
                }
            }
        }

        return;
    }

    private async Task<ShipNav> SendShipToRefuellingStation(string shipSymbol, ShipNav nav, ShipFuel fuel)
    {
        // Need to refuel               
        WaypointWithDistance? targetFuelStation = await _marketRepository.GetNearestMarketSellingGood(nav.SystemSymbol, nav.WaypointSymbol, "FUEL");
        if (targetFuelStation == null)
        {
            throw new Exception("No fuel station found!");
        }
        if (nav.WaypointSymbol == targetFuelStation.WaypointSymbol)
        {
            await DockShip(shipSymbol);
        }
        else if (targetFuelStation.Distance > fuel.Current)
        {
            throw new Exception("Not enough fuel to get to refuelling station!");
        }
        else if (nav.Status == ShipNavStatus.DOCKED)
        {
            await OrbitShip(shipSymbol);
        }
        else if (nav.WaypointSymbol != targetFuelStation.WaypointSymbol)
        {
            await NavigateShip(shipSymbol, targetFuelStation.WaypointSymbol);
            ShipNav? newNav = await _shipRepository.GetShipNav(shipSymbol);
            if (newNav == null)
            {
                throw new Exception("Error getting ship nav!");
            }
            _logger.LogInformation("Ship {shipSymbol} going to refuel at {targetFuelStationSymbol}. Eta: {shipNavRouteArrival}", shipSymbol, targetFuelStation, nav.Route.Arrival);

            nav = newNav;
        }

        return nav;
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
                await JettisonCargo(shipSymbol, errorResponseData.Data.TradeSymbol);
            }
            else
            {
                _logger.LogInformation("Unknown error code:{errorCode} Message: \"{errorMessage}\" Payload: {errorPayload}.", ex.ErrorResponseData.Code, ex.ErrorResponseData.Message, ex.ErrorResponseData.ErrorText);
            }
        }

    }

    private async Task JettisonCargo(string shipSymbol, string tradeSymbol)
    {
        ShipCargo? cargo = await _shipRepository.GetShipCargo(shipSymbol);
        if (cargo != null)
        {
            CargoRequest jettisonCargoRequest = new CargoRequest
            {
                Symbol = tradeSymbol,
                Units = cargo.Inventory.Where(x => x.Symbol == tradeSymbol).Select(x => x.Units).FirstOrDefault()
            };
            JettisonCargoResponseData jettisonCargoResponseData = await _spaceTradersApiService.PostToStarTradersApiWithPayload<JettisonCargoResponseData, CargoRequest>($"my/ships/{shipSymbol}/jettison", jettisonCargoRequest);
            await _shipRepository.UpdateCargo(shipSymbol, jettisonCargoResponseData.Cargo);
        }
    }

    private async Task ExtractWithShip(string shipSymbol)
    {
        ExtractResponseData extractResponseData = await _spaceTradersApiService.PostToStarTradersApi<ExtractResponseData>($"my/ships/{shipSymbol}/extract");
        await _shipRepository.UpdateCargo(shipSymbol, extractResponseData.Cargo);
        await _shipRepository.UpdateCooldown(shipSymbol, extractResponseData.Cooldown);
        _logger.LogInformation("Ship {shipSymbol} has extracted {extractResponseDataExtractionYieldUnits} {extractResponseDataExtractionYieldSymbol}", shipSymbol, extractResponseData.Extraction.Yield.Units, extractResponseData.Extraction.Yield.Symbol);
        _logger.LogInformation("Ship {shipSymbol} is on cooldown until {shipCooldownExpiration}", shipSymbol, extractResponseData.Cooldown.Expiration);
    }

    private async Task OrbitShip(string shipSymbol)
    {
        DockOrbitResponseData orbitResponse = await _spaceTradersApiService.PostToStarTradersApi<DockOrbitResponseData>($"my/ships/{shipSymbol}/orbit");
        await _shipRepository.UpdateNav(shipSymbol, orbitResponse.Nav);
    }

    private async Task RefuelShip(string shipSymbol)
    {
        RefuelResponseData refuelResponse = await _spaceTradersApiService.PostToStarTradersApi<RefuelResponseData>($"my/ships/{shipSymbol}/refuel");
        await _shipRepository.UpdateFuel(shipSymbol, refuelResponse.Fuel);
        _agentRepository.Agent = refuelResponse.Agent;
    }

    private async Task DockShip(string shipSymbol)
    {
        DockOrbitResponseData dockResponse = await _spaceTradersApiService.PostToStarTradersApi<DockOrbitResponseData>($"my/ships/{shipSymbol}/dock");
        await _shipRepository.UpdateNav(shipSymbol, dockResponse.Nav);
    }

    private async Task NavigateShip(string shipSymbol, string destinationWaypointSymbol)
    {
        NavigateRequest navigateRequest = new NavigateRequest { WaypointSymbol = destinationWaypointSymbol };
        NavigateResponseData navigateResponse = await _spaceTradersApiService.PostToStarTradersApiWithPayload<NavigateResponseData, NavigateRequest>($"my/ships/{shipSymbol}/navigate", navigateRequest);
        await _shipRepository.UpdateFuel(shipSymbol, navigateResponse.Fuel);
        await _shipRepository.UpdateNav(shipSymbol, navigateResponse.Nav);
        foreach (ShipConditionEvent shipConditionEvent in navigateResponse.Events)
        {
            _logger.LogInformation("Ship: {shipSymbol} Ship Condition Event: {shipConditionEvent}", shipSymbol, JsonSerializer.Serialize(shipConditionEvent));
        }
    }

    private async Task GetWaypoints(string systemSymbol)
    {
        try
        {
            List<Waypoint> waypoints = await _spaceTradersApiService.GetAllFromStarTradersApi<Waypoint>($"systems/{systemSymbol}/waypoints");
            foreach (Waypoint waypoint in waypoints)
            {
                _waypointRepository.AddOrUpdateWaypoint(waypoint);
            }
        }
        catch (StarTradersResponseJsonException ex)
        {
            _logger.LogInformation("JSON Parse Failure: {exception}", ex.Message);
        }
        catch (StarTradersApiFailException ex)
        {
            _logger.LogInformation("API Call Failure: {exception}", ex.Message);
        }
    }



    private async Task GetContracts()
    {
        try
        {            
            await _contractRepository.EnsureAllContractsLoaded();
            _logger.LogInformation("Loaded Contract Details");
        }
        catch (StarTradersResponseJsonException ex)
        {
            _logger.LogInformation("JSON Parse Failure: {exception}", ex.Message);
        }
        catch (StarTradersApiFailException ex)
        {
            _logger.LogInformation("API Call Failure: {exception}", ex.Message);
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
            _logger.LogInformation("JSON Parse Failure: {exception}", ex.Message);
        }
        catch (StarTradersApiFailException ex)
        {
            _logger.LogInformation("API Call Failure: {exception}", ex.Message);
        }
    }

    private async Task Register()
    {
        RegisterRequest request = new RegisterRequest { Symbol = "MiketheMage", Faction = "COSMIC" };

        try
        {
            RegisterResponseData registerResponseData = await _spaceTradersApiService.PostToStarTradersApiWithPayload<RegisterResponseData, RegisterRequest>("register", request);
            _agentRepository.Agent = registerResponseData.Agent;
            _tokenRepository.Token = registerResponseData.Token;
            await _tokenRepository.SaveTokenToFile();

            _factionRepository.Factions.Remove(registerResponseData.Faction.Symbol);
            _factionRepository.Factions.Add(registerResponseData.Faction.Symbol, registerResponseData.Faction);
            //_shipRepository.AddOrUpdateShip(registerResponseData.Ship);
            _contractRepository.AddOrUpdateContract(registerResponseData.Contract);

            
        }
        catch (StarTradersResponseJsonException ex)
        {
            _logger.LogInformation("JSON Parse Failure: {exception}", ex.Message);
        }
        catch (StarTradersApiFailException ex)
        {
            _logger.LogInformation("API Call Failure: {exception}", ex.Message);
        }
    }
}
