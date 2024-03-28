using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Api.Requests;
internal class RegisterRequest
{
    public string Symbol { get; set; } = string.Empty;
    public string Faction { get; set; } = string.Empty;
}
