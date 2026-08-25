using Miastro.Domain.Objects;
using Miastro.Graphics.Scene.Natal.Configuration;
using Avalonia.Media.Imaging;
using Miastro.Graphics.Interaction;
using Miastro.UI.Avalonia.Services;
using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Miastro.Application.Natal;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.UI.Avalonia.Commands;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed partial class MainWindowViewModel
{
    private const double DefaultNatalWheelSize =
        640.0;

    private double _natalWheelViewportWidth =
        DefaultNatalWheelSize;

    private double _natalWheelViewportHeight =
        DefaultNatalWheelSize;

    private double _natalWheelRenderScaling =
        1.0;

    private string? _selectedNatalObjectId;

    private readonly NatalWheelPresentationService
        _natalWheelPresentationService = new();

    private NatalWheelPresentation?
        _natalWheelPresentation;

    private Bitmap?
        _natalWheelBitmap;

    private string _selectedNatalObjectText =
        "Ningún objeto seleccionado.";

    private string _natalWheelTooltipText =
        string.Empty;

    private NatalPlacementRowViewModel?
        _selectedNatalPlacement;

    private bool _showNatalPlanets =
        true;

    private bool _showNatalPoints =
        true;

    private bool _showNatalAspects =
        true;

    private bool _showNatalCusps =
        true;

    private bool _showNatalLabels =
        true;

    private NatalWheelModeChoice
        _selectedNatalWheelMode = null!;

    private NatalHouseSystemChoice
        _selectedNatalHouseSystem = null!;

    private NatalChartSnapshotReadModel?
        _currentNatalSnapshot;

    private string _natalStatusMessage =
        "Carta natal no calculada.";

    private bool _natalCalculationFailed;

    public IReadOnlyList<NatalHouseSystemChoice>
        NatalHouseSystems { get; private set; }
        = Array.Empty<NatalHouseSystemChoice>();

    public ObservableCollection<
        NatalPlacementRowViewModel>
        NatalPlacements { get; } = [];

    public ObservableCollection<
        NatalAspectRowViewModel>
        NatalAspects { get; } = [];

    public AsyncCommand CalculateNatalCommand
        { get; private set; } = null!;

    public NatalHouseSystemChoice
        SelectedNatalHouseSystem
    {
        get =>
            _selectedNatalHouseSystem;

        set
        {
            if (SetField(
                ref _selectedNatalHouseSystem,
                value))
            {
                OnPropertyChanged(
                    nameof(
                        SelectedNatalHouseSystemText));

                CalculateNatalCommand?
                    .RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsNatalSectionVisible
        => EditingId is not null;

    public bool HasCurrentNatalChart
        => _currentNatalSnapshot is not null;

    public bool HasNatalPlacements
        => NatalPlacements.Count > 0;

    public bool HasNatalAspects
        => NatalAspects.Count > 0;

    public Bitmap? NatalWheelBitmap
        => _natalWheelBitmap;

    public bool HasNatalWheel
        => _natalWheelPresentation is not null
           && _natalWheelBitmap is not null;

    public string SelectedNatalObjectText
        => _selectedNatalObjectText;

    public string NatalWheelTooltipText
        => _natalWheelTooltipText;

    public string NatalWheelAccessibilityText
        =>
            _selectedNatalPlacement is null
                ? "Rueda natal interactiva. Usa las flechas para recorrer planetas y puntos visibles."
                : "Rueda natal. Seleccionado "
                  + _selectedNatalPlacement.ObjectName
                  + ". "
                  + _selectedNatalPlacement.PositionText
                  + ". "
                  + _selectedNatalPlacement.HouseText
                  + ". "
                  + _selectedNatalPlacement.MotionText
                  + ". Usa las flechas para cambiar de objeto.";

    public NatalPlacementRowViewModel?
        SelectedNatalPlacement
    {
        get =>
            _selectedNatalPlacement;

        set
        {
            if (ReferenceEquals(
                _selectedNatalPlacement,
                value))
            {
                return;
            }

            ApplyNatalWheelSelection(
                value?.ObjectId.ToString());
        }
    }

    public bool HasSelectedNatalObject
        => _selectedNatalPlacement is not null;

    public IReadOnlyList<NatalWheelModeChoice>
        NatalWheelModes { get; private set; }
        = Array.Empty<NatalWheelModeChoice>();

    public NatalWheelModeChoice
        SelectedNatalWheelMode
    {
        get => _selectedNatalWheelMode;

        set
        {
            if (SetField(
                ref _selectedNatalWheelMode,
                value))
            {
                RebuildNatalWheel();
            }
        }
    }

    public bool ShowNatalPlanets
    {
        get => _showNatalPlanets;

        set
        {
            if (SetField(
                ref _showNatalPlanets,
                value))
            {
                RebuildNatalWheel();
            }
        }
    }

    public bool ShowNatalPoints
    {
        get => _showNatalPoints;

        set
        {
            if (SetField(
                ref _showNatalPoints,
                value))
            {
                RebuildNatalWheel();
            }
        }
    }

    public bool ShowNatalAspects
    {
        get => _showNatalAspects;

        set
        {
            if (SetField(
                ref _showNatalAspects,
                value))
            {
                RebuildNatalWheel();
            }
        }
    }

    public bool ShowNatalCusps
    {
        get => _showNatalCusps;

        set
        {
            if (SetField(
                ref _showNatalCusps,
                value))
            {
                RebuildNatalWheel();
            }
        }
    }

    public bool ShowNatalLabels
    {
        get => _showNatalLabels;

        set
        {
            if (SetField(
                ref _showNatalLabels,
                value))
            {
                RebuildNatalWheel();
            }
        }
    }

    public bool NatalCalculationFailed
        => _natalCalculationFailed;

    public string NatalStatusMessage
    {
        get =>
            _natalStatusMessage;

        private set =>
            SetField(
                ref _natalStatusMessage,
                value);
    }

    public string NatalAvailabilityText
        => HasCurrentNatalChart
            ? "Carta natal disponible"
            : "Todavía no hay carta natal vigente.";

    public string NatalCalculatedAtText
        => _currentNatalSnapshot is null
            ? string.Empty
            : "Calculada: "
              + _currentNatalSnapshot
                  .CalculatedAtUtc
                  .ToLocalTime()
                  .ToString(
                      "dd/MM/yyyy HH:mm");

    public string CurrentNatalHouseSystemText
        => _currentNatalSnapshot is null
            ? string.Empty
            : "Sistema de casas: "
              + HouseSystemLabel(
                  _currentNatalSnapshot
                      .HouseSystem);

    public string SelectedNatalHouseSystemText
        => SelectedNatalHouseSystem.Label;

    public bool CanCalculateNatal
        => EditingId is not null
           && !IsBusy
           && !IsDirty;

    public void InitializeNatalEditor()
    {
        NatalHouseSystems =
        [
            new(
                "Placidus",
                HouseSystem.Placidus),

            new(
                "Koch",
                HouseSystem.Koch)
        ];

        _selectedNatalHouseSystem =
            NatalHouseSystems[0];

        CalculateNatalCommand =
            new AsyncCommand(
                CalculateNatalAsync,
                () => CanCalculateNatal);

        NatalWheelModes =
        [
            new(
                "Consulta",
                NatalWheelViewMode.Consultation),

            new(
                "Presentación",
                NatalWheelViewMode.Presentation)
        ];

        _selectedNatalWheelMode =
            NatalWheelModes[0];
    }

    public async Task LoadNatalAsync(
        Guid personId)
    {
        ResetNatalDisplay();

        using var scope =
            _scopeFactory.CreateScope();

        var store =
            scope.ServiceProvider
                .GetRequiredService<
                    INatalChartStore>();

        var current =
            await store
                .GetCurrentAsync(
                    personId);

        if (current is null)
        {
            NatalStatusMessage =
                "Carta natal no calculada.";

            NotifyNatalChanged();
            return;
        }

        ApplyNatalSnapshot(
            current);

        NatalStatusMessage =
            "Carta natal calculada.";

        NotifyNatalChanged();
    }

    public void ResetNatalEditor()
    {
        ResetNatalDisplay();

        if (NatalHouseSystems.Count > 0)
        {
            _selectedNatalHouseSystem =
                NatalHouseSystems[0];
        }

        NotifyNatalChanged();
    }

    private async Task CalculateNatalAsync()
    {
        if (EditingId is null)
        {
            return;
        }

        if (IsDirty)
        {
            _natalCalculationFailed = true;

            NatalStatusMessage =
                "Guarda primero los cambios de la ficha.";

            NotifyNatalChanged();
            return;
        }

        IsBusy = true;
        ClearError();

        _natalCalculationFailed = false;

        NatalStatusMessage =
            "Calculando carta natal…";

        NotifyNatalChanged();

        try
        {
            using var scope =
                _scopeFactory.CreateScope();

            var useCase =
                scope.ServiceProvider
                    .GetRequiredService<
                        CalculateNatalChartUseCase>();

            var result =
                await useCase.ExecuteAsync(
                    EditingId.Value,
                    SelectedNatalHouseSystem.Value,
                    DateTimeOffset.UtcNow);

            if (!result.Success
                || result.Snapshot is null)
            {
                _natalCalculationFailed = true;

                NatalStatusMessage =
                    HumanNatalFailure(
                        result.Code);

                NotifyNatalChanged();
                return;
            }

            ApplyNatalSnapshot(
                result.Snapshot);

            NatalStatusMessage =
                result.Code ==
                    NatalCalculationResultCode
                        .ExistingCurrentSnapshot
                    ? "Carta natal vigente cargada."
                    : "Carta natal calculada.";

            StatusMessage =
                NatalStatusMessage;

            NotifyNatalChanged();

            await RefreshAsync();
        }
        catch (Exception ex)
        {
            _natalCalculationFailed = true;

            NatalStatusMessage =
                "No se ha podido calcular la carta natal.";

            ShowError(ex);

            NotifyNatalChanged();
        }
        finally
        {
            IsBusy = false;

            CalculateNatalCommand
                .RaiseCanExecuteChanged();
        }
    }

    private void ApplyNatalSnapshot(
        NatalChartSnapshotReadModel snapshot)
    {
        _selectedNatalObjectId =
            null;

        _selectedNatalPlacement =
            null;

        _selectedNatalObjectText =
            "Ningún objeto seleccionado.";

        _currentNatalSnapshot =
            snapshot;

        var choice =
            NatalHouseSystems
                .FirstOrDefault(x =>
                    x.Value ==
                    snapshot.HouseSystem);

        if (choice is not null)
        {
            _selectedNatalHouseSystem =
                choice;

            OnPropertyChanged(
                nameof(
                    SelectedNatalHouseSystem));
        }

        NatalPlacements.Clear();

        foreach (
            var placement
            in snapshot.Placements)
        {
            NatalPlacements.Add(
                NatalPlacementRowViewModel
                    .From(
                        placement));
        }

        NatalAspects.Clear();

        foreach (
            var aspect
            in snapshot.Aspects)
        {
            NatalAspects.Add(
                NatalAspectRowViewModel
                    .From(
                        aspect));
        }

        _natalCalculationFailed =
            false;

        RebuildNatalWheel();
}

    private void ResetNatalDisplay()
    {
        _currentNatalSnapshot =
            null;

        NatalPlacements.Clear();
        NatalAspects.Clear();

        _natalCalculationFailed =
            false;

        NatalStatusMessage =
            "Carta natal no calculada.";

        ClearNatalWheel();
}

    private void NotifyNatalChanged()
    {
        foreach (
            var property
            in new[]
            {
                nameof(
                    IsNatalSectionVisible),

                nameof(
                    HasCurrentNatalChart),

                nameof(
                    HasNatalPlacements),

                nameof(
                    HasNatalAspects),

                nameof(
                    NatalCalculationFailed),

                nameof(
                    NatalAvailabilityText),

                nameof(
                    NatalCalculatedAtText),

                nameof(
                    CurrentNatalHouseSystemText),

                nameof(
                    SelectedNatalHouseSystemText),

                nameof(
                    CanCalculateNatal)
            })
        {
            OnPropertyChanged(
                property);
        }

        CalculateNatalCommand?
            .RaiseCanExecuteChanged();
    }

    private void RebuildNatalWheel()
    {
        if (_currentNatalSnapshot is null)
        {
            ClearNatalWheel();
            return;
        }

        var previousSelectionId =
            _selectedNatalObjectId;

        var presentation =
            _natalWheelPresentationService
                .Build(
                    _currentNatalSnapshot,
                    _natalWheelViewportWidth,
                    _natalWheelViewportHeight,
                    BuildNatalWheelConfiguration(),
                    _natalWheelRenderScaling);

        using var stream =
            new MemoryStream(
                presentation.PngBytes,
                writable: false);

        var bitmap =
            new Bitmap(stream);

        _natalWheelBitmap?.Dispose();

        _natalWheelPresentation =
            presentation;

        _natalWheelBitmap =
            bitmap;

        OnPropertyChanged(
            nameof(NatalWheelBitmap));

        OnPropertyChanged(
            nameof(HasNatalWheel));

        RestoreNatalWheelSelection(
            previousSelectionId);
    }

    private void RestoreNatalWheelSelection(
        string? objectId)
    {
        if (_natalWheelPresentation is null
            || objectId is null)
        {
            ApplyNatalWheelSelection(
                null);
            return;
        }

        var selectableIds =
            new NatalSceneHitTester()
                .GetSelectableObjectIds(
                    _natalWheelPresentation.Scene);

        var remainsVisible =
            selectableIds.Contains(
                objectId,
                StringComparer.Ordinal);

        ApplyNatalWheelSelection(
            remainsVisible
                ? objectId
                : null);
    }

    private void ClearNatalWheel()
    {
        _natalWheelBitmap?.Dispose();

        _natalWheelBitmap =
            null;

        _natalWheelPresentation =
            null;

        _selectedNatalObjectText =
            "Ningún objeto seleccionado.";

        _selectedNatalPlacement =
            null;

        _selectedNatalObjectId =
            null;

        OnPropertyChanged(
            nameof(NatalWheelBitmap));

        OnPropertyChanged(
            nameof(HasNatalWheel));

        OnPropertyChanged(
            nameof(SelectedNatalObjectText));

        OnPropertyChanged(
            nameof(SelectedNatalPlacement));

        OnPropertyChanged(
            nameof(HasSelectedNatalObject));

        OnPropertyChanged(
            nameof(NatalWheelAccessibilityText));
    }

    public void UpdateNatalWheelViewport(
        double width,
        double height,
        double renderScaling)
    {
        if (!double.IsFinite(width)
            || !double.IsFinite(height)
            || !double.IsFinite(renderScaling)
            || width <= 0.0
            || height <= 0.0
            || renderScaling <= 0.0)
        {
            return;
        }

        if (Math.Abs(
                _natalWheelViewportWidth
                - width)
                < 0.5
            && Math.Abs(
                _natalWheelViewportHeight
                - height)
                < 0.5
            && Math.Abs(
                _natalWheelRenderScaling
                - renderScaling)
                < 0.01)
        {
            return;
        }

        _natalWheelViewportWidth =
            width;

        _natalWheelViewportHeight =
            height;

        _natalWheelRenderScaling =
            renderScaling;

        if (_currentNatalSnapshot is not null)
        {
            RebuildNatalWheel();
        }
    }

    public void UpdateNatalWheelTooltipAt(
        double x,
        double y,
        double viewportWidth,
        double viewportHeight)
    {
        if (_natalWheelPresentation is null
            || viewportWidth <= 0.0
            || viewportHeight <= 0.0)
        {
            ClearNatalWheelTooltip();
            return;
        }

        var hit =
            new NatalSceneHitTester()
                .HitTestViewport(
                    _natalWheelPresentation.Scene,
                    x,
                    y,
                    viewportWidth,
                    viewportHeight,
                    tolerance: 5.0);

        var row =
            hit is null
                ? null
                : FindNatalPlacementRow(
                    hit.ObjectId);

        var next =
            row is null
                ? string.Empty
                : row.ObjectName
                    + "\n"
                    + row.PositionText
                    + " · "
                    + row.HouseText
                    + (
                        row.MotionText == "—"
                            ? string.Empty
                            : " · "
                              + row.MotionText
                      );

        if (string.Equals(
            _natalWheelTooltipText,
            next,
            StringComparison.Ordinal))
        {
            return;
        }

        _natalWheelTooltipText =
            next;

        OnPropertyChanged(
            nameof(
                NatalWheelTooltipText));
    }

    public void ClearNatalWheelTooltip()
    {
        if (_natalWheelTooltipText.Length == 0)
        {
            return;
        }

        _natalWheelTooltipText =
            string.Empty;

        OnPropertyChanged(
            nameof(
                NatalWheelTooltipText));
    }

    public void SelectNatalWheelAt(
        double x,
        double y,
        double viewportWidth,
        double viewportHeight)
    {
        if (_natalWheelPresentation is null)
        {
            return;
        }

        var hit =
            new NatalSceneHitTester()
                .HitTestViewport(
                    _natalWheelPresentation.Scene,
                    x,
                    y,
                    viewportWidth,
                    viewportHeight,
                    tolerance: 5.0);

        ApplyNatalWheelSelection(
            hit?.ObjectId);
    }

    public void MoveNatalWheelSelection(
        int delta)
    {
        if (_natalWheelPresentation is null
            || delta == 0)
        {
            return;
        }

        var ids =
            new NatalSceneHitTester()
                .GetSelectableObjectIds(
                    _natalWheelPresentation.Scene);

        if (ids.Count == 0)
        {
            return;
        }

        var currentIndex =
            _selectedNatalObjectId is null
                ? -1
                : ids
                    .Select(
                        (id, index) =>
                            (
                                Id: id,
                                Index: index
                            ))
                    .Where(
                        item =>
                            item.Id
                                == _selectedNatalObjectId)
                    .Select(
                        item =>
                            item.Index)
                    .DefaultIfEmpty(-1)
                    .First();

        var nextIndex =
            currentIndex < 0
                ? delta > 0
                    ? 0
                    : ids.Count - 1
                : (
                    currentIndex
                    + delta % ids.Count
                    + ids.Count
                  )
                  % ids.Count;

        ApplyNatalWheelSelection(
            ids[nextIndex]);
    }

    public void SelectFirstNatalWheelObject()
    {
        SelectNatalWheelBoundary(
            last: false);
    }

    public void SelectLastNatalWheelObject()
    {
        SelectNatalWheelBoundary(
            last: true);
    }

    public void ClearNatalWheelSelection()
    {
        ApplyNatalWheelSelection(
            null);
    }

    private void SelectNatalWheelBoundary(
        bool last)
    {
        if (_natalWheelPresentation is null)
        {
            return;
        }

        var ids =
            new NatalSceneHitTester()
                .GetSelectableObjectIds(
                    _natalWheelPresentation.Scene);

        if (ids.Count == 0)
        {
            return;
        }

        ApplyNatalWheelSelection(
            last
                ? ids[^1]
                : ids[0]);
    }

    private void ApplyNatalWheelSelection(
        string? objectId)
    {
        if (objectId is not null
            && _natalWheelPresentation is not null)
        {
            var selectableIds =
                new NatalSceneHitTester()
                    .GetSelectableObjectIds(
                        _natalWheelPresentation.Scene);

            if (!selectableIds.Contains(
                objectId,
                StringComparer.Ordinal))
            {
                objectId =
                    null;
            }
        }

        _selectedNatalObjectId =
            objectId;

        if (objectId is null)
        {
            _selectedNatalObjectText =
                "Ningún objeto seleccionado.";

            _selectedNatalPlacement =
                null;
        }
        else
        {
            _selectedNatalObjectText =
                $"Seleccionado: {objectId}";

            _selectedNatalPlacement =
                FindNatalPlacementRow(
                    objectId);
        }

        OnPropertyChanged(
            nameof(SelectedNatalObjectText));

        OnPropertyChanged(
            nameof(SelectedNatalPlacement));

        OnPropertyChanged(
            nameof(HasSelectedNatalObject));

        OnPropertyChanged(
            nameof(NatalWheelAccessibilityText));
    }

    private NatalWheelSceneConfiguration
        BuildNatalWheelConfiguration()
    {
        var mode =
            SelectedNatalWheelMode is null
                ? NatalWheelViewMode.Consultation
                : SelectedNatalWheelMode.Value;

        var labels =
            mode == NatalWheelViewMode.Presentation
                ? false
                : ShowNatalLabels;

        return new NatalWheelSceneConfiguration(
            mode,
            new NatalWheelVisibilityOptions(
                ShowPlanets: ShowNatalPlanets,
                ShowPoints: ShowNatalPoints,
                ShowAspects: ShowNatalAspects,
                ShowCusps: ShowNatalCusps,
                ShowLabels: labels));
    }

    private NatalPlacementRowViewModel?
        FindNatalPlacementRow(
            string objectId)
    {
        if (!Enum.TryParse<
            AstrologicalObjectId>(
                objectId,
                ignoreCase: false,
                out var parsed))
        {
            return null;
        }

        return NatalPlacements
            .FirstOrDefault(
                x =>
                    x.ObjectId == parsed);
    }

    private static string HumanNatalFailure(
        NatalCalculationResultCode code)
        => code switch
        {
            NatalCalculationResultCode
                .BirthDataMissing =>
                "Añade y guarda los datos de nacimiento antes de calcular.",

            NatalCalculationResultCode
                .BirthTimeInsufficient =>
                "La precisión de la hora no permite calcular una carta natal completa.",

            NatalCalculationResultCode
                .HistoricalTimeUnresolved =>
                "Resuelve y guarda primero la hora histórica de nacimiento.",

            NatalCalculationResultCode
                .InvalidCoordinates =>
                "Las coordenadas de nacimiento no son válidas.",

            NatalCalculationResultCode
                .HouseCalculationUnavailable =>
                "No se han podido calcular las casas para estos datos.",

            NatalCalculationResultCode
                .AstronomyCalculationFailed =>
                "No se ha podido completar el cálculo astronómico.",

            NatalCalculationResultCode
                .PersistenceFailed =>
                "La carta se calculó, pero no se pudo guardar.",

            _ =>
                "No se ha podido calcular la carta natal."
        };

    private static string HouseSystemLabel(
        HouseSystem houseSystem)
        => houseSystem switch
        {
            HouseSystem.Placidus =>
                "Placidus",

            HouseSystem.Koch =>
                "Koch",

            _ =>
                houseSystem.ToString()
        };
}
