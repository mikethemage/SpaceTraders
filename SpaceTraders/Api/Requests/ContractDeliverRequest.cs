using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Api.Requests;
internal class ContractDeliverRequest
{
    public string shipSymbol { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public int Units { get; set; }
}
