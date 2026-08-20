#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
DOMAIN="$ROOT/src/Miastro.Domain"

cat > "$DOMAIN/Zodiac/ZodiacAxis.cs" <<'EOF'
namespace Miastro.Domain.Zodiac;

public readonly record struct ZodiacAxis
{
    public ZodiacSign First { get; }

    public ZodiacSign Second { get; }

    public ZodiacAxis(
        ZodiacSign first,
        ZodiacSign second)
    {
        if (GetOpposite(first) != second)
        {
            throw new ArgumentException(
                "Los signos no forman un eje zodiacal válido.");
        }

        First = first;
        Second = second;
    }

    public static ZodiacSign GetOpposite(ZodiacSign sign)
    {
        var value = (int)sign;

        if (value is < 0 or > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(sign));
        }

        return (ZodiacSign)((value + 6) % 12);
    }
}
EOF

cat > "$DOMAIN/Houses/HouseAxis.cs" <<'EOF'
namespace Miastro.Domain.Houses;

public readonly record struct HouseAxis
{
    public AstrologicalHouse First { get; }

    public AstrologicalHouse Second { get; }

    public HouseAxis(
        AstrologicalHouse first,
        AstrologicalHouse second)
    {
        if (first.Opposite != second)
        {
            throw new ArgumentException(
                "Las casas no forman un eje válido.");
        }

        First = first;
        Second = second;
    }
}
EOF

cd "$ROOT"

dotnet build Miastro.sln \
  --configuration Release \
  --no-restore
