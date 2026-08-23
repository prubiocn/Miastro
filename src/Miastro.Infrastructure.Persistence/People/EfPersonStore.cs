using Microsoft.EntityFrameworkCore;
using Miastro.Application.People;
using Miastro.Domain.People;
using Miastro.Domain.Natal;
using Miastro.Infrastructure.Persistence.Entities;

namespace Miastro.Infrastructure.Persistence.People;

public sealed class EfPersonStore(
    MiastroDbContext dbContext)
    : IPersonStore
{
    public async Task<Guid> CreateAsync(
        CreatePersonCommand command,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var id = Guid.NewGuid();

        var entity = new PersonEntity
        {
            Id = id,
            FirstName = command.FirstName.Trim(),
            LastName = command.LastName.Trim(),
            NormalizedName = Normalize(
                command.FirstName,
                command.LastName),
            Phone = Clean(command.Phone),
            Email = Clean(command.Email),
            PrivateNote = Clean(command.PrivateNote),
            IsFavorite = command.IsFavorite,
            CreatedAtUtc = nowUtc,
            ModifiedAtUtc = nowUtc,
            BirthData = MapBirth(
                id,
                command.BirthData),
            CurrentResidence = MapResidence(
                id,
                command.CurrentResidence),
            History =
            [
                new PersonHistoryEntity
                {
                    PersonId = id,
                    EventType =
                        (int)PersonHistoryEventType.Created,
                    OccurredAtUtc = nowUtc,
                    Summary = "Persona creada"
                }
            ]
        };

        dbContext.People.Add(entity);

        await dbContext.SaveChangesAsync(
            cancellationToken);

        return id;
    }

    public async Task UpdateAsync(
        UpdatePersonCommand command,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.People
            .Include(x => x.BirthData)
            .Include(x => x.CurrentResidence)
            .Include(x => x.History)
            .SingleOrDefaultAsync(
                x => x.Id == command.Id,
                cancellationToken)
            ?? throw new KeyNotFoundException(
                "Person not found.");

        var natalRelevantBirthChange =
            BirthDataNatalChangeDetector
                .HasNatalRelevantChange(
                    entity.BirthData,
                    command.BirthData);

        entity.FirstName = command.FirstName.Trim();
        entity.LastName = command.LastName.Trim();
        entity.NormalizedName = Normalize(
            command.FirstName,
            command.LastName);
        entity.Phone = Clean(command.Phone);
        entity.Email = Clean(command.Email);
        entity.PrivateNote = Clean(command.PrivateNote);
        entity.IsFavorite = command.IsFavorite;
        entity.ModifiedAtUtc = nowUtc;

        if (entity.BirthData is not null)
        {
            dbContext.BirthData.Remove(
                entity.BirthData);
        }

        entity.BirthData = MapBirth(
            entity.Id,
            command.BirthData);

        if (entity.CurrentResidence is not null)
        {
            dbContext.CurrentResidences.Remove(
                entity.CurrentResidence);
        }

        entity.CurrentResidence = MapResidence(
            entity.Id,
            command.CurrentResidence);

        entity.History.Add(
            new PersonHistoryEntity
            {
                PersonId = entity.Id,
                EventType =
                    (int)PersonHistoryEventType.RelevantEdit,
                OccurredAtUtc = nowUtc,
                Summary = "Ficha actualizada"
            });

        if (natalRelevantBirthChange)
        {
            var currentCharts =
                await dbContext.NatalCharts
                    .Where(x =>
                        x.PersonId == entity.Id
                        && x.Status ==
                            (int)NatalChartStatus.Current)
                    .ToListAsync(
                        cancellationToken);

            foreach (var chart in currentCharts)
            {
                chart.Status =
                    (int)NatalChartStatus.Invalidated;

                chart.InvalidatedAtUtc =
                    nowUtc.ToUniversalTime();

                chart.SupersededByChartId =
                    null;
            }

            if (currentCharts.Count > 0)
            {
                entity.History.Add(
                    new PersonHistoryEntity
                    {
                        PersonId = entity.Id,
                        EventType =
                            (int)PersonHistoryEventType
                                .NatalChartInvalidated,
                        OccurredAtUtc =
                            nowUtc.ToUniversalTime(),
                        Summary =
                            "Carta natal invalidada por cambio en datos de nacimiento"
                    });
            }
        }

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task<PersonDetails?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.People
            .AsNoTracking()
            .Include(x => x.BirthData)
            .Include(x => x.CurrentResidence)
            .Include(x => x.History)
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);

        return entity is null
            ? null
            : MapDetails(entity);
    }

    public async Task<IReadOnlyList<PersonListItem>> SearchAsync(
        PersonSearchQuery query,
        CancellationToken cancellationToken = default)
    {
        IQueryable<PersonEntity> people =
            dbContext.People.AsNoTracking();

        var text = Clean(query.Text);

        if (text is not null)
        {
            var normalized =
                text.ToLowerInvariant();

            people = people.Where(
                x => x.NormalizedName.Contains(
                    normalized));
        }

        people = query.Filter switch
        {
            PersonFilter.Favorites =>
                people.Where(x => x.IsFavorite),

            PersonFilter.Recent =>
                people.Where(
                    x => x.LastConsultationAtUtc != null),

            _ => people
        };

        if (query.Sort == PersonSort.LastConsultation)
        {
            // EF Core SQLite no traduce ORDER BY para DateTimeOffset.
            // Se proyecta solo el modelo ligero de lista y se ordena
            // en memoria, manteniendo el límite público de 500.
            var recentRows = await people
                .Select(
                    x => new PersonListItem(
                        x.Id,
                        x.FirstName,
                        x.LastName,
                        x.IsFavorite,
                        x.LastConsultationAtUtc))
                .ToListAsync(cancellationToken);

            return recentRows
                .OrderByDescending(
                    x => x.LastConsultationAtUtc)
                .ThenBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ThenBy(x => x.Id)
                .Take(query.Limit)
                .ToArray();
        }

        people = query.Sort switch
        {
            PersonSort.LastName =>
                people
                    .OrderBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .ThenBy(x => x.Id),

            PersonSort.Favorite =>
                people
                    .OrderByDescending(x => x.IsFavorite)
                    .ThenBy(x => x.LastName)
                    .ThenBy(x => x.FirstName)
                    .ThenBy(x => x.Id),

            _ =>
                people
                    .OrderBy(x => x.FirstName)
                    .ThenBy(x => x.LastName)
                    .ThenBy(x => x.Id)
        };

        return await people
            .Take(query.Limit)
            .Select(
                x => new PersonListItem(
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.IsFavorite,
                    x.LastConsultationAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task SetFavoriteAsync(
        Guid id,
        bool isFavorite,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var entity = await FindRequiredAsync(
            id,
            cancellationToken);

        if (entity.IsFavorite == isFavorite)
        {
            return;
        }

        entity.IsFavorite = isFavorite;
        entity.ModifiedAtUtc = nowUtc;

        entity.History.Add(
            new PersonHistoryEntity
            {
                PersonId = id,
                EventType =
                    (int)PersonHistoryEventType.RelevantEdit,
                OccurredAtUtc = nowUtc,
                Summary = "Favorito actualizado"
            });

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task RecordConsultationAsync(
        Guid id,
        DateTimeOffset nowUtc,
        CancellationToken cancellationToken = default)
    {
        var entity = await FindRequiredAsync(
            id,
            cancellationToken);

        entity.LastConsultationAtUtc = nowUtc;

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.People
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken)
            ?? throw new KeyNotFoundException(
                "Person not found.");

        dbContext.People.Remove(entity);

        await dbContext.SaveChangesAsync(
            cancellationToken);
    }

    private Task<PersonEntity> FindRequiredAsync(
        Guid id,
        CancellationToken cancellationToken)
        => dbContext.People
            .Include(x => x.History)
            .SingleOrDefaultAsync(
                x => x.Id == id,
                cancellationToken)
            .ContinueWith(
                task => task.Result
                    ?? throw new KeyNotFoundException(
                        "Person not found."),
                cancellationToken,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);

    private static BirthDataEntity? MapBirth(
        Guid personId,
        BirthDataWriteModel? source)
        => source is null
            ? null
            : new BirthDataEntity
            {
                PersonId = personId,
                LocalDate = source.LocalDate,
                TimePrecision = (int)source.TimePrecision,
                LocalTime = source.LocalTime,
                RangeStart = source.RangeStart,
                RangeEnd = source.RangeEnd,
                DayPeriod = source.DayPeriod is null
                    ? null
                    : (int)source.DayPeriod.Value,
                GeoNameId = source.GeoNameId,
                Locality = source.Locality.Trim(),
                Country = source.Country.Trim(),
                Region = source.Region.Trim(),
                Subregion = Clean(source.Subregion),
                Latitude = source.Latitude,
                Longitude = source.Longitude,
                IanaTimeZoneId =
                    source.IanaTimeZoneId.Trim(),
                TzdbVersion = Clean(source.TzdbVersion),
                TemporalResolutionState =
                    (int)source.ResolutionState,
                HistoricalOffsetSeconds =
                    source.HistoricalOffsetSeconds,
                ResolvedInstantUtc =
                    source.ResolvedInstantUtc,
                AmbiguousEarlierOffsetSeconds =
                    source.AmbiguousEarlierOffsetSeconds,
                AmbiguousEarlierInstantUtc =
                    source.AmbiguousEarlierInstantUtc,
                AmbiguousLaterOffsetSeconds =
                    source.AmbiguousLaterOffsetSeconds,
                AmbiguousLaterInstantUtc =
                    source.AmbiguousLaterInstantUtc,
                AmbiguousSelectedCandidate =
                    source.AmbiguousSelectedCandidate,
                AmbiguousSelectionRecordedAtUtc =
                    source.AmbiguousSelectionRecordedAtUtc,
                ManualCoordinateOverride =
                    source.ManualCoordinateOverride,
                OriginalGeoNamesLatitude =
                    source.OriginalGeoNamesLatitude,
                OriginalGeoNamesLongitude =
                    source.OriginalGeoNamesLongitude
            };

    private static CurrentResidenceEntity? MapResidence(
        Guid personId,
        CurrentResidenceWriteModel? source)
        => source is null
            ? null
            : new CurrentResidenceEntity
            {
                PersonId = personId,
                Locality = source.Locality.Trim(),
                GeoNameId = source.GeoNameId,
                Region = source.Region.Trim(),
                Country = source.Country.Trim(),
                Latitude = source.Latitude,
                Longitude = source.Longitude,
                IanaTimeZoneId =
                    source.IanaTimeZoneId.Trim(),
                UpdatedAtUtc = source.UpdatedAtUtc
            };

    private static PersonDetails MapDetails(
        PersonEntity entity)
        => new(
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.Phone,
            entity.Email,
            entity.PrivateNote,
            entity.IsFavorite,
            entity.CreatedAtUtc,
            entity.ModifiedAtUtc,
            entity.LastConsultationAtUtc,
            entity.BirthData is null
                ? null
                : new BirthDataReadModel(
                    entity.BirthData.LocalDate,
                    (BirthTimePrecision)
                        entity.BirthData.TimePrecision,
                    entity.BirthData.LocalTime,
                    entity.BirthData.RangeStart,
                    entity.BirthData.RangeEnd,
                    entity.BirthData.DayPeriod is null
                        ? null
                        : (DayPeriod)
                            entity.BirthData.DayPeriod.Value,
                    entity.BirthData.GeoNameId,
                    entity.BirthData.Locality,
                    entity.BirthData.Country,
                    entity.BirthData.Region,
                    entity.BirthData.Subregion,
                    entity.BirthData.Latitude,
                    entity.BirthData.Longitude,
                    entity.BirthData.IanaTimeZoneId,
                    entity.BirthData.TzdbVersion,
                    (BirthTemporalResolutionState)
                        entity.BirthData.TemporalResolutionState,
                    entity.BirthData.HistoricalOffsetSeconds,
                    entity.BirthData.ResolvedInstantUtc,
                    entity.BirthData.AmbiguousEarlierOffsetSeconds,
                    entity.BirthData.AmbiguousEarlierInstantUtc,
                    entity.BirthData.AmbiguousLaterOffsetSeconds,
                    entity.BirthData.AmbiguousLaterInstantUtc,
                    entity.BirthData.AmbiguousSelectedCandidate,
                    entity.BirthData.AmbiguousSelectionRecordedAtUtc,
                    entity.BirthData.ManualCoordinateOverride,
                    entity.BirthData.OriginalGeoNamesLatitude,
                    entity.BirthData.OriginalGeoNamesLongitude),
            entity.CurrentResidence is null
                ? null
                : new CurrentResidenceReadModel(
                    entity.CurrentResidence.Locality,
                    entity.CurrentResidence.GeoNameId,
                    entity.CurrentResidence.Region,
                    entity.CurrentResidence.Country,
                    entity.CurrentResidence.Latitude,
                    entity.CurrentResidence.Longitude,
                    entity.CurrentResidence.IanaTimeZoneId,
                    entity.CurrentResidence.UpdatedAtUtc),
            entity.History
                .OrderBy(x => x.OccurredAtUtc)
                .ThenBy(x => x.Id)
                .Select(
                    x => new PersonHistoryReadModel(
                        (PersonHistoryEventType)
                            x.EventType,
                        x.OccurredAtUtc,
                        x.Summary))
                .ToArray());

    private static string Normalize(
        string firstName,
        string lastName)
        => $"{firstName.Trim()} {lastName.Trim()}"
            .ToLowerInvariant();

    private static string? Clean(string? value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
}
