using SpaceTraders.ApiModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.ApiModels.Responses;
internal class ExtractResponseData
{
    public Cooldown Cooldown { get; set; } = null!;
    public Extraction Extraction { get; set; } = null!;
    public ShipCargo Cargo { get; set; } = null!;
    public List<ShipConditionEvent> Events { get; set; } = new List<ShipConditionEvent>();
}
