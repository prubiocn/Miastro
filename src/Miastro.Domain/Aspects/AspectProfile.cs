using Miastro.Domain.Objects;

namespace Miastro.Domain.Aspects;

public sealed class AspectProfile
{
    private readonly IReadOnlyList<AspectDefinition> _aspects;
    private readonly HashSet<AstrologicalObjectId> _participants;

    public string Id { get; }

    public IReadOnlyList<AspectDefinition> Aspects => _aspects;

    public double LuminaryOrbBonusDegrees { get; }

    public AspectProfile(
        string id,
        IEnumerable<AspectDefinition> aspects,
        IEnumerable<AstrologicalObjectId> participants,
        double luminaryOrbBonusDegrees)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(aspects);
        ArgumentNullException.ThrowIfNull(participants);

        if (!double.IsFinite(luminaryOrbBonusDegrees) ||
            luminaryOrbBonusDegrees < 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(luminaryOrbBonusDegrees));
        }

        var aspectArray = aspects.ToArray();

        if (aspectArray.Length == 0)
        {
            throw new ArgumentException(
                "El perfil debe contener al menos un aspecto.",
                nameof(aspects));
        }

        if (aspectArray
            .GroupBy(x => x.Kind)
            .Any(group => group.Count() > 1))
        {
            throw new ArgumentException(
                "El perfil no puede contener aspectos duplicados.",
                nameof(aspects));
        }

        var participantSet = new HashSet<AstrologicalObjectId>(
            participants);

        if (participantSet.Count == 0)
        {
            throw new ArgumentException(
                "El perfil debe contener participantes.",
                nameof(participants));
        }

        Id = id;
        _aspects = Array.AsReadOnly(aspectArray);
        _participants = participantSet;
        LuminaryOrbBonusDegrees = luminaryOrbBonusDegrees;
    }

    public bool IsParticipant(
        AstrologicalObjectId objectId) =>
        _participants.Contains(objectId);

    public double GetAllowedOrb(
        AspectDefinition definition,
        AstrologicalObjectId first,
        AstrologicalObjectId second)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var hasLuminary =
            first is AstrologicalObjectId.Sun
                or AstrologicalObjectId.Moon
            ||
            second is AstrologicalObjectId.Sun
                or AstrologicalObjectId.Moon;

        return definition.BaseOrbDegrees +
               (hasLuminary
                   ? LuminaryOrbBonusDegrees
                   : 0.0);
    }
}
