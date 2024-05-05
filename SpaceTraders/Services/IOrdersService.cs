namespace SpaceTraders.Services;

internal interface IOrdersService
{
    Task ProcessIdleShip(string shipSymbol);
}