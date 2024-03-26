using SpaceTraders.ApiModels.Models;
using SpaceTraders.ApiModels.Requests;
using SpaceTraders.ApiModels.Responses;
using SpaceTraders.Exceptions;
using SpaceTraders.Repositories;
using SpaceTraders.Services;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
        List<string> systemNames = _shipRepository.Ships.Select(ship => ship.Nav.SystemSymbol).Distinct().ToList();
        await Task.WhenAll(systemNames.Select(systemName => GetWaypoints(systemName)));
        _logger.LogInformation("All waypoints loaded");

        while (!cancellationToken.IsCancellationRequested)
        {
            DateTime nextActionTime = DateTime.UtcNow.AddMinutes(2);

            // Check ship status
            foreach (Ship ship in _shipRepository.Ships.Where(s=>s.Cargo.Capacity>0))
            {
                if (ship.Cooldown.Expiration > DateTime.UtcNow)
                {
                    _logger.LogInformation("Ship {shipSymbol} is on cooldown until {shipCooldownExpiration}", ship.Symbol, ship.Cooldown.Expiration);
                    // Ship is on cooldown
                    if (ship.Cooldown.Expiration < nextActionTime)
                    {
                        nextActionTime = ship.Cooldown.Expiration;
                    }
                }
                else if (ship.Nav.Status == ShipNavStatus.IN_TRANSIT && ship.Nav.Route != null && ship.Nav.Route.Arrival > DateTime.UtcNow)
                {
                    _logger.LogInformation("Ship {shipSymbol} is in transit to {shipNavRouteDestinationSymbol}, Eta: {shipNavRouteArrival}", ship.Symbol, ship.Nav.Route.Destination.Symbol, ship.Nav.Route.Arrival);
                    // Ship is moving
                    if (ship.Nav.Route.Arrival < nextActionTime)
                    {
                        nextActionTime = ship.Nav.Route.Arrival;
                    }
                }
                else
                {
                    Waypoint? currentLocation = _waypointRepository.Waypoints.Where(w => w.Symbol == ship.Nav.WaypointSymbol).FirstOrDefault();
                    if (currentLocation == null)
                    {
                        _logger.LogWarning("Ship is lost in Space!");
                        await GetShipNav(ship);
                    }
                    else
                    {
                        if (ship.Fuel.Current * 4 < ship.Fuel.Capacity)
                        {
                            _logger.LogInformation("Ship {shipSymbol} is low on fuel.", ship.Symbol);
                            if (currentLocation.Type == WaypointType.FUEL_STATION
                                || currentLocation.Traits.Any(t=>t.Symbol==WaypointTraitSymbol.MARKETPLACE))
                            {
                                if (ship.Nav.Status == ShipNavStatus.DOCKED)
                                {
                                    //Refuel
                                    await RefuelShip(ship);
                                    _logger.LogInformation("Ship {shipSymbol} refueled.", ship.Symbol);
                                }
                                else
                                {
                                    await DockShip(ship);
                                    _logger.LogInformation("Ship {shipSymbol} docked at refueling station.", ship.Symbol);
                                }
                                if (DateTime.UtcNow.AddSeconds(2) < nextActionTime)
                                {
                                    nextActionTime = DateTime.UtcNow.AddSeconds(2);
                                }
                            }
                            else
                            {
                                if (ship.Nav.Status == ShipNavStatus.DOCKED)
                                {
                                    //Need to undock
                                    await OrbitShip(ship);
                                    _logger.LogInformation("Undocked");
                                    if (DateTime.UtcNow.AddSeconds(2) < nextActionTime)
                                    {
                                        nextActionTime = DateTime.UtcNow.AddSeconds(2);
                                    }
                                }
                                else
                                {
                                    // Need to refuel
                                    var possibleFuelStations = _waypointRepository.Waypoints.Where(w => w.SystemSymbol == ship.Nav.SystemSymbol).Where(
                                        w => w.Type == WaypointType.FUEL_STATION
                                        || w.Traits.Where(t=>t.Symbol==WaypointTraitSymbol.MARKETPLACE).Any()
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

                                    await NavigateShip(ship, targetFuelStation);
                                    _logger.LogInformation("Ship {shipSymbol} going to refuel at {targetFuelStationSymbol}. Eta: {shipNavRouteArrival}", ship.Symbol, targetFuelStation.Symbol, ship.Nav.Route?.Arrival);

                                    if (ship.Nav.Route?.Arrival < nextActionTime)
                                    {
                                        nextActionTime = ship.Nav.Route.Arrival;
                                    }
                                }
                            }
                        }
                        else
                        {
                            bool needToSellCargo = false;
                            //Ship is not busy and doesn't need to refuel?:
                            //Are we at a Mining Site?
                            if (currentLocation.Type == WaypointType.ENGINEERED_ASTEROID)
                            {
                                //Yes:
                                //Is Cargo Bay Full?
                                if (ship.Cargo.Units == ship.Cargo.Capacity)
                                {
                                    //Yes:
                                    //Go to sell cargo
                                    _logger.LogInformation("Ship {shipSymbol} needs to sell cargo", ship.Symbol);
                                    needToSellCargo = true;
                                }
                                else
                                {
                                    //No:
                                    //Keep Mining:
                                    if (ship.Nav.Status == ShipNavStatus.IN_ORBIT)
                                    {
                                        //Extract
                                        await ExtractWithShip(ship);
                                        _logger.LogInformation("Ship {shipSymbol} is on cooldown until {shipCooldownExpiration}", ship.Symbol, ship.Cooldown.Expiration);
                                    }
                                    else
                                    {
                                        //Orbit
                                        await OrbitShip(ship);
                                        _logger.LogInformation("Ship {shipSymbol} entered orbit, ready to mine at {currentLocationSymbol}", ship.Symbol, currentLocation.Symbol);
                                    }
                                }
                            }
                            else
                            {

                                //No:
                                //Is Cargo Bay Empty and is it a cargo ship????
                                if (ship.Cargo.Units == 0 && ship.Cargo.Capacity > 0)
                                {
                                    //Yes:                           
                                    if (ship.Nav.Status == ShipNavStatus.DOCKED)
                                    {
                                        //Need to undock
                                        await OrbitShip(ship);
                                        _logger.LogInformation("Undocked");
                                        if (DateTime.UtcNow.AddSeconds(2) < nextActionTime)
                                        {
                                            nextActionTime = DateTime.UtcNow.AddSeconds(2);
                                        }
                                    }
                                    else
                                    {
                                        //Get nearest mining site:
                                        var possibleMiningSites = _waypointRepository.Waypoints.Where(w => w.SystemSymbol == ship.Nav.SystemSymbol).Where(w => w.Type == WaypointType.ENGINEERED_ASTEROID).OrderBy(w =>
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
                                        await NavigateShip(ship, targetMiningSite);
                                        _logger.LogInformation("Ship {shipSymbol} going to mine at {targetMiningSiteSymbol}. Eta: {ship.NavRouteArrival}", ship.Symbol, targetMiningSite.Symbol, ship.Nav.Route?.Arrival);

                                        if (ship.Nav.Route?.Arrival < nextActionTime)
                                        {
                                            nextActionTime = ship.Nav.Route.Arrival;
                                        }
                                    }

                                }
                                else
                                {
                                    //No:
                                    //Go to sell cargo
                                    _logger.LogInformation("Ship {shipSymbol} needs to sell cargo", ship.Symbol);
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
                                    if (ship.Cargo.Inventory.Any(x => x.Symbol == contractDeliverable.TradeSymbol))
                                    {
                                        haveContractCargo = true;
                                        destinationWaypoint = contractDeliverable.DestinationSymbol;
                                        break;
                                    }
                                }

                                List<ShipCargoItem> cargoToSell = ship.Cargo.Inventory;
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
                                    _logger.LogInformation("{shipSymbol} needs to sell cargo!", ship.Symbol);

                                    if (ship.Nav.Status != ShipNavStatus.DOCKED)
                                    {
                                        //need to dock
                                        await DockShip(ship);
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

                                            await SellCargo(ship, sellCargoRequest);

                                            if (nextActionTime > DateTime.UtcNow.AddSeconds(2))
                                            {
                                                nextActionTime = DateTime.UtcNow.AddSeconds(2);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (ship.Nav.Status == ShipNavStatus.DOCKED)
                                    {
                                        //Need to undock
                                        await OrbitShip(ship);
                                        _logger.LogInformation("Undocked");
                                        if (DateTime.UtcNow.AddSeconds(2) < nextActionTime)
                                        {
                                            nextActionTime = DateTime.UtcNow.AddSeconds(2);
                                        }
                                    }
                                    else
                                    {

                                        await NavigateShip(ship, destination);
                                        _logger.LogInformation("Ship {shipSymbol} going to sell cargo at {destinationSymbol}. Eta: {shipNavRouteArrival}", ship.Symbol, destination.Symbol, ship.Nav.Route?.Arrival);

                                        if (ship.Nav.Route?.Arrival < nextActionTime)
                                        {
                                            nextActionTime = ship.Nav.Route.Arrival;
                                        }
                                    }
                                }
                            }
                        }
                    }       
                }                
            }
            var timeToWait = nextActionTime - DateTime.UtcNow;
            if (timeToWait.TotalMilliseconds > 0)
            {
                await Task.Delay(timeToWait, cancellationToken);
            }
        }

    }

    private async Task GetShipNav(Ship ship)
    {
        ShipNav shipNav = await _spaceTradersApiService.GetFromStarTradersApi<ShipNav>($"my/ships/{ship.Symbol}/nav");
        ship.Nav = shipNav;
    }

    private async Task SellCargo(Ship ship, CargoRequest sellCargoRequest)
    {
        try
        {
            BuySellCargoResponseData sellCargoResponseData = await _spaceTradersApiService.PostToStarTradersApiWithPayload<BuySellCargoResponseData, CargoRequest>($"my/ships/{ship.Symbol}/sell", sellCargoRequest);
            ship.Cargo = sellCargoResponseData.Cargo;
            _agentRepository.Agent = sellCargoResponseData.Agent;
            _logger.LogInformation("Ship {shipSymbol} has sold {sellCargoResponseDataTransactionUnits} of {sellCargoResponseDataTransactionTradeSymbol}", ship.Symbol, sellCargoResponseData.Transaction.Units, sellCargoResponseData.Transaction.TradeSymbol);
        }
        catch (StarTradersErrorResponseException ex) 
        {
            if(ex.ErrorResponseData is ErrorResponseData<MarketTradeNotSoldError> errorResponseData)
            {
                _logger.LogInformation("Ship {shipSymbol} has failed to sell {tradeSymbol} at {waypointSymbol}.  Error message: \"{errorMessage}\".",ship.Symbol, errorResponseData.Data.TradeSymbol, errorResponseData.Data.WaypointSymbol, errorResponseData.Message);
                await JettisonCargo(ship, errorResponseData.Data.TradeSymbol);
            }
            else
            {
                _logger.LogInformation("Unknown error code:{errorCode} Message: \"{errorMessage}\".",ex.ErrorResponseData.Code, ex.ErrorResponseData.Message);
            }
        }
        
    }

    private async Task JettisonCargo(Ship ship, string tradeSymbol)
    {
        CargoRequest jettisonCargoRequest = new CargoRequest 
        {
            Symbol=tradeSymbol,
            Units=ship.Cargo.Inventory.Where(x=>x.Symbol==tradeSymbol).Select(x=>x.Units).FirstOrDefault()
        };
        JettisonCargoResponseData jettisonCargoResponseData = await _spaceTradersApiService.PostToStarTradersApiWithPayload<JettisonCargoResponseData, CargoRequest>($"my/ships/{ship.Symbol}/jettison", jettisonCargoRequest);
        ship.Cargo = jettisonCargoResponseData.Cargo;
    }

    private async Task ExtractWithShip(Ship ship)
    {
        ExtractResponseData extractResponseData = await _spaceTradersApiService.PostToStarTradersApi<ExtractResponseData>($"my/ships/{ship.Symbol}/extract");
        ship.Cargo = extractResponseData.Cargo;
        ship.Cooldown = extractResponseData.Cooldown;
        _logger.LogInformation("Ship {shipSymbol} has extracted {extractResponseDataExtractionYieldUnits} {extractResponseDataExtractionYieldSymbol}", ship.Symbol, extractResponseData.Extraction.Yield.Units, extractResponseData.Extraction.Yield.Symbol);
    }

    private async Task OrbitShip(Ship ship)
    {
        DockOrbitResponseData orbitResponse = await _spaceTradersApiService.PostToStarTradersApi<DockOrbitResponseData>($"my/ships/{ship.Symbol}/orbit");
        ShipNavRoute? previousRoute = ship.Nav.Route;
        ship.Nav = orbitResponse.Nav;
        //if(ship.Nav.Route==null)
        //{
        //    ship.Nav.Route = previousRoute;
        //}
    }

    private async Task RefuelShip(Ship ship)
    {
        RefuelResponseData refuelResponse = await _spaceTradersApiService.PostToStarTradersApi<RefuelResponseData>($"my/ships/{ship.Symbol}/refuel");
        ship.Fuel = refuelResponse.Fuel;
        _agentRepository.Agent = refuelResponse.Agent;
    }

    private async Task DockShip(Ship ship)
    {
        DockOrbitResponseData dockResponse = await _spaceTradersApiService.PostToStarTradersApi<DockOrbitResponseData>($"my/ships/{ship.Symbol}/dock");
        ship.Nav = dockResponse.Nav;
    }

    private async Task NavigateShip(Ship ship, Waypoint destinationWaypoint)
    {
        NavigateRequest navigateRequest = new NavigateRequest { WaypointSymbol = destinationWaypoint.Symbol };
        NavigateResponseData navigateResponse = await _spaceTradersApiService.PostToStarTradersApiWithPayload<NavigateResponseData, NavigateRequest>($"my/ships/{ship.Symbol}/navigate", navigateRequest);
        ship.Fuel = navigateResponse.Fuel;
        ship.Nav = navigateResponse.Nav;
        foreach (ShipConditionEvent shipConditionEvent in navigateResponse.Events)
        {
            _logger.LogInformation("Ship: {shipSymbol} Ship Condition Event: {shipConditionEvent}", ship.Symbol, JsonSerializer.Serialize(shipConditionEvent));
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
            _shipRepository.Ships.Clear();
            _shipRepository.Ships.AddRange(ships);
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
            _shipRepository.Ships.Add(registerResponseData.Ship);
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
