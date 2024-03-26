namespace SpaceTraders.ApiModels.Responses;

internal class ErrorResponseData<T> : IErrorResponseData
{
    public string Message { get; set; } = string.Empty;
    public int Code { get; set; }
    public T Data { get; set; } = default!;
}
