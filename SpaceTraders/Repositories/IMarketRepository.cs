using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal interface IMarketRepository
{
    List<Market> Markets { get; set; }
}