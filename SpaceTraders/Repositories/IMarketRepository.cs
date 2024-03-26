using SpaceTraders.ApiModels.Models;

namespace SpaceTraders.Repositories;
internal interface IMarketRepository
{
    List<Market> Markets { get; set; }
}