namespace SpaceTraders.Services;

internal interface ISpaceTradersApiService
{
    Task<List<T>> GetAllFromStarTradersApi<T>(string requestUri);
    Task<T> GetFromStarTradersApi<T>(string requestUri);
    Task<T> PostToStarTradersApi<T>(string requestUri);
    Task<T> PostToStarTradersApiWithPayload<T, U>(string requestUri, U request);    
}