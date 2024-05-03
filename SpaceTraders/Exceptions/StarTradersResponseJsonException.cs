namespace SpaceTraders.Exceptions;
[Serializable]
internal class StarTradersResponseJsonException : Exception
{
    public StarTradersResponseJsonException()
    {
    }

    public StarTradersResponseJsonException(string? message) : base(message)
    {
    }

    public StarTradersResponseJsonException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

}