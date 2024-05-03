using SpaceTraders.Models;

namespace SpaceTraders.Repositories;
internal class ShipInfoRepository : IShipInfoRepository
{
    private readonly Dictionary<string, ShipInfo> _shipInfo = new Dictionary<string, ShipInfo>();

    public void InitializeRepository()
    {

    }

    public bool IsShipKnown(string shipSymbol)
    {
        return _shipInfo.ContainsKey(shipSymbol);
    }

    public void AddOrUpdateShipInfo(ShipInfo shipInfo)
    {
        _shipInfo.Remove(shipInfo.ShipSymbol);
        _shipInfo.Add(shipInfo.ShipSymbol, shipInfo);
    }

    public void RemoveShipInfo(string shipSymbol)
    {
        _shipInfo.Remove(shipSymbol);
    }

    public void Clear()
    {
        _shipInfo.Clear();
    }

    public void ShipUpdated(string shipSymbol, DateTime LastUpdated)
    {
        _shipInfo[shipSymbol].LastUpdated = LastUpdated;
    }

    public List<string> GetMissingShips(List<string> apiShipSymbols)
    {
        return _shipInfo.Keys.Where(x => !apiShipSymbols.Contains(x)).ToList();
    }

    public List<string> GetAllMiningShips()
    {
        return _shipInfo.Where(x => x.Value.Role == ShipInfoRole.Miner).Select(x => x.Key).ToList();
    }
}
