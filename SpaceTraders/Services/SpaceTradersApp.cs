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

    public SpaceTradersApp(
        IAgentRepository agentRepository,
        IShipRepository shipRepository,
        IContractRepository contractRepository,
        IWaypointRepository waypointRepository,
        ISpaceTradersApiService spaceTradersApiService,
        ITokenRepository tokenRepository,
        IFactionRepository factionRepository,
        ILogger<SpaceTradersApp> logger,
        IMarketRepository marketRepository)
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
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Space Traders API");

        await _tokenRepository.LoadTokenFromFile();

        if (_tokenRepository.Token == string.Empty)
        {
            _logger.LogInformation("Registering new agent");
            await Register();
            _logger.LogInformation("Agent registered");
        }
        else
        {
            _spaceTradersApiService.UpdateToken();
            _logger.LogInformation("Logging In");

            await GetAgent();
            await GetContracts();
            await GetShips();
        }

        // Find current contract
        Contract? currentContract = _contractRepository.Contracts.FirstOrDefault(c => c.Accepted);
        if (currentContract == null)
        {
            Contract? nextContract = _contractRepository.Contracts.FirstOrDefault();
            if (nextContract == null)
            {
                //We have no contracts!!
                throw new Exception("We have no contracts!!!");
            }
            AcceptContractResponseData acceptContractResponseData = await _spaceTradersApiService.PostToStarTradersApi<AcceptContractResponseData>($"my/contracts/{nextContract.Id}/accept");
            _agentRepository.Agent = acceptContractResponseData.Agent;
            _contractRepository.Contracts.Remove(nextContract);
            currentContract = acceptContractResponseData.Contract;
            _contractRepository.Contracts.Add(currentContract);
            _logger.LogInformation("Accepted new contract");
        }

        // By this point we should have an accepted contract

        _logger.LogInformation("Loading waypoints...");
        // Fetch waypoints for each system
        List<string> systemNames = _shipRepository.GetAllSystemsWithShips();
        await Task.WhenAll(systemNames.Select(systemName => GetWaypoints(systemName)));
        _logger.LogInformation("All waypoints loaded");

        while (!cancellationToken.IsCancellationRequested)
        {
            DateTime nextActionTime = DateTime.UtcNow.AddMinutes(2);

            // Check ship status
            foreach (string shipSymbol in _shipRepository.GetAllMiningShips())
            {
                nextActionTime = await ProcessShip(currentContract, nextActionTime, shipSymbol);

            }
            var timeToWait = nextActionTime - DateTime.UtcNow;
            if (timeToWait.TotalMilliseconds > 0)
            {
                await Task.Delay(timeToWait, cancellationToken);
            }
        }
    }

    private async Task<DateTime> ProcessShip(Contract currentContract, DateTime nextActionTime, string shipSymbol)
    {
        Cooldown? cooldown = _shipRepository.GetShipCooldown(shipSymbol);
        if (cooldown == null)
        {
            throw new Exception("Unable to get ship cooldown!");
        }

        if (cooldown.Expiration > DateTime.UtcNow)
        {
            _logger.LogInformation("Ship {shipSymbol} is on cooldown until {shipCooldownExpiration}", shipSymbol, cooldown.Expiration);
            // Ship is on cooldown
            if (cooldown.Expiration < nextActionTime)
            {
                nextActionTime = cooldown.Expiration;
            }
            return nextActionTime;
        }

        ShipNav? nav = _shipRepository.GetShipNav(shipSymbol);
        if (nav == null)
        {
            throw new Exception("Error getting ship nav!");
        }

        if (nav.Status == ShipNavStatus.IN_TRANSIT && nav.Route.Arrival > DateTime.UtcNow)
        {
            _logger.LogInformation("Ship {shipSymbol} is in transit to {shipNavRouteDestinationSymbol}, Eta: {shipNavRouteArrival}", shipSymbol, nav.Route.Destination.Symbol, nav.Route.Arrival);
            // Ship is moving
            if (nav.Route.Arrival < nextActionTime)
            {
                nextActionTime = nav.Route.Arrival;
            }
            return nextActionTime;
        }

        Waypoint? currentLocation = _waypointRepository.Waypoints.Where(w => w.Symbol == nav.WaypointSymbol).FirstOrDefault();
        if (currentLocation == null)
        {
            _logger.LogWarning("Ship is lost in Space!");
            await RefreshShipNav(shipSymbol);
            return nextActionTime;
        }

        ShipFuel? fuel = _shipRepository.GetShipFuel(shipSymbol);
        if (fuel == null)
        {
            throw new Exception("Error getting ship fuel!");
        }

        if (fuel.Current * 4 < fuel.Capacity)
        {
            _logger.LogInformation("Ship {shipSymbol} is low on fuel.", shipSymbol);
            if (currentLocation.Type == WaypointType.FUEL_STATION
                || currentLocation.Traits.Any(t => t.Symbol == WaypointTraitSymbol.MARKETPLACE))
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
                if (DateTime.UtcNow.AddSeconds(2) < nextActionTime)
                {
                    nextActionTime = DateTime.UtcNow.AddSeconds(2);
                }
            }
            else
            {
                if (nav.Status == ShipNavStatus.DOCKED)
                {
                    //Need to undock
                    await OrbitShip(shipSymbol);
                    _logger.LogInformation("Undocked");
                    if (DateTime.UtcNow.AddSeconds(2) < nextActionTime)
                    {
                        nextActionTime = DateTime.UtcNow.AddSeconds(2);
                    }
                }
                else
                {
                    // Need to refuel
                    var possibleFuelStations = _waypointRepository.Waypoints.Where(w => w.SystemSymbol == nav.SystemSymbol).Where(
                        w => w.Type == WaypointType.FUEL_STATION
                        || w.Traits.Where(t => t.Symbol == WaypointTraitSymbol.MARKETPLACE).Any()
                        ).OrderBy(w =>
                        Math.Sqrt(
                        Math.Pow(w.X - currentLocation.X, 2) +
                        Math.Pow(w.Y - currentLocation.Y, 2))
                    );

                    Waypoint? targetFuelStation = possibleFuelStations.FirstOrDefault();
                    if (targetFuelStation == null)
                    {
                        throw new Exception("Can't find fuel station!");
                    }

                    await NavigateShip(shipSymbol, targetFuelStation);                    
                    nav = _shipRepository.GetShipNav(shipSymbol);
                    if (nav == null)
                    {
                        throw new Exception("Error getting ship nav!");
                    }
                    _logger.LogInformation("Ship {shipSymbol} going to refuel at {targetFuelStationSymbol}. Eta: {shipNavRouteArrival}", shipSymbol, targetFuelStation.Symbol, nav.Route.Arrival);
                    if (nav.Route.Arrival < nextActionTime)
                    {
                        nextActionTime = nav.Route.Arrival;
                    }
                }
            }
            return nextActionTime;
        }

        ShipCargo? cargo = _shipRepository.GetShipCargo(shipSymbol);
        if (cargo == null)
        {
            throw new Exception("Error getting ship cargo!");
        }

        bool needToSellCargo = false;
        //Ship is not busy and doesn't need to refuel?:
        //Are we at a Mining Site?
        if (currentLocation.Type == WaypointType.ENGINEERED_ASTEROID)
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
                    cooldown = _shipRepository.GetShipCooldown(shipSymbol);
                    if (cooldown == null)
                    {
                        throw new Exception("Unable to get ship cooldown!");
                    }
                    _logger.LogInformation("Ship {shipSymbol} is on cooldown until {shipCooldownExpiration}", shipSymbol, cooldown.Expiration);
                }
                else
                {
                    //Orbit
                    await OrbitShip(shipSymbol);
                    _logger.LogInformation("Ship {shipSymbol} entered orbit, ready to mine at {currentLocationSymbol}", shipSymbol, currentLocation.Symbol);
                    if (DateTime.UtcNow.AddSeconds(2) < nextActionTime)
                    {
                        nextActionTime = DateTime.UtcNow.AddSeconds(2);
                    }
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
                    if (DateTime.UtcNow.AddSeconds(2) < nextActionTime)
                    {
                        nextActionTime = DateTime.UtcNow.AddSeconds(2);
                    }
                }
                else
                {
                    //Get nearest mining site:
                    var possibleMiningSites = _waypointRepository.Waypoints.Where(w => w.SystemSymbol == nav.SystemSymbol).Where(w => w.Type == WaypointType.ENGINEERED_ASTEROID).OrderBy(w =>
                        Math.Sqrt(
                        Math.Pow(w.X - currentLocation.X, 2) +
                        Math.Pow(w.Y - currentLocation.Y, 2))
                    );

                    Waypoint? targetMiningSite = possibleMiningSites.FirstOrDefault();
                    if (targetMiningSite == null)
                    {
                        throw new Exception("Can't find mining site!");
                    }

                    //Go to Mining Site:
                    await NavigateShip(shipSymbol, targetMiningSite);                    
                    nav = _shipRepository.GetShipNav(shipSymbol);
                    if (nav == null)
                    {
                        throw new Exception("Error getting ship nav!");
                    }
                    _logger.LogInformation("Ship {shipSymbol} going to mine at {targetMiningSiteSymbol}. Eta: {ship.NavRouteArrival}", shipSymbol, targetMiningSite.Symbol, nav.Route.Arrival);
                    if (nav.Route.Arrival < nextActionTime)
                    {
                        nextActionTime = nav.Route.Arrival;
                    }
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
            string destinationWaypoint = "";
            bool haveContractCargo = false;
            //Do we have any of contract cargo???
            foreach (ContractDeliverGood contractDeliverable in currentContract.Terms.Deliver)
            {
                if (cargo.Inventory.Any(x => x.Symbol == contractDeliverable.TradeSymbol))
                {
                    haveContractCargo = true;
                    destinationWaypoint = contractDeliverable.DestinationSymbol;
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
                //Go to nearest sale destination
                var possibleMarkets = _waypointRepository.Waypoints.Where(w => w.Traits.Any(t => t.Symbol == WaypointTraitSymbol.MARKETPLACE)).OrderBy(w =>
                        Math.Sqrt(
                        Math.Pow(w.X - currentLocation.X, 2) +
                        Math.Pow(w.Y - currentLocation.Y, 2))
                    );

                if (!possibleMarkets.Any())
                {
                    throw new Exception("No marketplaces!");
                }

                foreach (Waypoint possibleMarket in possibleMarkets)
                {
                    var marketData = _marketRepository.Markets.Where(m => m.Symbol == possibleMarket.Symbol).FirstOrDefault();
                    if (marketData == null)
                    {
                        //Get market data
                        marketData = await _spaceTradersApiService.GetFromStarTradersApi<Market>($"systems/{possibleMarket.SystemSymbol}/waypoints/{possibleMarket.Symbol}/market");
                        _marketRepository.Markets.Add(marketData);
                    }
                    if (marketData!.Imports.Where(i => i.Symbol == cargoToSell.First().Symbol).Any())
                    {
                        destinationWaypoint = possibleMarket.Symbol;
                        break;
                    }
                }

                if (destinationWaypoint == "")
                {
                    //throw new Exception("No valid destination found!");
                    destinationWaypoint = possibleMarkets.Select(x => x.Symbol).First();
                }
            }

            var destination = _waypointRepository.Waypoints.Where(x => x.Symbol == destinationWaypoint).First();
            if (currentLocation == destination)
            {
                //Dock and sell cargo
                _logger.LogInformation("{shipSymbol} needs to sell cargo!", shipSymbol);

                if (nav.Status != ShipNavStatus.DOCKED)
                {
                    //need to dock
                    await DockShip(shipSymbol);
                    if (DateTime.UtcNow.AddSeconds(2) < nextActionTime)
                    {
                        nextActionTime = DateTime.UtcNow.AddSeconds(2);
                    }
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

                        if (nextActionTime > DateTime.UtcNow.AddSeconds(2))
                        {
                            nextActionTime = DateTime.UtcNow.AddSeconds(2);
                        }
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
                    if (DateTime.UtcNow.AddSeconds(2) < nextActionTime)
                    {
                        nextActionTime = DateTime.UtcNow.AddSeconds(2);
                    }
                }
                else
                {
                    await NavigateShip(shipSymbol, destination);                    
                    nav = _shipRepository.GetShipNav(shipSymbol);
                    if (nav == null)
                    {
                        throw new Exception("Error getting ship nav!");
                    }
                    _logger.LogInformation("Ship {shipSymbol} going to sell cargo at {destinationSymbol}. Eta: {shipNavRouteArrival}", shipSymbol, destination.Symbol, nav.Route.Arrival);
                    if (nav.Route.Arrival < nextActionTime)
                    {
                        nextActionTime = nav.Route.Arrival;
                    }
                }
            }
        }

        return nextActionTime;
    }

    private async Task RefreshShipNav(string shipName)
    {
        ShipNav shipNav = await _spaceTradersApiService.GetFromStarTradersApi<ShipNav>($"my/ships/{shipName}/nav");
        _shipRepository.UpdateNav(shipName, shipNav);
    }

    private async Task SellCargo(string shipSymbol, CargoRequest sellCargoRequest)
    {
        try
        {
            BuySellCargoResponseData sellCargoResponseData = await _spaceTradersApiService.PostToStarTradersApiWithPayload<BuySellCargoResponseData, CargoRequest>($"my/ships/{shipSymbol}/sell", sellCargoRequest);
            _shipRepository.UpdateCargo(shipSymbol, sellCargoResponseData.Cargo);
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
        ShipCargo? cargo = _shipRepository.GetShipCargo(shipSymbol);
        if (cargo != null)
        {
            CargoRequest jettisonCargoRequest = new CargoRequest
            {
                Symbol = tradeSymbol,
                Units = cargo.Inventory.Where(x => x.Symbol == tradeSymbol).Select(x => x.Units).FirstOrDefault()
            };
            JettisonCargoResponseData jettisonCargoResponseData = await _spaceTradersApiService.PostToStarTradersApiWithPayload<JettisonCargoResponseData, CargoRequest>($"my/ships/{shipSymbol}/jettison", jettisonCargoRequest);
            _shipRepository.UpdateCargo(shipSymbol, jettisonCargoResponseData.Cargo);
        }        
    }

    private async Task ExtractWithShip(string shipSymbol)
    {
        ExtractResponseData extractResponseData = await _spaceTradersApiService.PostToStarTradersApi<ExtractResponseData>($"my/ships/{shipSymbol}/extract");
        _shipRepository.UpdateCargo(shipSymbol, extractResponseData.Cargo);
        _shipRepository.UpdateCooldown(shipSymbol, extractResponseData.Cooldown);
        _logger.LogInformation("Ship {shipSymbol} has extracted {extractResponseDataExtractionYieldUnits} {extractResponseDataExtractionYieldSymbol}", shipSymbol, extractResponseData.Extraction.Yield.Units, extractResponseData.Extraction.Yield.Symbol);
    }

    private async Task OrbitShip(string shipSymbol)
    {
        DockOrbitResponseData orbitResponse = await _spaceTradersApiService.PostToStarTradersApi<DockOrbitResponseData>($"my/ships/{shipSymbol}/orbit");
        _shipRepository.UpdateNav(shipSymbol, orbitResponse.Nav);
    }

    private async Task RefuelShip(string shipSymbol)
    {
        RefuelResponseData refuelResponse = await _spaceTradersApiService.PostToStarTradersApi<RefuelResponseData>($"my/ships/{shipSymbol}/refuel");
        _shipRepository.UpdateFuel(shipSymbol, refuelResponse.Fuel);
        _agentRepository.Agent = refuelResponse.Agent;
    }

    private async Task DockShip(string shipSymbol)
    {
        DockOrbitResponseData dockResponse = await _spaceTradersApiService.PostToStarTradersApi<DockOrbitResponseData>($"my/ships/{shipSymbol}/dock");
        _shipRepository.UpdateNav(shipSymbol, dockResponse.Nav);
    }

    private async Task NavigateShip(string shipSymbol, Waypoint destinationWaypoint)
    {
        NavigateRequest navigateRequest = new NavigateRequest { WaypointSymbol = destinationWaypoint.Symbol };
        NavigateResponseData navigateResponse = await _spaceTradersApiService.PostToStarTradersApiWithPayload<NavigateResponseData, NavigateRequest>($"my/ships/{shipSymbol}/navigate", navigateRequest);
        _shipRepository.UpdateFuel(shipSymbol, navigateResponse.Fuel);
        _shipRepository.UpdateNav(shipSymbol, navigateResponse.Nav);
        foreach (ShipConditionEvent shipConditionEvent in navigateResponse.Events)
        {
            _logger.LogInformation("Ship: {shipSymbol} Ship Condition Event: {shipConditionEvent}", shipSymbol, JsonSerializer.Serialize(shipConditionEvent));
        }
    }

    private async Task GetWaypoints(string systemName)
    {
        try
        {
            List<Waypoint> waypoints = await _spaceTradersApiService.GetAllFromStarTradersApi<Waypoint>($"systems/{systemName}/waypoints");
            foreach (Waypoint waypoint in waypoints)
            {
                Waypoint? existingWaypoint = _waypointRepository.Waypoints.Where(w => w.Symbol == waypoint.Symbol).FirstOrDefault();
                if (existingWaypoint != null)
                {
                    _waypointRepository.Waypoints.Remove(existingWaypoint);
                }
                _waypointRepository.Waypoints.Add(waypoint);
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

    private async Task GetShips()
    {
        try
        {
            List<Ship> ships = await _spaceTradersApiService.GetAllFromStarTradersApi<Ship>("my/ships");
            _shipRepository.Clear();
            _shipRepository.AddOrUpdateShips(ships);
            _logger.LogInformation("Loaded Ship Details");
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
            List<Contract> contracts = await _spaceTradersApiService.GetAllFromStarTradersApi<Contract>("my/contracts");
            _contractRepository.Contracts.Clear();
            _contractRepository.Contracts.AddRange(contracts);
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
            _shipRepository.AddOrUpdateShip(registerResponseData.Ship);
            _contractRepository.Contracts.Add(registerResponseData.Contract);

            _spaceTradersApiService.UpdateToken();
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
