namespace Miastro.Application.Natal.Reading;

public static class NatalDistributionSynthesisBuilder
{
    public static NatalDistributionSynthesisReadModel Build(
        NatalChartSnapshotReadModel snapshot,
        NatalDistributionProfile? profile = null)
    {
        ArgumentNullException.ThrowIfNull(
            snapshot);

        profile ??=
            NatalDistributionProfile.MiastroV1;

        var zodiac =
            NatalDistributionService.Build(
                snapshot,
                profile);

        var houses =
            NatalHouseDistributionService.Build(
                snapshot,
                profile);

        return Build(
            zodiac,
            houses);
    }

    public static NatalDistributionSynthesisReadModel Build(
        NatalDistributionReadModel zodiac,
        NatalHouseDistributionReadModel houses)
    {
        ArgumentNullException.ThrowIfNull(
            zodiac);

        ArgumentNullException.ThrowIfNull(
            houses);

        if (!string.Equals(
            zodiac.ProfileId,
            houses.ProfileId,
            StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Las distribuciones zodiacal y de casas "
                + "deben usar el mismo perfil.");
        }

        var lines =
            new List<string>
            {
                DescribeSection(
                    "Elemento",
                    "Elementos",
                    zodiac.Elements),

                DescribeSection(
                    "Modalidad",
                    "Modalidades",
                    zodiac.Modalities),

                DescribeSection(
                    "Polaridad",
                    "Polaridades",
                    zodiac.Polarities),

                DescribeSection(
                    "Hemisferio Este/Oeste",
                    "Hemisferios Este/Oeste",
                    houses.EastWest),

                DescribeSection(
                    "Hemisferio Superior/Inferior",
                    "Hemisferios Superior/Inferior",
                    houses.UpperLower),

                DescribeSection(
                    "Cuadrante",
                    "Cuadrantes",
                    houses.Quadrants),

                DescribeSection(
                    "Naturaleza de casas",
                    "Naturaleza de casas",
                    houses.HouseModes)
            };

        return new NatalDistributionSynthesisReadModel(
            zodiac.ProfileId,
            lines);
    }

    private static string DescribeSection<TCategory>(
        string singularLabel,
        string pluralLabel,
        NatalDistributionSection<TCategory> section)
        where TCategory : struct, Enum
    {
        ArgumentNullException.ThrowIfNull(
            section);

        var total =
            section.Buckets.Sum(
                x => x.Count);

        if (total == 0)
        {
            return
                $"{pluralLabel}: sin datos.";
        }

        if (section.Predominant is { } predominant)
        {
            var bucket =
                section.Buckets.Single(
                    x =>
                        EqualityComparer<TCategory>
                            .Default
                            .Equals(
                                x.Category,
                                predominant));

            return
                $"{singularLabel} predominante: "
                + $"{bucket.Label} "
                + $"({bucket.Count}/{total}).";
        }

        if (section.IsBalanced)
        {
            return
                $"{pluralLabel}: distribución equilibrada.";
        }

        var maximum =
            section.Buckets.Max(
                x => x.Count);

        var leaders =
            section.Buckets
                .Where(
                    x =>
                        x.Count == maximum)
                .Select(
                    x => x.Label)
                .ToArray();

        return
            $"{pluralLabel}: sin predominio único"
            + (
                leaders.Length > 1
                    ? " ("
                      + string.Join(
                          " / ",
                          leaders)
                      + ")."
                    : "."
            );
    }
}
