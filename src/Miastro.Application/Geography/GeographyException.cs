namespace Miastro.Application.Geography;

public sealed class GeographyException : Exception
{
    public GeographyErrorCode Code { get; }

    public GeographyException(
        GeographyErrorCode code,
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Code = code;
    }
}
