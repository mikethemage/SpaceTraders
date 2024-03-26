namespace SpaceTraders.ApiModels.Responses;

internal interface IErrorResponseData
{
    public string Message { get; set; }
    public int Code { get; set; }
}