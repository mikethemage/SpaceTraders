using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceTraders.Api.Models;

namespace SpaceTraders.Repositories;
internal class ContractRepository : IContractRepository
{
    public List<Contract> Contracts { get; set; } = new List<Contract>();
}
