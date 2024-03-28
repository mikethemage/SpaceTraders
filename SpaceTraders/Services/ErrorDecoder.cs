using SpaceTraders.Api.Responses;
using SpaceTraders.Api.Responses.ResponseData;
using SpaceTraders.Api.Responses.ResponseData.Errors;
using System.Text.Json;

namespace SpaceTraders.Services;
internal class ErrorDecoder : IErrorDecoder
{
    const int _marketTradeNotSoldError = 4602;

    public IErrorResponseData Decode(string jsonResponse)
    {
        try
        {
            JsonElement? errorResponseDynamic = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
            if (errorResponseDynamic?.GetProperty("error").GetProperty("code").GetInt32() is int statusCode)
            {               
                if (statusCode == _marketTradeNotSoldError)
                {
                    ErrorResponse<ErrorResponseData<MarketTradeNotSoldError>>? errorResponse = JsonSerializer.Deserialize<ErrorResponse<ErrorResponseData<MarketTradeNotSoldError>>>(jsonResponse,new JsonSerializerOptions { PropertyNameCaseInsensitive=true });
                    if (errorResponse is not null)
                    {
                        return errorResponse.Error;
                    }
                }

                return new UnknownErrorResponseData
                {
                    Message = errorResponseDynamic?.GetProperty("error").GetProperty("message").GetString() ?? string.Empty,
                    Code = statusCode,
                    ErrorText = errorResponseDynamic?.GetProperty("error").GetProperty("data").ToString() ?? string.Empty
                };

            }
            throw new Exception("Unable to decode dynamic JSON");
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to deserialize error response", ex);
        }
    }
}
