namespace Miastro.Astronomy.Abstractions.Errors;

public sealed class AstronomyEngineException : Exception
{
    public AstronomyError Error { get; }

    public string? TechnicalDetail { get; }

    public AstronomyEngineException(
        AstronomyError error,
        string? technicalDetail = null,
        Exception? innerException = null)
        : base(error.SafeMessage, innerException)
    {
        Error = error;
        TechnicalDetail = technicalDetail;
    }
}
