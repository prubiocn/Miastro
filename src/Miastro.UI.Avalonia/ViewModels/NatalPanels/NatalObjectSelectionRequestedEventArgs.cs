using Miastro.Domain.Objects;

namespace Miastro.UI.Avalonia.ViewModels.NatalPanels;

public sealed class NatalObjectSelectionRequestedEventArgs :
    EventArgs
{
    public AstrologicalObjectId ObjectId { get; }

    public NatalObjectSelectionRequestedEventArgs(
        AstrologicalObjectId objectId)
    {
        ObjectId =
            objectId;
    }
}
