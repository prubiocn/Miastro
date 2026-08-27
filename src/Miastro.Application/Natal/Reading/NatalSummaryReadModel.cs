using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;

namespace Miastro.Application.Natal.Reading;

public sealed record NatalSummaryAspectReadModel(
    AstrologicalObjectId FirstObjectId,
    AstrologicalObjectId SecondObjectId,
    AspectKind Kind,
    string Text,
    double DeviationDegrees);

public sealed record NatalSummaryReadModel(
    string SunText,
    string MoonText,
    string AscendantText,
    string MidheavenText,
    string ElementText,
    string ModalityText,
    string HouseConcentrationText,
    string RetrogradesText,
    IReadOnlyList<NatalSummaryAspectReadModel> MainAspects)
{
    public IReadOnlyList<string> Lines
    {
        get
        {
            var lines =
                new List<string>
                {
                    SunText,
                    MoonText,
                    AscendantText,
                    MidheavenText,
                    ElementText,
                    ModalityText,
                    HouseConcentrationText,
                    RetrogradesText
                };

            lines.AddRange(
                MainAspects.Select(
                    x => x.Text));

            return lines;
        }
    }
}
