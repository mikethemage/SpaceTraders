using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.ApiModels.Requests;
internal class CargoRequest
{
    public string Symbol { get; set; } = string.Empty;
    public int Units { get; set; }
}
