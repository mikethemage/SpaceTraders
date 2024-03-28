using SpaceTraders.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Api.Responses.ResponseData;
internal class NavigateResponseData
{
    public ShipFuel Fuel { get; set; } = null!;
    public ShipNav Nav { get; set; } = null!;
    public List<ShipConditionEvent> Events { get; set; } = new List<ShipConditionEvent>();
}
