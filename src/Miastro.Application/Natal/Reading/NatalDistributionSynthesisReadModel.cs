namespace Miastro.Application.Natal.Reading;

public sealed record NatalDistributionSynthesisReadModel(
    string ProfileId,
    IReadOnlyList<string> Lines)
{
    public string Text =>
        string.Join(
            Environment.NewLine,
            Lines);
}
