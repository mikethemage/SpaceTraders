using SpaceTraders.Api.Responses.ResponseData;

namespace SpaceTraders.Exceptions;
[Serializable]
internal class StarTradersErrorResponseException : Exception
{
    public IErrorResponseData ErrorResponseData { get; init; }

    public StarTradersErrorResponseException(IErrorResponseData errorResponseData) : base(errorResponseData.Message)
    {
        ErrorResponseData = errorResponseData;
    }

    public StarTradersErrorResponseException(IErrorResponseData errorResponseData, Exception? innerException) : base(errorResponseData.Message, innerException)
    {
        ErrorResponseData = errorResponseData;
    }
}