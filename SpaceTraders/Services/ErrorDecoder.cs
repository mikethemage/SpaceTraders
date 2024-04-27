using SpaceTraders.Api.Responses;
using SpaceTraders.Api.Responses.ResponseData;
using SpaceTraders.Api.Responses.ResponseData.Errors;
using System.Text.Json;

namespace SpaceTraders.Services;
internal class ErrorDecoder : IErrorDecoder
{
    const int _marketTradeNotSoldError = 4602;
    const int _authenticationError = 401;

    public IErrorResponseData Decode(string jsonResponse)
    {
        try
        {
            JsonElement? errorResponseDynamic = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
            if (errorResponseDynamic?.GetProperty("error").GetProperty("code").GetInt32() is int statusCode)
            {
                var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                IErrorResponseData? errorResponseData = null;

                switch (statusCode)
                {
                    case _marketTradeNotSoldError:
                        {
                            ErrorResponse<ErrorResponseData<MarketTradeNotSoldError>>? errorResponse = JsonSerializer.Deserialize<ErrorResponse<ErrorResponseData<MarketTradeNotSoldError>>>(jsonResponse, jsonOptions);
                            errorResponseData = errorResponse?.Error;
                            break;
                        }


                    case _authenticationError:
                        {
                            ErrorResponse<ErrorResponseData<MatchingError>>? errorResponse = JsonSerializer.Deserialize<ErrorResponse<ErrorResponseData<MatchingError>>>(jsonResponse, jsonOptions);
                            errorResponseData = errorResponse?.Error;
                            break;
                        }
                }

                if (errorResponseData != null)
                {
                    return errorResponseData;
                }
                else
                {
                    return new UnknownErrorResponseData
                    {
                        Message = errorResponseDynamic?.GetProperty("error").GetProperty("message").GetString() ?? string.Empty,
                        Code = statusCode,
                        ErrorText = errorResponseDynamic?.GetProperty("error").GetProperty("data").ToString() ?? string.Empty
                    };
                }
            }
            throw new Exception("Unable to decode dynamic JSON");
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to deserialize error response", ex);
        }
    }
}
