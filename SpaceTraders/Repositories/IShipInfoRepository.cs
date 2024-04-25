using SpaceTraders.Models;

namespace SpaceTraders.Repositories;
internal interface IShipInfoRepository
{
    void AddOrUpdateShipInfo(ShipInfo shipInfo);
    void Clear();
    List<string> GetAllMiningShips();
    List<string> GetMissingShips(List<string> apiShipSymbols);
    void InitializeRepository();
    bool IsShipKnown(string shipSymbol);
    void RemoveShipInfo(string shipSymbol);
    void ShipUpdated(string shipSymbol, DateTime LastUpdated);
}