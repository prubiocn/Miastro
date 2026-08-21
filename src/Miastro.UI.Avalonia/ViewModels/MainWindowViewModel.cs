using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Miastro.Application.Configuration;
using Miastro.Application.People;
using Miastro.UI.Avalonia.Commands;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed partial class MainWindowViewModel
    : INotifyPropertyChanged
{
    private readonly ApplicationSettings _settings;
    private readonly IServiceScopeFactory _scopeFactory;

    private string _searchText = string.Empty;
    private PersonListRowViewModel? _selectedPerson;
    private PersonFilterChoice _selectedFilter;
    private PersonSortChoice _selectedSort;

    private Guid? _editingId;
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;
    private string _phone = string.Empty;
    private string _email = string.Empty;
    private string _privateNote = string.Empty;
    private bool _isFavorite;
    private DateTimeOffset? _lastConsultationAtUtc;

    private bool _isEditorVisible;
    private bool _isDirty;
    private bool _deleteArmed;
    private bool _cancelArmed;
    private bool _isBusy;

    private string _statusMessage =
        "Preparado";

    private string _errorMessage =
        string.Empty;

    public MainWindowViewModel(
        ApplicationSettings settings,
        IServiceScopeFactory scopeFactory)
    {
        _settings = settings;
        _scopeFactory = scopeFactory;

        Filters =
        [
            new(
                "Todas",
                PersonFilter.All),
            new(
                "Recientes",
                PersonFilter.Recent),
            new(
                "Favoritas",
                PersonFilter.Favorites)
        ];

        SortChoices =
        [
            new(
                "Nombre",
                PersonSort.FirstName),
            new(
                "Apellidos",
                PersonSort.LastName),
            new(
                "Última consulta",
                PersonSort.LastConsultation),
            new(
                "Favoritas primero",
                PersonSort.Favorite)
        ];

        _selectedFilter =
            Filters[0];

        _selectedSort =
            SortChoices[0];

        RefreshCommand =
            new AsyncCommand(
                RefreshAsync);

        SearchCommand =
            new AsyncCommand(
                RefreshAsync);

        OpenSelectedCommand =
            new AsyncCommand(
                OpenSelectedAsync,
                () => SelectedPerson is not null);

        NewPersonCommand =
            new DelegateCommand(
                StartNewPerson);

        SaveCommand =
            new AsyncCommand(
                SaveAsync,
                () => IsEditorVisible
                      && !IsBusy);

        CancelCommand =
            new DelegateCommand(
                CancelEdit,
                () => IsEditorVisible
                      && !IsBusy);

        SetConsultationNowCommand =
            new AsyncCommand(
                RecordConsultationAsync,
                () => EditingId is not null
                      && !IsBusy);

        DeleteCommand =
            new AsyncCommand(
                DeleteAsync,
                () => EditingId is not null
                      && !IsBusy);

        InitializePeopleDetailsEditor();
    }

    public event PropertyChangedEventHandler?
        PropertyChanged;

    public string Title
        => "Miastro";

    public string Language
        => _settings.Language;

    public string Status
        => StatusMessage;

    public ObservableCollection<
        PersonListRowViewModel> People
        { get; } = [];

    public IReadOnlyList<PersonFilterChoice>
        Filters { get; }

    public IReadOnlyList<PersonSortChoice>
        SortChoices { get; }

    public ICommand RefreshCommand { get; }

    public ICommand SearchCommand { get; }

    public ICommand OpenSelectedCommand { get; }

    public ICommand NewPersonCommand { get; }

    public ICommand SaveCommand { get; }

    public ICommand CancelCommand { get; }

    public ICommand SetConsultationNowCommand { get; }

    public ICommand DeleteCommand { get; }

    public string SearchText
    {
        get => _searchText;
        set => SetField(
            ref _searchText,
            value);
    }

    public PersonListRowViewModel?
        SelectedPerson
    {
        get => _selectedPerson;

        set
        {
            if (SetField(
                ref _selectedPerson,
                value))
            {
                RaiseCommands();
            }
        }
    }

    public PersonFilterChoice SelectedFilter
    {
        get => _selectedFilter;

        set
        {
            if (SetField(
                ref _selectedFilter,
                value))
            {
                _ = RefreshSafeAsync();
            }
        }
    }

    public PersonSortChoice SelectedSort
    {
        get => _selectedSort;

        set
        {
            if (SetField(
                ref _selectedSort,
                value))
            {
                _ = RefreshSafeAsync();
            }
        }
    }

    public Guid? EditingId
    {
        get => _editingId;

        private set
        {
            if (SetField(
                ref _editingId,
                value))
            {
                OnPropertyChanged(
                    nameof(EditorTitle));

                OnPropertyChanged(
                    nameof(DeleteButtonText));

                RaiseCommands();
            }
        }
    }

    public string EditorTitle
        => EditingId is null
            ? "Nueva persona"
            : "Ficha de persona";

    public string FirstName
    {
        get => _firstName;

        set
        {
            if (SetField(
                ref _firstName,
                value))
            {
                MarkDirty();
            }
        }
    }

    public string LastName
    {
        get => _lastName;

        set
        {
            if (SetField(
                ref _lastName,
                value))
            {
                MarkDirty();
            }
        }
    }

    public string Phone
    {
        get => _phone;

        set
        {
            if (SetField(
                ref _phone,
                value))
            {
                MarkDirty();
            }
        }
    }

    public string Email
    {
        get => _email;

        set
        {
            if (SetField(
                ref _email,
                value))
            {
                MarkDirty();
            }
        }
    }

    public string PrivateNote
    {
        get => _privateNote;

        set
        {
            if (SetField(
                ref _privateNote,
                value))
            {
                MarkDirty();
            }
        }
    }

    public bool IsFavorite
    {
        get => _isFavorite;

        set
        {
            if (SetField(
                ref _isFavorite,
                value))
            {
                MarkDirty();
            }
        }
    }

    public DateTimeOffset?
        LastConsultationAtUtc
    {
        get => _lastConsultationAtUtc;

        private set
        {
            if (SetField(
                ref _lastConsultationAtUtc,
                value))
            {
                OnPropertyChanged(
                    nameof(
                        LastConsultationText));
            }
        }
    }

    public string LastConsultationText
        => LastConsultationAtUtc is null
            ? "Todavía no hay consultas registradas."
            : "Última consulta: "
              + LastConsultationAtUtc.Value
                  .ToLocalTime()
                  .ToString(
                      "dd/MM/yyyy HH:mm");

    public bool IsEditorVisible
    {
        get => _isEditorVisible;

        private set
        {
            if (SetField(
                ref _isEditorVisible,
                value))
            {
                RaiseCommands();
            }
        }
    }

    public bool IsDirty
    {
        get => _isDirty;

        private set
            => SetField(
                ref _isDirty,
                value);
    }

    public bool IsBusy
    {
        get => _isBusy;

        private set
        {
            if (SetField(
                ref _isBusy,
                value))
            {
                RaiseCommands();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;

        private set
        {
            if (SetField(
                ref _statusMessage,
                value))
            {
                OnPropertyChanged(
                    nameof(Status));
            }
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;

        private set
            => SetField(
                ref _errorMessage,
                value);
    }

    public bool HasError
        => !string.IsNullOrWhiteSpace(
            ErrorMessage);

    public string CancelButtonText
        => _cancelArmed
            ? "Confirmar descartar"
            : "Cancelar";

    public string DeleteButtonText
        => _deleteArmed
            ? "Confirmar eliminación"
            : "Eliminar";

    public async Task InitializeAsync()
        => await RefreshSafeAsync();

    private async Task RefreshSafeAsync()
    {
        try
        {
            await RefreshAsync();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private async Task RefreshAsync()
    {
        ClearError();

        using var scope =
            _scopeFactory.CreateScope();

        var useCase =
            scope.ServiceProvider
                .GetRequiredService<
                    SearchPeopleUseCase>();

        var results =
            await useCase.ExecuteAsync(
                new PersonSearchQuery(
                    SearchText,
                    SelectedFilter.Value,
                    SelectedSort.Value,
                    200));

        People.Clear();

        foreach (var person in results)
        {
            People.Add(
                new PersonListRowViewModel(
                    person.Id,
                    person.FirstName,
                    person.LastName,
                    person.IsFavorite,
                    person.LastConsultationAtUtc));
        }

        StatusMessage =
            People.Count == 1
                ? "1 persona"
                : $"{People.Count} personas";
    }

    private void StartNewPerson()
    {
        ClearError();
        ClearValidationErrors();

        EditingId = null;

        SetEditorValues(
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            false,
            null);

        ResetPeopleDetailsForNewPerson();
        ResetPersonHistory();

        _deleteArmed = false;
        _cancelArmed = false;

        OnPropertyChanged(
            nameof(DeleteButtonText));

        OnPropertyChanged(
            nameof(CancelButtonText));

        ResetPeopleDetailsForNewPerson();
        ResetPersonHistory();

        _cancelArmed = false;

        OnPropertyChanged(
            nameof(CancelButtonText));

        IsDirty = false;
        IsEditorVisible = true;

        StatusMessage =
            "Nueva persona";
    }

    private async Task OpenSelectedAsync()
    {
        if (SelectedPerson is null)
        {
            return;
        }

        ClearError();
        ClearValidationErrors();

        using var scope =
            _scopeFactory.CreateScope();

        var useCase =
            scope.ServiceProvider
                .GetRequiredService<
                    GetPersonUseCase>();

        var person =
            await useCase.ExecuteAsync(
                SelectedPerson.Id);

        if (person is null)
        {
            await RefreshAsync();
            return;
        }

        EditingId =
            person.Id;

        SetEditorValues(
            person.FirstName,
            person.LastName,
            person.Phone ?? string.Empty,
            person.Email ?? string.Empty,
            person.PrivateNote ?? string.Empty,
            person.IsFavorite,
            person.LastConsultationAtUtc);

        LoadPeopleDetails(
            person.BirthData,
            person.CurrentResidence);

        LoadPersonHistory(
            person.History);

        _deleteArmed = false;
        _cancelArmed = false;

        OnPropertyChanged(
            nameof(CancelButtonText));

        OnPropertyChanged(
            nameof(DeleteButtonText));

        IsDirty = false;
        IsEditorVisible = true;

        StatusMessage =
            "Ficha cargada";
    }

    private async Task SaveAsync()
    {
        ClearError();

        if (!ValidateEditor())
        {
            return;
        }

        IsBusy = true;

        try
        {
            using var scope =
                _scopeFactory.CreateScope();

            var now =
                DateTimeOffset.UtcNow;

            if (EditingId is null)
            {
                var useCase =
                    scope.ServiceProvider
                        .GetRequiredService<
                            CreatePersonUseCase>();

                EditingId =
                    await useCase.ExecuteAsync(
                        new CreatePersonCommand(
                            FirstName,
                            LastName,
                            EmptyToNull(Phone),
                            EmptyToNull(Email),
                            EmptyToNull(
                                PrivateNote),
                            IsFavorite,
                            BuildBirthWriteModel(),
                            BuildResidenceWriteModel()),
                        now);
            }
            else
            {
                var getUseCase =
                    scope.ServiceProvider
                        .GetRequiredService<
                            GetPersonUseCase>();

                var current =
                    await getUseCase
                        .ExecuteAsync(
                            EditingId.Value)
                    ?? throw new
                        KeyNotFoundException(
                            "La persona ya no existe.");

                var updateUseCase =
                    scope.ServiceProvider
                        .GetRequiredService<
                            UpdatePersonUseCase>();

                await updateUseCase
                    .ExecuteAsync(
                        new UpdatePersonCommand(
                            EditingId.Value,
                            FirstName,
                            LastName,
                            EmptyToNull(Phone),
                            EmptyToNull(Email),
                            EmptyToNull(
                                PrivateNote),
                            IsFavorite,
                            BuildBirthWriteModel(),
                            BuildResidenceWriteModel()),
                        now);
            }

            IsDirty = false;

            StatusMessage =
                "Guardado";

            await RefreshAsync();

            if (EditingId is not null)
            {
                SelectedPerson =
                    People.FirstOrDefault(
                        x => x.Id
                             == EditingId.Value);
            }
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RecordConsultationAsync()
    {
        if (EditingId is null)
        {
            return;
        }

        IsBusy = true;
        ClearError();

        try
        {
            var now =
                DateTimeOffset.UtcNow;

            using var scope =
                _scopeFactory.CreateScope();

            var useCase =
                scope.ServiceProvider
                    .GetRequiredService<
                        RecordPersonConsultationUseCase>();

            await useCase.ExecuteAsync(
                EditingId.Value,
                now);

            LastConsultationAtUtc =
                now;

            StatusMessage =
                "Última consulta actualizada";

            await RefreshAsync();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteAsync()
    {
        if (EditingId is null)
        {
            return;
        }

        if (!_deleteArmed)
        {
            _deleteArmed = true;

            OnPropertyChanged(
                nameof(DeleteButtonText));

            StatusMessage =
                "Pulsa otra vez para confirmar la eliminación.";

            return;
        }

        IsBusy = true;
        ClearError();

        try
        {
            using var scope =
                _scopeFactory.CreateScope();

            var useCase =
                scope.ServiceProvider
                    .GetRequiredService<
                        DeletePersonUseCase>();

            await useCase.ExecuteAsync(
                EditingId.Value,
                confirmed: true);

            ClearEditor();

            await RefreshAsync();

            StatusMessage =
                "Persona eliminada";
        }
        catch (Exception ex)
        {
            _deleteArmed = false;

            OnPropertyChanged(
                nameof(DeleteButtonText));

            ShowError(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void CancelEdit()
    {
        ClearError();

        if (IsDirty && !_cancelArmed)
        {
            _cancelArmed = true;

            OnPropertyChanged(
                nameof(CancelButtonText));

            StatusMessage =
                "Hay cambios sin guardar. Pulsa otra vez para descartarlos.";

            return;
        }

        if (IsDirty)
        {
            StatusMessage =
                "Cambios descartados";
        }

        ClearEditor();
    }

    private void ClearEditor()
    {
        ClearValidationErrors();

        EditingId = null;

        SetEditorValues(
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            false,
            null);

        _deleteArmed = false;

        OnPropertyChanged(
            nameof(DeleteButtonText));

        IsDirty = false;
        IsEditorVisible = false;
    }

    private void SetEditorValues(
        string firstName,
        string lastName,
        string phone,
        string email,
        string privateNote,
        bool isFavorite,
        DateTimeOffset?
            lastConsultationAtUtc)
    {
        _firstName = firstName;
        _lastName = lastName;
        _phone = phone;
        _email = email;
        _privateNote = privateNote;
        _isFavorite = isFavorite;
        _lastConsultationAtUtc =
            lastConsultationAtUtc;

        OnPropertyChanged(
            nameof(FirstName));

        OnPropertyChanged(
            nameof(LastName));

        OnPropertyChanged(
            nameof(Phone));

        OnPropertyChanged(
            nameof(Email));

        OnPropertyChanged(
            nameof(PrivateNote));

        OnPropertyChanged(
            nameof(IsFavorite));

        OnPropertyChanged(
            nameof(LastConsultationAtUtc));

        OnPropertyChanged(
            nameof(LastConsultationText));
    }

    private void MarkDirty()
    {
        if (IsEditorVisible)
        {
            IsDirty = true;

            if (_deleteArmed)
            {
                _deleteArmed = false;

                OnPropertyChanged(
                    nameof(DeleteButtonText));
            }

            if (_cancelArmed)
            {
                _cancelArmed = false;

                OnPropertyChanged(
                    nameof(CancelButtonText));
            }
        }
    }

    private void RaiseCommands()
    {
        foreach (var command in new ICommand[]
        {
            OpenSelectedCommand,
            SaveCommand,
            CancelCommand,
            SetConsultationNowCommand,
            DeleteCommand
        })
        {
            switch (command)
            {
                case AsyncCommand asyncCommand:
                    asyncCommand
                        .RaiseCanExecuteChanged();
                    break;

                case DelegateCommand commandValue:
                    commandValue
                        .RaiseCanExecuteChanged();
                    break;
            }
        }
    }

    private void ShowError(
        Exception exception)
    {
        ErrorMessage =
            HumanizeError(exception);

        OnPropertyChanged(
            nameof(HasError));

        StatusMessage =
            "No guardado";
    }

    private void ClearError()
    {
        ErrorMessage =
            string.Empty;

        OnPropertyChanged(
            nameof(HasError));
    }

    private static string HumanizeError(
        Exception exception)
        => exception switch
        {
            ArgumentException =>
                "Revisa los datos introducidos.",

            KeyNotFoundException =>
                "La persona ya no está disponible.",

            InvalidOperationException =>
                "No se puede completar la operación con los datos actuales.",

            _ =>
                "No se ha podido completar la operación."
        };

    private static string? EmptyToNull(
        string value)
        => string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();

    private static BirthDataWriteModel
        ToWriteModel(
            BirthDataReadModel source)
        => new(
            source.LocalDate,
            source.TimePrecision,
            source.LocalTime,
            source.RangeStart,
            source.RangeEnd,
            source.DayPeriod,
            source.GeoNameId,
            source.Locality,
            source.Country,
            source.Region,
            source.Subregion,
            source.Latitude,
            source.Longitude,
            source.IanaTimeZoneId,
            source.TzdbVersion,
            source.ResolutionState,
            source.HistoricalOffsetSeconds,
            source.ResolvedInstantUtc,
            source.AmbiguousEarlierOffsetSeconds,
            source.AmbiguousEarlierInstantUtc,
            source.AmbiguousLaterOffsetSeconds,
            source.AmbiguousLaterInstantUtc,
            source.AmbiguousSelectedCandidate,
            source.AmbiguousSelectionRecordedAtUtc,
            source.ManualCoordinateOverride,
            source.OriginalGeoNamesLatitude,
            source.OriginalGeoNamesLongitude);

    private static CurrentResidenceWriteModel
        ToWriteModel(
            CurrentResidenceReadModel source)
        => new(
            source.Locality,
            source.GeoNameId,
            source.Region,
            source.Country,
            source.Latitude,
            source.Longitude,
            source.IanaTimeZoneId,
            source.UpdatedAtUtc);

    private bool SetField<T>(
        ref T field,
        T value,
        [CallerMemberName]
        string? propertyName = null)
    {
        if (EqualityComparer<T>
            .Default
            .Equals(
                field,
                value))
        {
            return false;
        }

        field = value;

        OnPropertyChanged(
            propertyName);

        return true;
    }

    private void OnPropertyChanged(
        [CallerMemberName]
        string? propertyName = null)
        => PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(
                propertyName));
}
