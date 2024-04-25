using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Models;
internal class CargoWithDestination
{
    public string TradeSymbol { get; set; } = string.Empty;
    public string DestinationWaypointSymbol { get; set; } = string.Empty;
}
