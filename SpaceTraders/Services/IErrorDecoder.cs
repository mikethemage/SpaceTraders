using SpaceTraders.ApiModels.Responses;

namespace SpaceTraders.Services;
internal interface IErrorDecoder
{
    IErrorResponseData Decode(string jsonResponse);
}