using Miastro.Graphics.Layout;
using Miastro.Graphics.Layout.Placement;
using Miastro.Graphics.Scene.Natal;
using Miastro.Graphics.Scene.Natal.Aspects;

namespace Miastro.Graphics.Adapters.Natal;

public sealed record NatalSnapshotGraphicsModel(
    Guid ChartId,
    NatalWheelLayoutSnapshot Wheel,
    NatalObjectPlacementSnapshot Placements,
    IReadOnlyList<NatalSceneObjectInput> Objects,
    IReadOnlyList<NatalAspectSceneInput> Aspects);
