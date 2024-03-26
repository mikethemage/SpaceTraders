using SpaceTraders.ApiModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.ApiModels.Responses;
internal class NavigateResponseData
{
    public ShipFuel Fuel { get; set; } = null!;
    public ShipNav Nav { get; set; } = null!;
    public List<ShipConditionEvent> Events { get; set; } = new List<ShipConditionEvent>();
}
