using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Miastro.Application.People;
using Miastro.Application.Time;
using Miastro.Domain.People;
using Miastro.UI.Avalonia.Commands;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed partial class MainWindowViewModel
{
    private bool _hasBirthData;
    private DateTimeOffset? _birthDate;
    private BirthPrecisionChoice _selectedBirthPrecision = null!;
    private string _birthTimeText = string.Empty;
    private string _birthRangeStartText = string.Empty;
    private string _birthRangeEndText = string.Empty;
    private DayPeriodChoice _selectedDayPeriod = null!;

    private string _birthLocationSearchText = string.Empty;
    private LocationResultViewModel? _selectedBirthSearchResult;
    private long? _birthGeoNameId;
    private string _birthLocality = string.Empty;
    private string _birthRegion = string.Empty;
    private string _birthSubregion = string.Empty;
    private string _birthCountry = string.Empty;
    private double? _birthLatitude;
    private double? _birthLongitude;
    private string _birthIanaTimeZoneId = string.Empty;

    private BirthDataWriteModel? _resolvedBirthSnapshot;
    private HistoricalTimeResolution? _pendingAmbiguousResolution;

    private string _birthResolutionMessage =
        "Pendiente de datos de nacimiento.";

    private bool _birthAmbiguous;
    private bool _birthSkipped;

    private bool _hasResidence;
    private string _residenceLocationSearchText = string.Empty;
    private LocationResultViewModel? _selectedResidenceSearchResult;
    private long? _residenceGeoNameId;
    private string _residenceLocality = string.Empty;
    private string _residenceRegion = string.Empty;
    private string _residenceCountry = string.Empty;
    private double? _residenceLatitude;
    private double? _residenceLongitude;
    private string _residenceIanaTimeZoneId = string.Empty;

    public void InitializePeopleDetailsEditor()
    {
        BirthPrecisionChoices =
        [
            new("Exacta", BirthTimePrecision.Exact),
            new("Aproximada", BirthTimePrecision.Approximate),
            new("Rango", BirthTimePrecision.Range),
            new("Momento del día", BirthTimePrecision.DayPeriod),
            new("Desconocida", BirthTimePrecision.Unknown)
        ];

        DayPeriodChoices =
        [
            new("Madrugada", DayPeriod.EarlyMorning),
            new("Mañana", DayPeriod.Morning),
            new("Tarde", DayPeriod.Afternoon),
            new("Noche", DayPeriod.Night)
        ];

        _selectedBirthPrecision = BirthPrecisionChoices[0];
        _selectedDayPeriod = DayPeriodChoices[1];

        SearchBirthLocationCommand =
            new AsyncCommand(SearchBirthLocationAsync);

        SelectBirthLocationCommand =
            new AsyncCommand(
                SelectBirthLocationAsync,
                () => SelectedBirthSearchResult is not null);

        SearchResidenceLocationCommand =
            new AsyncCommand(SearchResidenceLocationAsync);

        SelectResidenceLocationCommand =
            new AsyncCommand(
                SelectResidenceLocationAsync,
                () => SelectedResidenceSearchResult is not null);

        ResolveBirthTimeCommand =
            new AsyncCommand(
                ResolveBirthTimeAsync,
                () => CanResolveBirthTime);

        ChooseEarlierAmbiguousTimeCommand =
            new DelegateCommand(
                () => SelectAmbiguousCandidate(1),
                () => BirthAmbiguous);

        ChooseLaterAmbiguousTimeCommand =
            new DelegateCommand(
                () => SelectAmbiguousCandidate(2),
                () => BirthAmbiguous);
    }

    public IReadOnlyList<BirthPrecisionChoice>
        BirthPrecisionChoices { get; private set; }
        = Array.Empty<BirthPrecisionChoice>();

    public IReadOnlyList<DayPeriodChoice>
        DayPeriodChoices { get; private set; }
        = Array.Empty<DayPeriodChoice>();

    public ObservableCollection<LocationResultViewModel>
        BirthLocationResults { get; } = [];

    public ObservableCollection<LocationResultViewModel>
        ResidenceLocationResults { get; } = [];

    public AsyncCommand SearchBirthLocationCommand
        { get; private set; } = null!;

    public AsyncCommand SelectBirthLocationCommand
        { get; private set; } = null!;

    public AsyncCommand SearchResidenceLocationCommand
        { get; private set; } = null!;

    public AsyncCommand SelectResidenceLocationCommand
        { get; private set; } = null!;

    public AsyncCommand ResolveBirthTimeCommand
        { get; private set; } = null!;

    public DelegateCommand ChooseEarlierAmbiguousTimeCommand
        { get; private set; } = null!;

    public DelegateCommand ChooseLaterAmbiguousTimeCommand
        { get; private set; } = null!;

    public bool HasBirthData
    {
        get => _hasBirthData;
        set
        {
            if (SetField(ref _hasBirthData, value))
            {
                MarkDirty();
                InvalidateBirthResolution();
                RaiseBirthVisibility();
            }
        }
    }

    public DateTimeOffset? BirthDate
    {
        get => _birthDate;
        set
        {
            if (SetField(ref _birthDate, value))
            {
                MarkDirty();
                InvalidateBirthResolution();
            }
        }
    }

    public BirthPrecisionChoice SelectedBirthPrecision
    {
        get => _selectedBirthPrecision;
        set
        {
            if (SetField(ref _selectedBirthPrecision, value))
            {
                MarkDirty();
                InvalidateBirthResolution();
                RaiseBirthVisibility();
            }
        }
    }

    public string BirthTimeText
    {
        get => _birthTimeText;
        set
        {
            if (SetField(ref _birthTimeText, value))
            {
                MarkDirty();
                InvalidateBirthResolution();
            }
        }
    }

    public string BirthRangeStartText
    {
        get => _birthRangeStartText;
        set
        {
            if (SetField(ref _birthRangeStartText, value))
            {
                MarkDirty();
                InvalidateBirthResolution();
            }
        }
    }

    public string BirthRangeEndText
    {
        get => _birthRangeEndText;
        set
        {
            if (SetField(ref _birthRangeEndText, value))
            {
                MarkDirty();
                InvalidateBirthResolution();
            }
        }
    }

    public DayPeriodChoice SelectedDayPeriod
    {
        get => _selectedDayPeriod;
        set
        {
            if (SetField(ref _selectedDayPeriod, value))
            {
                MarkDirty();
                InvalidateBirthResolution();
            }
        }
    }

    public bool IsConcreteBirthTime
        => HasBirthData
           && SelectedBirthPrecision.Value
               is BirthTimePrecision.Exact
               or BirthTimePrecision.Approximate;

    public bool IsBirthRange
        => HasBirthData
           && SelectedBirthPrecision.Value
               == BirthTimePrecision.Range;

    public bool IsBirthDayPeriod
        => HasBirthData
           && SelectedBirthPrecision.Value
               == BirthTimePrecision.DayPeriod;

    public string BirthLocationSearchText
    {
        get => _birthLocationSearchText;
        set => SetField(ref _birthLocationSearchText, value);
    }

    public LocationResultViewModel? SelectedBirthSearchResult
    {
        get => _selectedBirthSearchResult;
        set
        {
            if (SetField(ref _selectedBirthSearchResult, value))
            {
                SelectBirthLocationCommand?
                    .RaiseCanExecuteChanged();
            }
        }
    }

    public string BirthLocationDisplay
        => _birthGeoNameId is null
            ? "Ninguna localidad seleccionada"
            : FormatLocation(
                _birthLocality,
                _birthRegion,
                _birthSubregion,
                _birthCountry);

    public string BirthResolutionMessage
    {
        get => _birthResolutionMessage;
        private set =>
            SetField(ref _birthResolutionMessage, value);
    }

    public bool BirthAmbiguous
    {
        get => _birthAmbiguous;
        private set
        {
            if (SetField(ref _birthAmbiguous, value))
            {
                ChooseEarlierAmbiguousTimeCommand?
                    .RaiseCanExecuteChanged();

                ChooseLaterAmbiguousTimeCommand?
                    .RaiseCanExecuteChanged();
            }
        }
    }

    public bool BirthSkipped
    {
        get => _birthSkipped;
        private set =>
            SetField(ref _birthSkipped, value);
    }

    public bool CanResolveBirthTime
        => HasBirthData
           && IsConcreteBirthTime
           && BirthDate is not null
           && _birthGeoNameId is not null
           && !string.IsNullOrWhiteSpace(BirthTimeText);

    public bool HasResidence
    {
        get => _hasResidence;
        set
        {
            if (SetField(ref _hasResidence, value))
            {
                MarkDirty();

                OnPropertyChanged(
                    nameof(ResidenceLocationDisplay));
            }
        }
    }

    public string ResidenceLocationSearchText
    {
        get => _residenceLocationSearchText;
        set =>
            SetField(ref _residenceLocationSearchText, value);
    }

    public LocationResultViewModel? SelectedResidenceSearchResult
    {
        get => _selectedResidenceSearchResult;
        set
        {
            if (SetField(
                ref _selectedResidenceSearchResult,
                value))
            {
                SelectResidenceLocationCommand?
                    .RaiseCanExecuteChanged();
            }
        }
    }

    public string ResidenceLocationDisplay
        => _residenceGeoNameId is null
            ? "Ninguna residencia seleccionada"
            : FormatLocation(
                _residenceLocality,
                _residenceRegion,
                null,
                _residenceCountry);

    public void LoadPeopleDetails(
        BirthDataReadModel? birth,
        CurrentResidenceReadModel? residence)
    {
        if (birth is null)
        {
            ResetBirthEditor();
        }
        else
        {
            _hasBirthData = true;

            _birthDate =
                new DateTimeOffset(
                    birth.LocalDate.Year,
                    birth.LocalDate.Month,
                    birth.LocalDate.Day,
                    0,
                    0,
                    0,
                    TimeSpan.Zero);

            _selectedBirthPrecision =
                BirthPrecisionChoices.Single(
                    x => x.Value == birth.TimePrecision);

            _birthTimeText =
                birth.LocalTime?.ToString("HH:mm")
                ?? string.Empty;

            _birthRangeStartText =
                birth.RangeStart?.ToString("HH:mm")
                ?? string.Empty;

            _birthRangeEndText =
                birth.RangeEnd?.ToString("HH:mm")
                ?? string.Empty;

            if (birth.DayPeriod is not null)
            {
                _selectedDayPeriod =
                    DayPeriodChoices.Single(
                        x => x.Value == birth.DayPeriod.Value);
            }

            _birthGeoNameId = birth.GeoNameId;
            _birthLocality = birth.Locality;
            _birthRegion = birth.Region;
            _birthSubregion = birth.Subregion ?? string.Empty;
            _birthCountry = birth.Country;
            _birthLatitude = birth.Latitude;
            _birthLongitude = birth.Longitude;
            _birthIanaTimeZoneId = birth.IanaTimeZoneId;

            _resolvedBirthSnapshot =
                ToWriteModel(birth);

            BirthAmbiguous =
                birth.ResolutionState
                    == BirthTemporalResolutionState.Ambiguous
                && birth.AmbiguousSelectedCandidate is null;

            BirthSkipped =
                birth.ResolutionState
                    == BirthTemporalResolutionState.Skipped;

            BirthResolutionMessage =
                HumanBirthResolution(
                    birth.ResolutionState,
                    birth.AmbiguousSelectedCandidate);
        }

        if (residence is null)
        {
            ResetResidenceEditor();
        }
        else
        {
            _hasResidence = true;
            _residenceGeoNameId = residence.GeoNameId;
            _residenceLocality = residence.Locality;
            _residenceRegion = residence.Region;
            _residenceCountry = residence.Country;
            _residenceLatitude = residence.Latitude;
            _residenceLongitude = residence.Longitude;
            _residenceIanaTimeZoneId =
                residence.IanaTimeZoneId;
        }

        NotifyPeopleDetailsChanged();
    }

    public BirthDataWriteModel? BuildBirthWriteModel()
    {
        if (!HasBirthData)
        {
            return null;
        }

        if (BirthDate is null)
        {
            throw new ArgumentException(
                "La fecha de nacimiento es obligatoria.");
        }

        if (_birthGeoNameId is null
            || _birthLatitude is null
            || _birthLongitude is null
            || string.IsNullOrWhiteSpace(
                _birthIanaTimeZoneId))
        {
            throw new ArgumentException(
                "Selecciona una localidad de nacimiento.");
        }

        var localDate =
            DateOnly.FromDateTime(
                BirthDate.Value.Date);

        var precision =
            SelectedBirthPrecision.Value;

        if (precision
            is BirthTimePrecision.Exact
            or BirthTimePrecision.Approximate)
        {
            var localTime =
                ParseTime(
                    BirthTimeText,
                    "hora de nacimiento");

            if (_resolvedBirthSnapshot is null)
            {
                throw new ArgumentException(
                    "Resuelve la hora histórica antes de guardar.");
            }

            if (BirthSkipped)
            {
                throw new ArgumentException(
                    "La hora local indicada no existió. Corrige la hora antes de guardar.");
            }

            return _resolvedBirthSnapshot with
            {
                LocalDate = localDate,
                TimePrecision = precision,
                LocalTime = localTime,
                GeoNameId = _birthGeoNameId.Value,
                Locality = _birthLocality,
                Country = _birthCountry,
                Region = _birthRegion,
                Subregion = EmptyToNull(_birthSubregion),
                Latitude = _birthLatitude.Value,
                Longitude = _birthLongitude.Value,
                IanaTimeZoneId = _birthIanaTimeZoneId
            };
        }

        if (precision == BirthTimePrecision.Range)
        {
            return BaseBirthWriteModel(
                localDate,
                precision) with
            {
                RangeStart =
                    ParseTime(
                        BirthRangeStartText,
                        "inicio del rango"),

                RangeEnd =
                    ParseTime(
                        BirthRangeEndText,
                        "fin del rango")
            };
        }

        if (precision
            == BirthTimePrecision.DayPeriod)
        {
            return BaseBirthWriteModel(
                localDate,
                precision) with
            {
                DayPeriod = SelectedDayPeriod.Value
            };
        }

        return BaseBirthWriteModel(
            localDate,
            BirthTimePrecision.Unknown);
    }

    public CurrentResidenceWriteModel?
        BuildResidenceWriteModel()
    {
        if (!HasResidence)
        {
            return null;
        }

        if (_residenceGeoNameId is null
            || _residenceLatitude is null
            || _residenceLongitude is null
            || string.IsNullOrWhiteSpace(
                _residenceIanaTimeZoneId))
        {
            throw new ArgumentException(
                "Selecciona una localidad de residencia.");
        }

        return new CurrentResidenceWriteModel(
            _residenceLocality,
            _residenceGeoNameId,
            _residenceRegion,
            _residenceCountry,
            _residenceLatitude.Value,
            _residenceLongitude.Value,
            _residenceIanaTimeZoneId,
            DateTimeOffset.UtcNow);
    }

    public void ResetPeopleDetailsForNewPerson()
    {
        ResetBirthEditor();
        ResetResidenceEditor();
        NotifyPeopleDetailsChanged();
    }

    private async Task SearchBirthLocationAsync()
    {
        ClearError();

        using var scope =
            _scopeFactory.CreateScope();

        var useCase =
            scope.ServiceProvider
                .GetRequiredService<
                    ResolveBirthLocationUseCase>();

        var results =
            await useCase.ExecuteAsync(
                BirthLocationSearchText,
                25);

        BirthLocationResults.Clear();

        foreach (var location in results)
        {
            BirthLocationResults.Add(
                LocationResultViewModel.From(location));
        }

        StatusMessage =
            BirthLocationResults.Count == 0
                ? "No se encontraron localidades."
                : "Selecciona una localidad de la lista.";
    }

    private Task SelectBirthLocationAsync()
    {
        if (SelectedBirthSearchResult is null)
        {
            return Task.CompletedTask;
        }

        var value = SelectedBirthSearchResult;

        _birthGeoNameId = value.GeoNameId;
        _birthLocality = value.Name;
        _birthRegion = value.Region;
        _birthSubregion = value.Subregion ?? string.Empty;
        _birthCountry = value.Country;
        _birthLatitude = value.Latitude;
        _birthLongitude = value.Longitude;
        _birthIanaTimeZoneId = value.IanaTimeZoneId;

        BirthLocationResults.Clear();
        SelectedBirthSearchResult = null;

        OnPropertyChanged(
            nameof(BirthLocationDisplay));

        MarkDirty();
        InvalidateBirthResolution();

        StatusMessage =
            "Localidad de nacimiento seleccionada.";

        return Task.CompletedTask;
    }

    private async Task SearchResidenceLocationAsync()
    {
        ClearError();

        using var scope =
            _scopeFactory.CreateScope();

        var useCase =
            scope.ServiceProvider
                .GetRequiredService<
                    ResolveCurrentResidenceLocationUseCase>();

        var results =
            await useCase.ExecuteAsync(
                ResidenceLocationSearchText,
                25);

        ResidenceLocationResults.Clear();

        foreach (var location in results)
        {
            ResidenceLocationResults.Add(
                LocationResultViewModel.From(location));
        }

        StatusMessage =
            ResidenceLocationResults.Count == 0
                ? "No se encontraron localidades."
                : "Selecciona una residencia de la lista.";
    }

    private Task SelectResidenceLocationAsync()
    {
        if (SelectedResidenceSearchResult is null)
        {
            return Task.CompletedTask;
        }

        var value =
            SelectedResidenceSearchResult;

        _residenceGeoNameId = value.GeoNameId;
        _residenceLocality = value.Name;
        _residenceRegion = value.Region;
        _residenceCountry = value.Country;
        _residenceLatitude = value.Latitude;
        _residenceLongitude = value.Longitude;
        _residenceIanaTimeZoneId = value.IanaTimeZoneId;

        ResidenceLocationResults.Clear();
        SelectedResidenceSearchResult = null;

        OnPropertyChanged(
            nameof(ResidenceLocationDisplay));

        MarkDirty();

        StatusMessage =
            "Residencia seleccionada.";

        return Task.CompletedTask;
    }

    private Task ResolveBirthTimeAsync()
    {
        ClearError();

        try
        {
            if (BirthDate is null)
            {
                throw new ArgumentException(
                    "Indica la fecha de nacimiento.");
            }

            if (_birthGeoNameId is null)
            {
                throw new ArgumentException(
                    "Selecciona la localidad de nacimiento.");
            }

            var localTime =
                ParseTime(
                    BirthTimeText,
                    "hora de nacimiento");

            using var scope =
                _scopeFactory.CreateScope();

            var useCase =
                scope.ServiceProvider
                    .GetRequiredService<
                        ResolveBirthHistoricalTimeUseCase>();

            var resolution =
                useCase.Execute(
                    DateOnly.FromDateTime(
                        BirthDate.Value.Date),
                    localTime,
                    _birthIanaTimeZoneId);

            var baseModel =
                BaseBirthWriteModel(
                    DateOnly.FromDateTime(
                        BirthDate.Value.Date),
                    SelectedBirthPrecision.Value) with
                {
                    LocalTime = localTime
                };

            if (resolution.Resolution.Status
                == HistoricalTimeResolutionStatus.Resolved)
            {
                _resolvedBirthSnapshot =
                    BirthHistoricalTimeSnapshotMapper.Apply(
                        baseModel,
                        resolution.Resolution);

                _pendingAmbiguousResolution = null;
                BirthAmbiguous = false;
                BirthSkipped = false;

                BirthResolutionMessage =
                    "Hora histórica resuelta correctamente.";
            }
            else if (
                resolution.Resolution.Status
                == HistoricalTimeResolutionStatus.Ambiguous)
            {
                _resolvedBirthSnapshot = null;

                _pendingAmbiguousResolution =
                    resolution.Resolution;

                BirthAmbiguous = true;
                BirthSkipped = false;

                BirthResolutionMessage =
                    "Hora ambigua: elige una de las dos posibilidades.";
            }
            else
            {
                _resolvedBirthSnapshot =
                    BirthHistoricalTimeSnapshotMapper.Apply(
                        baseModel,
                        resolution.Resolution);

                _pendingAmbiguousResolution = null;
                BirthAmbiguous = false;
                BirthSkipped = true;

                BirthResolutionMessage =
                    "Esa hora local no existió por un cambio horario. Corrige la hora.";
            }

            MarkDirty();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }

        return Task.CompletedTask;
    }

    private void SelectAmbiguousCandidate(
        int candidate)
    {
        if (_pendingAmbiguousResolution is null)
        {
            return;
        }

        try
        {
            if (BirthDate is null)
            {
                throw new ArgumentException(
                    "Falta la fecha de nacimiento.");
            }

            var baseModel =
                BaseBirthWriteModel(
                    DateOnly.FromDateTime(
                        BirthDate.Value.Date),
                    SelectedBirthPrecision.Value) with
                {
                    LocalTime =
                        ParseTime(
                            BirthTimeText,
                            "hora de nacimiento")
                };

            _resolvedBirthSnapshot =
                BirthHistoricalTimeSnapshotMapper.Apply(
                    baseModel,
                    _pendingAmbiguousResolution,
                    candidate,
                    DateTimeOffset.UtcNow);

            BirthAmbiguous = false;
            BirthSkipped = false;

            BirthResolutionMessage =
                candidate == 1
                    ? "Primera posibilidad seleccionada."
                    : "Segunda posibilidad seleccionada.";

            MarkDirty();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private BirthDataWriteModel BaseBirthWriteModel(
        DateOnly date,
        BirthTimePrecision precision)
        => new(
            date,
            precision,
            null,
            null,
            null,
            null,
            _birthGeoNameId
                ?? throw new ArgumentException(
                    "Selecciona una localidad de nacimiento."),
            _birthLocality,
            _birthCountry,
            _birthRegion,
            EmptyToNull(_birthSubregion),
            _birthLatitude
                ?? throw new ArgumentException(
                    "Falta latitud de la localidad."),
            _birthLongitude
                ?? throw new ArgumentException(
                    "Falta longitud de la localidad."),
            _birthIanaTimeZoneId,
            null,
            precision
                is BirthTimePrecision.Exact
                or BirthTimePrecision.Approximate
                    ? BirthTemporalResolutionState.Pending
                    : BirthTemporalResolutionState.NotApplicable,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            false,
            null,
            null);

    private void InvalidateBirthResolution()
    {
        _resolvedBirthSnapshot = null;
        _pendingAmbiguousResolution = null;
        BirthAmbiguous = false;
        BirthSkipped = false;

        BirthResolutionMessage =
            IsConcreteBirthTime
                ? "Pendiente de resolver la hora histórica."
                : "No requiere resolución histórica.";

        ResolveBirthTimeCommand?
            .RaiseCanExecuteChanged();

        OnPropertyChanged(
            nameof(CanResolveBirthTime));
    }

    private void RaiseBirthVisibility()
    {
        OnPropertyChanged(nameof(IsConcreteBirthTime));
        OnPropertyChanged(nameof(IsBirthRange));
        OnPropertyChanged(nameof(IsBirthDayPeriod));
        OnPropertyChanged(nameof(CanResolveBirthTime));

        ResolveBirthTimeCommand?
            .RaiseCanExecuteChanged();
    }

    private void ResetBirthEditor()
    {
        _hasBirthData = false;
        _birthDate = null;

        _selectedBirthPrecision =
            BirthPrecisionChoices.Count > 0
                ? BirthPrecisionChoices[0]
                : null!;

        _birthTimeText = string.Empty;
        _birthRangeStartText = string.Empty;
        _birthRangeEndText = string.Empty;

        _selectedDayPeriod =
            DayPeriodChoices.Count > 0
                ? DayPeriodChoices[1]
                : null!;

        _birthLocationSearchText = string.Empty;
        _selectedBirthSearchResult = null;
        _birthGeoNameId = null;
        _birthLocality = string.Empty;
        _birthRegion = string.Empty;
        _birthSubregion = string.Empty;
        _birthCountry = string.Empty;
        _birthLatitude = null;
        _birthLongitude = null;
        _birthIanaTimeZoneId = string.Empty;

        BirthLocationResults.Clear();

        _resolvedBirthSnapshot = null;
        _pendingAmbiguousResolution = null;
        _birthAmbiguous = false;
        _birthSkipped = false;

        _birthResolutionMessage =
            "Pendiente de datos de nacimiento.";
    }

    private void ResetResidenceEditor()
    {
        _hasResidence = false;
        _residenceLocationSearchText = string.Empty;
        _selectedResidenceSearchResult = null;
        _residenceGeoNameId = null;
        _residenceLocality = string.Empty;
        _residenceRegion = string.Empty;
        _residenceCountry = string.Empty;
        _residenceLatitude = null;
        _residenceLongitude = null;
        _residenceIanaTimeZoneId = string.Empty;

        ResidenceLocationResults.Clear();
    }

    private void NotifyPeopleDetailsChanged()
    {
        foreach (var property in new[]
        {
            nameof(HasBirthData),
            nameof(BirthDate),
            nameof(SelectedBirthPrecision),
            nameof(BirthTimeText),
            nameof(BirthRangeStartText),
            nameof(BirthRangeEndText),
            nameof(SelectedDayPeriod),
            nameof(IsConcreteBirthTime),
            nameof(IsBirthRange),
            nameof(IsBirthDayPeriod),
            nameof(BirthLocationDisplay),
            nameof(BirthResolutionMessage),
            nameof(BirthAmbiguous),
            nameof(BirthSkipped),
            nameof(HasResidence),
            nameof(ResidenceLocationDisplay)
        })
        {
            OnPropertyChanged(property);
        }

        ResolveBirthTimeCommand?
            .RaiseCanExecuteChanged();
    }

    private static TimeOnly ParseTime(
        string value,
        string fieldName)
    {
        if (TimeOnly.TryParse(
            value,
            out var parsed))
        {
            return parsed;
        }

        throw new ArgumentException(
            $"Revisa {fieldName}. Usa HH:mm.");
    }

    private static string FormatLocation(
        string locality,
        string region,
        string? subregion,
        string country)
    {
        var parts =
            new[]
            {
                locality,
                subregion,
                region,
                country
            }
            .Where(
                x => !string.IsNullOrWhiteSpace(x));

        return string.Join(
            " — ",
            parts);
    }

    private static string HumanBirthResolution(
        BirthTemporalResolutionState state,
        int? selectedCandidate)
        => state switch
        {
            BirthTemporalResolutionState.Resolved =>
                "Hora histórica resuelta correctamente.",

            BirthTemporalResolutionState.Ambiguous
                when selectedCandidate is not null =>
                "Hora ambigua resuelta mediante elección explícita.",

            BirthTemporalResolutionState.Ambiguous =>
                "Hora ambigua: requiere elegir una posibilidad.",

            BirthTemporalResolutionState.Skipped =>
                "Esa hora local no existió por un cambio horario.",

            BirthTemporalResolutionState.Pending =>
                "Pendiente de resolver la hora histórica.",

            _ =>
                "No requiere resolución histórica."
        };
}
