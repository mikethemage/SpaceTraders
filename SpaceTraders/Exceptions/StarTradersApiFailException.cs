using System.Runtime.Serialization;

namespace SpaceTraders.Exceptions;
[Serializable]
internal class StarTradersApiFailException : Exception
{
    public StarTradersApiFailException()
    {
    }

    public StarTradersApiFailException(string? message) : base(message)
    {
    }

    public StarTradersApiFailException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}