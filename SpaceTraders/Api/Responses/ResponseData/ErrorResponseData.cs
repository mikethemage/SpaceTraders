using SpaceTraders.Api.Responses.ResponseData.Errors;

namespace SpaceTraders.Api.Responses.ResponseData;

internal class ErrorResponseData<T> : IErrorResponseData where T : IErrorData
{
    public string Message { get; set; } = string.Empty;
    public int Code { get; set; }
    public T Data { get; set; } = default!;

    public string ErrorText
    {
        get
        {
            return Data.GetErrorAsText();
        }
    }
}
