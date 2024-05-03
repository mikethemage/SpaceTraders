namespace SpaceTraders.Api.Responses;
internal class SuccessResponse<T>
{
    public T Data { get; set; } = default!;
    public Meta? Meta { get; set; } = null;

}
