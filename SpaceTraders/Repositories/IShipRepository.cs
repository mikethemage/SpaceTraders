using SpaceTraders.ApiModels.Models;

namespace SpaceTraders.Repositories;
internal interface IShipRepository
{
    List<Ship> Ships { get; set; }
}