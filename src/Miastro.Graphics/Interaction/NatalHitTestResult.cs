using Miastro.Graphics.Geometry;

namespace Miastro.Graphics.Interaction;

public sealed record NatalHitTestResult(
    string ObjectId,
    NatalHitTargetKind Kind,
    ChartRect Bounds);
