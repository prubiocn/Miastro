using System.Globalization;
using Miastro.Application.Natal;
using Miastro.Domain.Aspects;
using Miastro.Domain.Objects;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed record NatalAspectRowViewModel(
    AstrologicalObjectId FirstObjectId,
    AstrologicalObjectId SecondObjectId,
    string FirstObjectName,
    string AspectName,
    string SecondObjectName,
    string OrbText)
{
    public static NatalAspectRowViewModel From(
        NatalAspectSnapshot aspect)
        => new(
            aspect.FirstObject,
            aspect.SecondObject,
            NatalPlacementRowViewModel.ObjectLabel(
                aspect.FirstObject),
            AspectLabel(
                aspect.Kind),
            NatalPlacementRowViewModel.ObjectLabel(
                aspect.SecondObject),
            string.Format(
                CultureInfo.GetCultureInfo("es-ES"),
                "Orbe {0:0.00}°",
                aspect.UsedOrbDegrees));

    private static string AspectLabel(
        AspectKind kind)
        => kind switch
        {
            AspectKind.Conjunction =>
                "Conjunción",

            AspectKind.Semisextile =>
                "Semisextil",

            AspectKind.Sextile =>
                "Sextil",

            AspectKind.Square =>
                "Cuadratura",

            AspectKind.Trine =>
                "Trígono",

            AspectKind.Quincunx =>
                "Quincuncio",

            AspectKind.Opposition =>
                "Oposición",

            AspectKind.Quintile =>
                "Quintil",

            AspectKind.Biquintile =>
                "Biquintil",

            _ =>
                kind.ToString()
        };
}
