using SpaceTraders.Api.Responses;
using SpaceTraders.Api.Responses.ResponseData;
using SpaceTraders.Exceptions;
using SpaceTraders.Repositories;
using System.Net.Http.Json;

namespace SpaceTraders.Services;

internal class SpaceTradersApiService : ISpaceTradersApiService
{
    private readonly HttpClient _httpClient;
    private readonly IErrorDecoder _errorDecoder;
    private readonly IThrottleService _throttleService;       

    public SpaceTradersApiService(HttpClient httpClient, IErrorDecoder errorDecoder, IThrottleService throttleService)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.spacetraders.io/v2/");
        _errorDecoder = errorDecoder;
        _throttleService = throttleService;   
    }
       
    public async Task<List<T>> GetAllFromStarTradersApi<T>(string requestUri)
    {
        int page = 1;
        bool keepGoing;
        List<T> output = new List<T>();

        do
        {
            keepGoing = false;
            await _throttleService.Throttle();
           
            using HttpResponseMessage response = await _httpClient.GetAsync($"{requestUri}?page={page}");            

            if (response.IsSuccessStatusCode)
            {                
                SuccessResponse<List<T>>? registerResponse = await response.Content.ReadFromJsonAsync<SuccessResponse<List<T>>>();
                if (registerResponse != null)
                {
                    output.AddRange(registerResponse.Data);
                    if (registerResponse.Meta != null)
                    {
                        if (registerResponse.Meta.Total > registerResponse.Meta.Limit * registerResponse.Meta.Page)
                        {
                            page++;
                            keepGoing = true;                            
                        }
                    }
                }
                else
                {
                    throw new StarTradersResponseJsonException(await response.Content.ReadAsStringAsync());
                }
            }
            else
            {
                throw new StarTradersApiFailException(await response.Content.ReadAsStringAsync());
            }
        } while (keepGoing);

        return output;
    }

    public async Task<T> GetFromStarTradersApi<T>(string requestUri)
    {
        await _throttleService.Throttle();
        
        using HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
        
        return await ReadApiResponse<T>(response);
    }

    public async Task<T> PostToStarTradersApiWithPayload<T, U>(string requestUri, U request)
    {
        await _throttleService.Throttle();
       
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(requestUri, request);
        
        return await ReadApiResponse<T>(response);
    }

    public async Task<T> PostToStarTradersApi<T>(string requestUri)
    {
        // Create empty content
        HttpContent content = new StringContent("");
        await _throttleService.Throttle();
        
        using HttpResponseMessage response = await _httpClient.PostAsync(requestUri, content);
        
        return await ReadApiResponse<T>(response);
    }

    private async Task<T> ReadApiResponse<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {            
            SuccessResponse<T>? registerResponse = await response.Content.ReadFromJsonAsync<SuccessResponse<T>>();
            if (registerResponse != null)
            {
                return registerResponse.Data;
            }
            else
            {
                throw new StarTradersResponseJsonException(await response.Content.ReadAsStringAsync());                             
            }
        }
        else
        {
            string responseString = await response.Content.ReadAsStringAsync();
            IErrorResponseData errorResponseData = _errorDecoder.Decode(responseString);
            if (errorResponseData != null)
            {
                throw new StarTradersErrorResponseException(errorResponseData);
            }
            else
            {
                throw new StarTradersApiFailException(responseString);
            }
        }
    }
}
