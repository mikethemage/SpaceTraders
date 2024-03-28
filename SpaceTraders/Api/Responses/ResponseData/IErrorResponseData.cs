namespace SpaceTraders.Api.Responses.ResponseData;

internal interface IErrorResponseData
{
    public string Message { get; set; }
    public int Code { get; set; }
    string ErrorText { get; }
}