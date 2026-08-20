using Miastro.Domain.Angles;
using Miastro.Domain.Houses;
using Miastro.Domain.Objects;
using Miastro.Domain.Zodiac;

namespace Miastro.Domain.Placements;

public sealed record AstrologicalPlacement
{
    public AstrologicalObjectId ObjectId { get; }

    public EclipticLongitude Longitude { get; }

    public ZodiacSign Sign { get; }

    public double DegreeInSign { get; }

    public AstrologicalHouse? House { get; }

    public double? SpeedDegreesPerDay { get; }

    public MotionState? Motion { get; }

    public bool? IsRetrograde =>
        Motion switch
        {
            MotionState.Retrograde => true,
            MotionState.Direct => false,
            MotionState.Stationary => false,
            null => null,
            _ => null
        };

    public AstrologicalPlacement(
        AstrologicalObjectId objectId,
        EclipticLongitude longitude,
        AstrologicalHouse? house = null,
        double? speedDegreesPerDay = null)
    {
        _ = AstrologicalObjectCatalog.GetCategory(objectId);

        if (speedDegreesPerDay is not null &&
            !double.IsFinite(speedDegreesPerDay.Value))
        {
            throw new ArgumentOutOfRangeException(
                nameof(speedDegreesPerDay));
        }

        var zodiacPosition =
            ZodiacPosition.FromLongitude(longitude);

        ObjectId = objectId;
        Longitude = longitude;
        Sign = zodiacPosition.Sign;
        DegreeInSign = zodiacPosition.DegreeInSign;
        House = house;
        SpeedDegreesPerDay = speedDegreesPerDay;
        Motion = speedDegreesPerDay is null
            ? null
            : MotionStateResolver.FromSpeed(
                speedDegreesPerDay.Value);
    }
}
