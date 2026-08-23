using Microsoft.EntityFrameworkCore;
using Miastro.Application.Natal;
using Miastro.Domain.Aspects;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.Domain.Objects;
using Miastro.Domain.Placements;
using Miastro.Domain.People;
using Miastro.Infrastructure.Persistence.Entities;

namespace Miastro.Infrastructure.Persistence.Natal;

public sealed class EfNatalChartStore(
    MiastroDbContext dbContext)
    : INatalChartStore
{
    public async Task<PersistNatalChartResult>
        SaveOrGetExistingAsync(
            NatalChartSnapshotWriteModel snapshot,
            string inputHash,
            CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentException.ThrowIfNullOrWhiteSpace(inputHash);

        NatalSnapshotValidator.Validate(snapshot);

        var normalizedHash =
            inputHash.Trim().ToLowerInvariant();

        var expectedInputHash =
            NatalInputHash.Compute(
                snapshot.Input);

        if (!string.Equals(
                normalizedHash,
                expectedInputHash,
                StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "InputHash no coincide con el fingerprint natal.",
                nameof(inputHash));
        }

        var existing =
            await QueryFull()
                .SingleOrDefaultAsync(
                    x =>
                        x.PersonId == snapshot.PersonId
                        && x.InputHash == normalizedHash,
                    cancellationToken);

        if (existing is not null)
        {
            if (existing.Status !=
                (int)NatalChartStatus.Current)
            {
                await using var transaction =
                    await dbContext.Database
                        .BeginTransactionAsync(
                            cancellationToken);

                var current =
                    await dbContext.NatalCharts
                        .Where(x =>
                            x.PersonId == snapshot.PersonId
                            && x.Status ==
                                (int)NatalChartStatus.Current
                            && x.Id != existing.Id)
                        .ToListAsync(
                            cancellationToken);

                foreach (var chart in current)
                {
                    chart.Status =
                        (int)NatalChartStatus.Superseded;

                    chart.SupersededByChartId =
                        existing.Id;
                }

                existing.Status =
                    (int)NatalChartStatus.Current;

                existing.InvalidatedAtUtc = null;
                existing.SupersededByChartId = null;

                await dbContext.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);
            }

            return new(
                Map(existing),
                Created: false);
        }

        await using var createTransaction =
            await dbContext.Database.BeginTransactionAsync(
                cancellationToken);

        var entity =
            MapNew(
                snapshot,
                normalizedHash);

        var previousCurrent =
            await dbContext.NatalCharts
                .Where(x =>
                    x.PersonId == snapshot.PersonId
                    && x.Status ==
                        (int)NatalChartStatus.Current)
                .ToListAsync(
                    cancellationToken);

        foreach (var previous in previousCurrent)
        {
            previous.Status =
                (int)NatalChartStatus.Superseded;

            previous.SupersededByChartId =
                entity.Id;
        }

        dbContext.NatalCharts.Add(entity);

        var hasPreviousChart =
            await dbContext.NatalCharts
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.PersonId == snapshot.PersonId
                        && x.Id != entity.Id,
                    cancellationToken);

        dbContext.PersonHistory.Add(
            new PersonHistoryEntity
            {
                PersonId =
                    snapshot.PersonId,

                EventType =
                    (int)(
                        hasPreviousChart
                            ? PersonHistoryEventType
                                .NatalChartRecalculated
                            : PersonHistoryEventType
                                .NatalChartCalculated),

                OccurredAtUtc =
                    snapshot.CalculatedAtUtc
                        .ToUniversalTime(),

                Summary =
                    hasPreviousChart
                        ? "Carta natal recalculada"
                        : "Carta natal calculada"
            });

        await dbContext.SaveChangesAsync(
            cancellationToken);

        await createTransaction.CommitAsync(
            cancellationToken);

        return new(
            Map(entity),
            Created: true);
    }

    public async Task<NatalChartSnapshotReadModel?>
        GetCurrentAsync(
            Guid personId,
            CancellationToken cancellationToken = default)
    {
        var candidates =
            await QueryFull()
                .AsNoTracking()
                .Where(x =>
                    x.PersonId == personId
                    && x.Status ==
                        (int)NatalChartStatus.Current)
                .ToListAsync(
                    cancellationToken);

        var entity =
            candidates
                .OrderByDescending(x =>
                    x.CalculatedAtUtc)
                .FirstOrDefault();

        return entity is null
            ? null
            : Map(entity);
    }

    public async Task<NatalChartSnapshotReadModel?>
        GetByInputHashAsync(
            Guid personId,
            string inputHash,
            CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(
            inputHash);

        var normalizedHash =
            inputHash.Trim().ToLowerInvariant();

        var entity =
            await QueryFull()
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    x =>
                        x.PersonId == personId
                        && x.InputHash == normalizedHash,
                    cancellationToken);

        return entity is null
            ? null
            : Map(entity);
    }

    public async Task InvalidateCurrentAsync(
        Guid personId,
        DateTimeOffset invalidatedAtUtc,
        CancellationToken cancellationToken = default)
    {
        var current =
            await dbContext.NatalCharts
                .Where(x =>
                    x.PersonId == personId
                    && x.Status ==
                        (int)NatalChartStatus.Current)
                .ToListAsync(
                    cancellationToken);

        foreach (var chart in current)
        {
            chart.Status =
                (int)NatalChartStatus.Invalidated;

            chart.InvalidatedAtUtc =
                invalidatedAtUtc.ToUniversalTime();

            chart.SupersededByChartId = null;
        }

        if (current.Count > 0)
        {
            await dbContext.SaveChangesAsync(
                cancellationToken);
        }
    }

    private IQueryable<NatalChartEntity> QueryFull()
        => dbContext.NatalCharts
            .Include(x => x.Placements)
            .Include(x => x.HouseCusps)
            .Include(x => x.Aspects);

    private static NatalChartEntity MapNew(
        NatalChartSnapshotWriteModel snapshot,
        string inputHash)
    {
        var id = Guid.NewGuid();

        return new NatalChartEntity
        {
            Id = id,
            PersonId = snapshot.PersonId,

            Status =
                (int)NatalChartStatus.Current,

            InputHash = inputHash,

            BirthDataVersion =
                snapshot.BirthDataVersion,

            BirthDataHash =
                ResolveBirthDataHash(
                    snapshot),

            BirthTimePrecision =
                (int)snapshot.Input.TimePrecision,

            GeoNameId =
                snapshot.Input.GeoNameId,

            HistoricalOffsetSeconds =
                snapshot.Input.HistoricalOffsetSeconds,

            AmbiguousSelection =
                string.IsNullOrWhiteSpace(
                    snapshot.Input.AmbiguousSelection)
                    ? null
                    : snapshot.Input
                        .AmbiguousSelection
                        .Trim(),

            IsApproximateBirthTime =
                snapshot.IsApproximateBirthTime,

            BirthLocalDate =
                snapshot.Input.LocalDate,

            BirthLocalTime =
                snapshot.Input.LocalTime,

            InstantUtc =
                snapshot.Input.InstantUtc
                    .ToUniversalTime(),

            Locality =
                snapshot.Locality.Trim(),

            Latitude =
                snapshot.Input.Latitude,

            Longitude =
                snapshot.Input.Longitude,

            IanaTimeZoneId =
                snapshot.Input.IanaTimeZoneId.Trim(),

            TzdbVersion =
                snapshot.Input.TzdbVersion.Trim(),

            HouseSystem =
                (int)snapshot.Input.HouseSystem,

            CalculationProfileId =
                snapshot.Input.CalculationProfileId.Trim(),

            MiastroVersion =
                snapshot.MiastroVersion.Trim(),

            Engine =
                snapshot.Input.Engine.Trim(),

            EngineVersion =
                snapshot.Input.EngineVersion.Trim(),

            AdapterVersion =
                snapshot.AdapterVersion.Trim(),

            EphemerisVersion =
                snapshot.Input.EphemerisVersion.Trim(),

            CalculatedAtUtc =
                snapshot.CalculatedAtUtc
                    .ToUniversalTime(),

            Placements =
                snapshot.Placements
                    .Select(x =>
                        new NatalPlacementEntity
                        {
                            ChartId = id,
                            ObjectId =
                                (int)x.ObjectId,
                            LongitudeDegrees =
                                x.LongitudeDegrees,
                            LatitudeDegrees =
                                x.LatitudeDegrees,
                            DistanceAu =
                                x.DistanceAu,
                            LongitudeSpeedDegreesPerDay =
                                x.LongitudeSpeedDegreesPerDay,
                            LatitudeSpeedDegreesPerDay =
                                x.LatitudeSpeedDegreesPerDay,
                            DistanceSpeedAuPerDay =
                                x.DistanceSpeedAuPerDay,
                            Motion =
                                x.Motion is null
                                    ? null
                                    : (int)x.Motion.Value,
                            ZodiacSign =
                                x.ZodiacSign,
                            DegreeInSign =
                                x.DegreeInSign,
                            HouseNumber =
                                x.HouseNumber
                        })
                    .ToList(),

            HouseCusps =
                snapshot.HouseCusps
                    .Select(x =>
                        new NatalHouseCuspEntity
                        {
                            ChartId = id,
                            HouseNumber =
                                x.HouseNumber,
                            LongitudeDegrees =
                                x.LongitudeDegrees
                        })
                    .ToList(),

            Aspects =
                snapshot.Aspects
                    .Select(x =>
                        new NatalAspectEntity
                        {
                            ChartId = id,
                            FirstObject =
                                (int)x.FirstObject,
                            SecondObject =
                                (int)x.SecondObject,
                            Kind =
                                (int)x.Kind,
                            SeparationDegrees =
                                x.SeparationDegrees,
                            ExactAngleDegrees =
                                x.ExactAngleDegrees,
                            DeviationDegrees =
                                x.DeviationDegrees,
                            AllowedOrbDegrees =
                                x.AllowedOrbDegrees,
                            UsedOrbDegrees =
                                x.UsedOrbDegrees
                        })
                    .ToList()
        };
    }

    private static string ResolveBirthDataHash(
        NatalChartSnapshotWriteModel snapshot)
    {
        if (snapshot.BirthDataVersion !=
            NatalBirthDataIdentity.CurrentVersion)
        {
            throw new ArgumentOutOfRangeException(
                nameof(snapshot),
                "Versión BirthData no soportada.");
        }

        var expected =
            NatalBirthDataIdentity.Compute(
                snapshot.Input);

        if (string.IsNullOrWhiteSpace(
                snapshot.BirthDataHash))
        {
            return expected;
        }

        var supplied =
            snapshot.BirthDataHash
                .Trim()
                .ToLowerInvariant();

        if (!string.Equals(
                supplied,
                expected,
                StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "BirthDataHash no coincide con el fingerprint.",
                nameof(snapshot));
        }

        return supplied;
    }

    private static NatalChartSnapshotReadModel Map(
        NatalChartEntity entity)
        => new(
            entity.Id,
            entity.PersonId,
            (NatalChartStatus)entity.Status,
            entity.InputHash,
            entity.IsApproximateBirthTime,
            entity.BirthLocalDate,
            entity.BirthLocalTime,
            entity.InstantUtc,
            entity.Locality,
            entity.Latitude,
            entity.Longitude,
            entity.IanaTimeZoneId,
            entity.TzdbVersion,
            (HouseSystem)entity.HouseSystem,
            entity.CalculationProfileId,
            entity.MiastroVersion,
            entity.Engine,
            entity.EngineVersion,
            entity.AdapterVersion,
            entity.EphemerisVersion,
            entity.CalculatedAtUtc,
            entity.InvalidatedAtUtc,
            entity.SupersededByChartId,
            entity.Placements
                .OrderBy(x => x.ObjectId)
                .Select(x =>
                    new NatalPlacementSnapshot(
                        (AstrologicalObjectId)x.ObjectId,
                        x.LongitudeDegrees,
                        x.LatitudeDegrees,
                        x.DistanceAu,
                        x.LongitudeSpeedDegreesPerDay,
                        x.LatitudeSpeedDegreesPerDay,
                        x.DistanceSpeedAuPerDay,
                        x.Motion is null
                            ? null
                            : (MotionState)x.Motion.Value,
                        x.ZodiacSign,
                        x.DegreeInSign,
                        x.HouseNumber))
                .ToArray(),
            entity.HouseCusps
                .OrderBy(x => x.HouseNumber)
                .Select(x =>
                    new NatalHouseCuspSnapshot(
                        x.HouseNumber,
                        x.LongitudeDegrees))
                .ToArray(),
            entity.Aspects
                .OrderBy(x => x.FirstObject)
                .ThenBy(x => x.SecondObject)
                .Select(x =>
                    new NatalAspectSnapshot(
                        (AstrologicalObjectId)x.FirstObject,
                        (AstrologicalObjectId)x.SecondObject,
                        (AspectKind)x.Kind,
                        x.SeparationDegrees,
                        x.ExactAngleDegrees,
                        x.DeviationDegrees,
                        x.AllowedOrbDegrees,
                        x.UsedOrbDegrees))
                .ToArray(),
            entity.BirthDataVersion,
            entity.BirthDataHash,
            entity.BirthTimePrecision,
            entity.GeoNameId,
            entity.HistoricalOffsetSeconds,
            entity.AmbiguousSelection);
}
