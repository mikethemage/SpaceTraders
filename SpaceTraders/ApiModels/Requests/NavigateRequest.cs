using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.ApiModels.Requests;
internal class NavigateRequest
{
    public string WaypointSymbol { get; set; } = string.Empty;
}
