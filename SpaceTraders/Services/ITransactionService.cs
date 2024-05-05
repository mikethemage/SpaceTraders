using SpaceTraders.Api.Models;
using SpaceTraders.Api.Requests;

namespace SpaceTraders.Services;
internal interface ITransactionService
{
    Task RefuelShip(string shipSymbol);
    
    Task SellCargo(string shipSymbol, ShipCargo cargo);
}