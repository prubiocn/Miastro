namespace Miastro.Graphics.Styles;

public sealed class NatalSceneStyleCatalog
{
    private readonly IReadOnlyDictionary<
        string,
        SceneStyle> _styles;

    public NatalSceneStyleCatalog()
    {
        _styles =
            Build()
                .Select(x => x.Validate())
                .ToDictionary(
                    x => x.Key,
                    StringComparer.Ordinal);
    }

    public IReadOnlyCollection<string> Keys =>
        _styles.Keys
            .OrderBy(
                x => x,
                StringComparer.Ordinal)
            .ToArray();

    public SceneStyle GetRequired(
        string key)
        =>
            _styles.TryGetValue(
                key,
                out var style)
                ? style
                : throw new KeyNotFoundException(
                    $"Unknown scene style '{key}'.");

    public bool TryGet(
        string key,
        out SceneStyle style)
        =>
            _styles.TryGetValue(
                key,
                out style!);

    private static IEnumerable<SceneStyle>
        Build()
    {
        var charcoal =
            new SceneColor(
                52,
                55,
                58);

        var blueGrey =
            new SceneColor(
                92,
                108,
                120);

        var blueGreyStrong =
            new SceneColor(
                67,
                84,
                99);

        var warmGrey =
            new SceneColor(
                151,
                146,
                137);

        var warmGreySoft =
            new SceneColor(
                190,
                185,
                176);

        var sand =
            new SceneColor(
                177,
                145,
                92);

        var ivory =
            new SceneColor(
                250,
                248,
                243);

        yield return new SceneStyle(
            NatalSceneStyleKeys.Background,
            warmGreySoft,
            0.8,
            FillColor: ivory);

        yield return new SceneStyle(
            NatalSceneStyleKeys.ZodiacBoundary,
            blueGrey,
            1.2);

        yield return new SceneStyle(
            NatalSceneStyleKeys.ZodiacGlyph,
            blueGreyStrong,
            1.6);

        yield return new SceneStyle(
            NatalSceneStyleKeys.DegreeBoundary,
            warmGrey,
            1.0,
            Opacity: 0.90);

        yield return new SceneStyle(
            NatalSceneStyleKeys.DegreeMinor,
            warmGreySoft,
            0.65,
            Opacity: 0.75);

        yield return new SceneStyle(
            NatalSceneStyleKeys.DegreeFive,
            warmGrey,
            1.0,
            Opacity: 0.90);

        yield return new SceneStyle(
            NatalSceneStyleKeys.DegreeTen,
            blueGrey,
            1.35);

        yield return new SceneStyle(
            NatalSceneStyleKeys.HouseCusp,
            warmGrey,
            0.9,
            Opacity: 0.85);

        yield return new SceneStyle(
            NatalSceneStyleKeys.HouseNumber,
            charcoal,
            1.0);

        yield return new SceneStyle(
            NatalSceneStyleKeys.PlanetOrbit,
            warmGrey,
            1.45,
            Opacity: 0.82);

        yield return new SceneStyle(
            NatalSceneStyleKeys.SoulCore,
            blueGrey,
            1.15,
            FillColor: ivory);

        yield return new SceneStyle(
            NatalSceneStyleKeys.AngleMajor,
            blueGreyStrong,
            2.4);

        yield return new SceneStyle(
            NatalSceneStyleKeys.AngleMinor,
            warmGrey,
            1.25,
            SceneLinePattern.Dashed,
            0.90);

        yield return new SceneStyle(
            NatalSceneStyleKeys.AngleLabelMajor,
            blueGreyStrong,
            1.5);

        yield return new SceneStyle(
            NatalSceneStyleKeys.AngleLabelMinor,
            warmGrey,
            1.0);

        yield return new SceneStyle(
            NatalSceneStyleKeys.BodyGlyph,
            charcoal,
            1.8);

        yield return new SceneStyle(
            NatalSceneStyleKeys.PointGlyph,
            sand,
            1.7);

        yield return new SceneStyle(
            NatalSceneStyleKeys.RealPositionMark,
            blueGreyStrong,
            1.1);

        yield return new SceneStyle(
            NatalSceneStyleKeys.LeaderLine,
            warmGrey,
            0.85,
            SceneLinePattern.Dashed,
            0.85);

        yield return new SceneStyle(
            NatalSceneStyleKeys.AspectRing,
            warmGrey,
            1.0,
            Opacity: 0.80);

        yield return new SceneStyle(
            NatalSceneStyleKeys.AspectMajor,
            blueGrey,
            1.35,
            SceneLinePattern.Solid,
            0.72);

        yield return new SceneStyle(
            NatalSceneStyleKeys.AspectSecondary,
            warmGrey,
            0.85,
            SceneLinePattern.Dashed,
            0.48);

        yield return new SceneStyle(
            NatalSceneStyleKeys.LabelPrimary,
            charcoal,
            1.0);

        yield return new SceneStyle(
            NatalSceneStyleKeys.LabelSecondary,
            warmGrey,
            0.9);

        yield return new SceneStyle(
            NatalSceneStyleKeys.InteractionSelected,
            sand,
            2.6);
    }
}
