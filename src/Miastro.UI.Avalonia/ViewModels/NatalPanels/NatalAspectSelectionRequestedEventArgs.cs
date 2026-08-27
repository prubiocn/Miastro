using Miastro.Application.Natal.Reading;

namespace Miastro.UI.Avalonia.ViewModels.NatalPanels;

public sealed class NatalAspectSelectionRequestedEventArgs :
    EventArgs
{
    public NatalAspectMatrixCell Cell { get; }

    public NatalAspectSelectionRequestedEventArgs(
        NatalAspectMatrixCell cell)
    {
        ArgumentNullException.ThrowIfNull(
            cell);

        Cell =
            cell;
    }
}
