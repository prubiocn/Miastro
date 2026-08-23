using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using Miastro.Application.Natal;
using Miastro.Domain.Houses;
using Miastro.Domain.Natal;
using Miastro.UI.Avalonia.Commands;

namespace Miastro.UI.Avalonia.ViewModels;

public sealed partial class MainWindowViewModel
{
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

        _natalCalculationFailed =
            false;
    }

    private void ResetNatalDisplay()
    {
        _currentNatalSnapshot =
            null;

        NatalPlacements.Clear();

        _natalCalculationFailed =
            false;

        NatalStatusMessage =
            "Carta natal no calculada.";
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
