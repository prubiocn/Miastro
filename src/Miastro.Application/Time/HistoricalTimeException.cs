namespace Miastro.Application.Time;

public sealed class HistoricalTimeException : Exception
{
    public HistoricalTimeErrorCode Code { get; }

    public HistoricalTimeException(
        HistoricalTimeErrorCode code,
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Code = code;
    }
}
