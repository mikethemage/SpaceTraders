using SpaceTraders.ApiModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Repositories;
internal class MarketRepository : IMarketRepository
{
    public List<Market> Markets { get; set; } = new List<Market>();
}
