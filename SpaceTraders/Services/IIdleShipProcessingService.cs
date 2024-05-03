namespace SpaceTraders.Services;

internal interface IIdleShipProcessingService
{
    Task ProcessIdleShip(string shipSymbol);
}