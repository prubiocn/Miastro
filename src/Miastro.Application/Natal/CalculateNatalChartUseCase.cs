using Miastro.Astronomy.Abstractions.Contracts;
using Miastro.Astronomy.Abstractions.Errors;
using Miastro.Astronomy.Abstractions.Models;
using Miastro.Application.People;
using Miastro.Domain.Aspects;
using Miastro.Domain.Calculation;
using Miastro.Domain.Charts;
using Miastro.Domain.DerivedPoints;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.People;
using Miastro.Domain.Placements;

namespace Miastro.Application.Natal;

public sealed class CalculateNatalChartUseCase(
    IPersonStore personStore,
    INatalChartStore natalChartStore,
    IEclipticPositionCalculator positionCalculator,
    IHouseCalculator houseCalculator,
    INatalCalculationMetadataProvider metadataProvider)
{
    private static readonly AstrologicalObjectId[] SwissObjects =
    [
        AstrologicalObjectId.Sun,
        AstrologicalObjectId.Moon,
        AstrologicalObjectId.Mercury,
        AstrologicalObjectId.Venus,
        AstrologicalObjectId.Mars,
        AstrologicalObjectId.Jupiter,
        AstrologicalObjectId.Saturn,
        AstrologicalObjectId.Uranus,
        AstrologicalObjectId.Neptune,
        AstrologicalObjectId.Pluto,
        AstrologicalObjectId.NorthTrueNode,
        AstrologicalObjectId.MeanLilith,
        AstrologicalObjectId.Chiron,
        AstrologicalObjectId.Ceres,
        AstrologicalObjectId.Pallas,
        AstrologicalObjectId.Juno,
        AstrologicalObjectId.Vesta
    ];

    public async Task<NatalCalculationResult> ExecuteAsync(
        Guid personId,
        HouseSystem houseSystem = HouseSystem.Placidus,
        DateTimeOffset? calculatedAtUtc = null,
        CancellationToken cancellationToken = default)
    {
        if (personId == Guid.Empty)
        {
            return Failure(
                NatalCalculationResultCode.PersonNotFound,
                "La Persona indicada no existe.");
        }

        var person =
            await personStore.GetAsync(
                personId,
                cancellationToken);

        if (person is null)
        {
            return Failure(
                NatalCalculationResultCode.PersonNotFound,
                "La Persona indicada no existe.");
        }

        var birth =
            person.BirthData;

        if (birth is null)
        {
            return Failure(
                NatalCalculationResultCode.BirthDataMissing,
                "La Persona no tiene datos de nacimiento.");
        }

        var eligibility =
            NatalCalculationEligibilityPolicy.Evaluate(
                birth);

        if (!eligibility.CanCalculate)
        {
            return EligibilityFailure(
                eligibility.Status);
        }

        if (birth.LocalTime is null
            || birth.ResolvedInstantUtc is null
            || string.IsNullOrWhiteSpace(
                birth.TzdbVersion))
        {
            return Failure(
                NatalCalculationResultCode.HistoricalTimeUnresolved,
                "La hora histórica de nacimiento no está resuelta.");
        }

        GeographicLocation location;

        try
        {
            location =
                new GeographicLocation(
                    birth.Latitude,
                    birth.Longitude);
        }
        catch (ArgumentOutOfRangeException)
        {
            return Failure(
                NatalCalculationResultCode.InvalidCoordinates,
                "Las coordenadas de nacimiento no son válidas.");
        }

        NatalCalculationEnvironment environment;

        try
        {
            environment =
                metadataProvider.Get();

            ValidateEnvironment(environment);
        }
        catch
        {
            return Failure(
                NatalCalculationResultCode.AstronomyCalculationFailed,
                "No están disponibles los metadatos del motor astronómico.");
        }

        var instant =
            AstronomicalInstant.FromUtc(
                birth.ResolvedInstantUtc.Value);

        var fingerprint =
            new NatalInputFingerprint(
                birth.LocalDate,
                birth.LocalTime.Value,
                birth.ResolvedInstantUtc.Value,
                birth.Latitude,
                birth.Longitude,
                birth.IanaTimeZoneId,
                birth.TzdbVersion,
                houseSystem,
                CalculationProfile.MiastroV1.Id,
                environment.Engine,
                environment.EngineVersion,
                environment.EphemerisVersion,
                birth.TimePrecision,
                birth.GeoNameId,
                birth.Locality,
                birth.HistoricalOffsetSeconds,
                birth.AmbiguousSelectedCandidate?
                    .ToString(),
                RangeStart:
                    birth.RangeStart,
                RangeEnd:
                    birth.RangeEnd,
                DayPeriod:
                    birth.DayPeriod,
                Country:
                    birth.Country,
                Region:
                    birth.Region,
                Subregion:
                    birth.Subregion,
                ResolutionState:
                    birth.ResolutionState,
                AmbiguousEarlierOffsetSeconds:
                    birth.AmbiguousEarlierOffsetSeconds,
                AmbiguousEarlierInstantUtc:
                    birth.AmbiguousEarlierInstantUtc,
                AmbiguousLaterOffsetSeconds:
                    birth.AmbiguousLaterOffsetSeconds,
                AmbiguousLaterInstantUtc:
                    birth.AmbiguousLaterInstantUtc,
                ManualCoordinateOverride:
                    birth.ManualCoordinateOverride);

        var inputHash =
            NatalInputHash.Compute(
                fingerprint);

        var sameInput =
            await natalChartStore.GetByInputHashAsync(
                personId,
                inputHash,
                cancellationToken);

        if (sameInput?.Status ==
            NatalChartStatus.Current)
        {
            return new(
                NatalCalculationResultCode.ExistingCurrentSnapshot,
                "La carta natal vigente ya corresponde a estas entradas.",
                sameInput);
        }

        HouseCalculationResult houses;

        try
        {
            houses =
                houseCalculator.Calculate(
                    instant,
                    location,
                    houseSystem);
        }
        catch (AstronomyEngineException)
        {
            return Failure(
                NatalCalculationResultCode.HouseCalculationUnavailable,
                "No se pudieron calcular las casas para esta carta.");
        }

        if (!houses.Success
            || houses.Cusps.Count != 12
            || houses.Ascendant is null
            || houses.Midheaven is null)
        {
            return Failure(
                NatalCalculationResultCode.HouseCalculationUnavailable,
                houses.Error?.SafeMessage
                ?? "No se pudieron calcular las casas para esta carta.");
        }

        var raw =
            new Dictionary<
                AstrologicalObjectId,
                EclipticPosition>();

        try
        {
            foreach (var objectId in SwissObjects)
            {
                raw[objectId] =
                    positionCalculator.Calculate(
                        objectId,
                        instant,
                        CalculationProfile.MiastroV1);
            }
        }
        catch (AstronomyEngineException ex)
        {
            return Failure(
                NatalCalculationResultCode.AstronomyCalculationFailed,
                ex.Error.SafeMessage);
        }
        catch
        {
            return Failure(
                NatalCalculationResultCode.AstronomyCalculationFailed,
                "No se pudo completar el cálculo astronómico.");
        }

        if (!TryValidateEngineMetadata(
            raw.Values,
            environment))
        {
            return Failure(
                NatalCalculationResultCode.AstronomyCalculationFailed,
                "Los metadatos del motor astronómico no son coherentes.");
        }

        var placements =
            new List<AstrologicalPlacement>();

        foreach (var objectId in SwissObjects)
        {
            var position =
                raw[objectId];

            placements.Add(
                new AstrologicalPlacement(
                    objectId,
                    position.Longitude,
                    NatalHousePlacementResolver.Resolve(
                        position.Longitude,
                        houses.Cusps),
                    position.LongitudeSpeedDegreesPerDay));
        }

        var north =
            raw[
                AstrologicalObjectId.NorthTrueNode];

        var southLongitude =
            LunarNodeCalculator.CalculateSouthNode(
                north.Longitude);

        placements.Add(
            new AstrologicalPlacement(
                AstrologicalObjectId.SouthNode,
                southLongitude,
                NatalHousePlacementResolver.Resolve(
                    southLongitude,
                    houses.Cusps),
                north.LongitudeSpeedDegreesPerDay));

        var ascendant =
            houses.Ascendant.Value;

        var midheaven =
            houses.Midheaven.Value;

        placements.Add(
            new AstrologicalPlacement(
                AstrologicalObjectId.Ascendant,
                ascendant,
                NatalHousePlacementResolver.Resolve(
                    ascendant,
                    houses.Cusps)));

        placements.Add(
            new AstrologicalPlacement(
                AstrologicalObjectId.Midheaven,
                midheaven,
                NatalHousePlacementResolver.Resolve(
                    midheaven,
                    houses.Cusps)));

        var sun =
            raw[AstrologicalObjectId.Sun];

        var moon =
            raw[AstrologicalObjectId.Moon];

        var sect =
            NatalChartSectResolver.Resolve(
                sun.Longitude,
                houses.Cusps);

        var fortuneLongitude =
            PartOfFortuneCalculator.Calculate(
                ascendant,
                sun.Longitude,
                moon.Longitude,
                sect);

        placements.Add(
            new AstrologicalPlacement(
                AstrologicalObjectId.PartOfFortune,
                fortuneLongitude,
                NatalHousePlacementResolver.Resolve(
                    fortuneLongitude,
                    houses.Cusps)));

        var orderedPlacements =
            placements
                .OrderBy(x =>
                    NatalObjectOrder.GetIndex(
                        x.ObjectId))
                .ToArray();

        if (orderedPlacements.Length != 21)
        {
            return Failure(
                NatalCalculationResultCode.AstronomyCalculationFailed,
                "El cálculo natal devolvió un conjunto incompleto de objetos.");
        }

        var aspects =
            NatalAspectCalculator.Calculate(
                orderedPlacements);

        var chartId =
            Guid.NewGuid();

        var chart =
            new AstrologicalChart(
                chartId,
                ChartType.Natal,
                orderedPlacements,
                CalculationProfile.MiastroV1,
                MiastroV1AspectProfile.Instance,
                new CalculationMetadata(
                    miastroVersion:
                        environment.MiastroVersion,
                    calculationProfileId:
                        CalculationProfile.MiastroV1.Id,
                    engine:
                        environment.Engine,
                    engineVersion:
                        environment.EngineVersion,
                    ephemerisVersion:
                        environment.EphemerisVersion,
                    tzdbVersion:
                        birth.TzdbVersion,
                    houseSystem:
                        houseSystem),
                houses.Cusps,
                houseSystem);

        var snapshot =
            CreateSnapshot(
                personId,
                birth,
                fingerprint,
                environment,
                eligibility.IsApproximate,
                calculatedAtUtc
                    ?? DateTimeOffset.UtcNow,
                orderedPlacements,
                raw,
                houses,
                aspects,
                fortuneLongitude);

        try
        {
            var persisted =
                await natalChartStore
                    .SaveOrGetExistingAsync(
                        snapshot,
                        inputHash,
                        cancellationToken);

            return new(
                persisted.Created
                    ? NatalCalculationResultCode.Calculated
                    : NatalCalculationResultCode.ExistingCurrentSnapshot,
                persisted.Created
                    ? "Carta natal calculada."
                    : "La carta natal ya estaba calculada.",
                persisted.Chart,
                chart,
                aspects,
                sect);
        }
        catch
        {
            return Failure(
                NatalCalculationResultCode.PersistenceFailed,
                "La carta se calculó pero no pudo guardarse.");
        }
    }

    private static NatalChartSnapshotWriteModel
        CreateSnapshot(
            Guid personId,
            BirthDataReadModel birth,
            NatalInputFingerprint fingerprint,
            NatalCalculationEnvironment environment,
            bool isApproximate,
            DateTimeOffset calculatedAtUtc,
            IReadOnlyList<AstrologicalPlacement> placements,
            IReadOnlyDictionary<
                AstrologicalObjectId,
                EclipticPosition> raw,
            HouseCalculationResult houses,
            IReadOnlyList<AspectResult> aspects,
            Miastro.Domain.Angles.EclipticLongitude fortuneLongitude)
    {
        var placementSnapshots =
            placements
                .Select(placement =>
                {
                    raw.TryGetValue(
                        placement.ObjectId,
                        out var position);

                    double? latitude = null;
                    double? distance = null;
                    double? longitudeSpeed =
                        placement.SpeedDegreesPerDay;
                    double? latitudeSpeed = null;
                    double? radialSpeed = null;

                    if (position is not null)
                    {
                        latitude =
                            position.LatitudeDegrees;

                        distance =
                            position.DistanceAu;

                        longitudeSpeed =
                            position.LongitudeSpeedDegreesPerDay;

                        latitudeSpeed =
                            position.LatitudeSpeedDegreesPerDay;

                        radialSpeed =
                            position.DistanceSpeedAuPerDay;
                    }
                    else if (
                        placement.ObjectId ==
                        AstrologicalObjectId.SouthNode)
                    {
                        var north =
                            raw[
                                AstrologicalObjectId
                                    .NorthTrueNode];

                        latitude =
                            -north.LatitudeDegrees;

                        distance =
                            north.DistanceAu;

                        longitudeSpeed =
                            north.LongitudeSpeedDegreesPerDay;

                        latitudeSpeed =
                            -north.LatitudeSpeedDegreesPerDay;

                        radialSpeed =
                            north.DistanceSpeedAuPerDay;
                    }

                    return new NatalPlacementSnapshot(
                        placement.ObjectId,
                        placement.Longitude.Degrees,
                        latitude,
                        distance,
                        longitudeSpeed,
                        latitudeSpeed,
                        radialSpeed,
                        placement.Motion,
                        (int)placement.Sign,
                        placement.DegreeInSign,
                        placement.House?.Number);
                })
                .ToArray();

        var cuspSnapshots =
            houses.Cusps
                .OrderBy(x =>
                    x.House.Number)
                .Select(x =>
                    new NatalHouseCuspSnapshot(
                        x.House.Number,
                        x.Longitude.Degrees))
                .ToArray();

        var aspectSnapshots =
            aspects
                .Select(x =>
                    new NatalAspectSnapshot(
                        x.FirstObject,
                        x.SecondObject,
                        x.Definition.Kind,
                        x.Separation.Degrees,
                        x.ExactAngleDegrees,
                        x.DeviationDegrees,
                        x.AllowedOrbDegrees,
                        x.UsedOrbDegrees))
                .ToArray();

        return new(
            personId,
            fingerprint,
            isApproximate,
            birth.Locality,
            environment.MiastroVersion,
            environment.AdapterVersion,
            calculatedAtUtc.ToUniversalTime(),
            placementSnapshots,
            cuspSnapshots,
            aspectSnapshots,
            NatalBirthDataIdentity.CurrentVersion,
            NatalBirthDataIdentity.Compute(
                fingerprint));
    }

    private static bool TryValidateEngineMetadata(
        IEnumerable<EclipticPosition> positions,
        NatalCalculationEnvironment environment)
    {
        foreach (var position in positions)
        {
            if (!string.Equals(
                    position.EngineMetadata.Engine,
                    environment.Engine,
                    StringComparison.Ordinal)
                || !string.Equals(
                    position.EngineMetadata.EngineVersion,
                    environment.EngineVersion,
                    StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    private static void ValidateEnvironment(
        NatalCalculationEnvironment environment)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            environment.MiastroVersion);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            environment.Engine);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            environment.EngineVersion);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            environment.AdapterVersion);

        ArgumentException.ThrowIfNullOrWhiteSpace(
            environment.EphemerisVersion);
    }

    private static NatalCalculationResult
        EligibilityFailure(
            NatalCalculationEligibilityStatus status)
        => status switch
        {
            NatalCalculationEligibilityStatus.BirthDataMissing =>
                Failure(
                    NatalCalculationResultCode.BirthDataMissing,
                    "La Persona no tiene datos de nacimiento."),

            NatalCalculationEligibilityStatus
                .BirthTimeRangeRequiresResolution
            or NatalCalculationEligibilityStatus
                .BirthTimeDayPeriodInsufficient
            or NatalCalculationEligibilityStatus
                .BirthTimeUnknown =>
                Failure(
                    NatalCalculationResultCode.BirthTimeInsufficient,
                    "La precisión horaria no permite una carta natal completa."),

            _ =>
                Failure(
                    NatalCalculationResultCode.HistoricalTimeUnresolved,
                    "La hora histórica de nacimiento debe resolverse antes de calcular.")
        };

    private static NatalCalculationResult Failure(
        NatalCalculationResultCode code,
        string message)
        => new(
            code,
            message);
}
