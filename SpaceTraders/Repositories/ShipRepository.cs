using SpaceTraders.ApiModels.Models;

namespace SpaceTraders.Repositories;
internal class ShipRepository : IShipRepository
{
    public List<Ship> Ships { get; set; } = new List<Ship>();

}
