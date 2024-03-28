using SpaceTraders.Api.Responses;
using SpaceTraders.Api.Responses.ResponseData;
using SpaceTraders.Exceptions;
using SpaceTraders.Repositories;
using System.Net.Http.Json;

namespace SpaceTraders.Services;
internal class SpaceTradersApiService : ISpaceTradersApiService
{
    private readonly HttpClient _httpClient;    
    private readonly ITokenRepository _tokenRepository;
    private readonly IErrorDecoder _errorDecoder;

    // Semaphore for throttling requests
    private readonly SemaphoreSlim _throttleSemaphore;
    private readonly Queue<DateTime> _requestQueue;
    private readonly object _lockObject = new object();

    public SpaceTradersApiService(HttpClient httpClient, ITokenRepository tokenRepository, IErrorDecoder errorDecoder)
    {
        _tokenRepository = tokenRepository;
        _httpClient = httpClient;

        _httpClient.BaseAddress = new Uri("https://api.spacetraders.io/v2/");
        _errorDecoder = errorDecoder;

        // Initialize the semaphore with a limit of 2 requests per second
        _throttleSemaphore = new SemaphoreSlim(2, 2);
        _requestQueue = new Queue<DateTime>();
    }

    private async Task ThrottleAndCheckBurstLimit()
    {
        const int burstTimeInSeconds = 60;
        const int burstMaxRequests = 30;

        await _throttleSemaphore.WaitAsync();

        lock (_lockObject)
        {
            while(_requestQueue.Count > 0 && _requestQueue.Peek() >  DateTime.UtcNow.AddSeconds(burstTimeInSeconds))
            {
                _requestQueue.Dequeue();
            }

            // Check if burst limit is reached
            if (_requestQueue.Count >= burstMaxRequests)
            {
                var oldestRequest = _requestQueue.Peek();
                var diff = DateTime.UtcNow - oldestRequest;
                if (diff < TimeSpan.FromSeconds(burstTimeInSeconds))
                {
                    var delayTime = TimeSpan.FromSeconds(burstTimeInSeconds) - diff;
                    Task.Delay(delayTime).Wait(); // Use Wait since Task.Delay doesn't support async lock
                }
            }

            // Delay to ensure 2 requests per second
            DateTime lastRequestTime = _requestQueue.LastOrDefault();

            TimeSpan timeSinceLastRequest = DateTime.UtcNow - lastRequestTime;
            if (timeSinceLastRequest < TimeSpan.FromSeconds(0.5))
            {
                var delayTime = TimeSpan.FromSeconds(0.5) - timeSinceLastRequest;
                Task.Delay(delayTime).Wait(); // Wait to maintain rate limit
            }            

            _requestQueue.Enqueue(DateTime.UtcNow);
        }
    }


    public void UpdateToken()
    {
        _httpClient.DefaultRequestHeaders.Clear();
        if(_tokenRepository.Token!=string.Empty)
        {
            // Set authorization header for HttpClient
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_tokenRepository.Token}");
        }
    }

    public async Task<List<T>> GetAllFromStarTradersApi<T>(string requestUri)
    {
        int page = 1;
        bool keepGoing;
        List<T> output = new List<T>();

        do
        {
            keepGoing = false;
            await ThrottleAndCheckBurstLimit();
            using HttpResponseMessage response = await _httpClient.GetAsync($"{requestUri}?page={page}");
            _throttleSemaphore.Release(); // Release semaphore token

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
        await ThrottleAndCheckBurstLimit();
        using HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
        _throttleSemaphore.Release(); // Release semaphore token
        return await ReadApiResponse<T>(response);
    }

    public async Task<T> PostToStarTradersApiWithPayload<T, U>(string requestUri, U request)
    {
        await ThrottleAndCheckBurstLimit();
        using HttpResponseMessage response = await _httpClient.PostAsJsonAsync(requestUri, request);
        _throttleSemaphore.Release(); // Release semaphore token
        return await ReadApiResponse<T>(response);
    }

    public async Task<T> PostToStarTradersApi<T>(string requestUri)
    {
        // Create empty content
        HttpContent content = new StringContent("");
        await ThrottleAndCheckBurstLimit();
        using HttpResponseMessage response = await _httpClient.PostAsync(requestUri, content);
        _throttleSemaphore.Release(); // Release semaphore token
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
