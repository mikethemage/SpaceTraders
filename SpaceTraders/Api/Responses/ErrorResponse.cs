using SpaceTraders.Api.Responses.ResponseData;

namespace SpaceTraders.Api.Responses;
internal class ErrorResponse<T> where T : IErrorResponseData
{
    public T Error { get; set; } = default!;
}