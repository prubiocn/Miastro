namespace Miastro.Astronomy.Abstractions.Errors;

public sealed record AstronomyError
{
    public AstronomyErrorCode Code { get; }

    public string TechnicalCode { get; }

    public string SafeMessage { get; }

    public AstronomyError(
        AstronomyErrorCode code,
        string technicalCode,
        string safeMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(technicalCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(safeMessage);

        Code = code;
        TechnicalCode = technicalCode;
        SafeMessage = safeMessage;
    }
}
