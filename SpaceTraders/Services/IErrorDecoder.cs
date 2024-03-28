using SpaceTraders.Api.Responses.ResponseData;

namespace SpaceTraders.Services;
internal interface IErrorDecoder
{
    IErrorResponseData Decode(string jsonResponse);
}