using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTraders.Repositories.DatabaseRepositories.DbModels;
public class Token
{
    public int Id { get; set; }
    public string TokenValue { get; set; } = string.Empty;
}
