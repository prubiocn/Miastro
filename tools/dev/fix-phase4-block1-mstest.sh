#!/usr/bin/env bash
set -euo pipefail

ROOT="/home/pablo/Aplicaciones/Miastro"
cd "$ROOT"

echo "=== MIASTRO — FASE 4 / FIX MSTEST BLOQUE 1 ==="

# ------------------------------------------------------------
# 1. DataTestMethod -> TestMethod
# ------------------------------------------------------------

sed -i \
  's/\[DataTestMethod\]/[TestMethod]/g' \
  tests/Miastro.Tests/Phase4CoordinateTests.cs

# ------------------------------------------------------------
# 2. Assert.ThrowsException -> try/catch explícito
# ------------------------------------------------------------

cat > tests/Miastro.Tests/Phase4CoordinateTests.cs <<'EOF'
using Miastro.Domain.Geography;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase4CoordinateTests
{
    [TestMethod]
    [DataRow(-90d)]
    [DataRow(90d)]
    [DataRow(0d)]
    public void Latitude_AcceptsBoundaries(double value)
    {
        Assert.AreEqual(value, new Latitude(value).Value);
    }

    [TestMethod]
    [DataRow(-90.0001)]
    [DataRow(90.0001)]
    public void Latitude_RejectsOutOfRange(double value)
    {
        ArgumentOutOfRangeException? captured = null;

        try
        {
            _ = new Latitude(value);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            captured = ex;
        }

        Assert.IsNotNull(captured);
    }

    [TestMethod]
    [DataRow(-180d)]
    [DataRow(180d)]
    [DataRow(0d)]
    public void Longitude_AcceptsBoundaries(double value)
    {
        Assert.AreEqual(value, new Longitude(value).Value);
    }

    [TestMethod]
    [DataRow(-180.0001)]
    [DataRow(180.0001)]
    public void Longitude_RejectsOutOfRange(double value)
    {
        ArgumentOutOfRangeException? captured = null;

        try
        {
            _ = new Longitude(value);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            captured = ex;
        }

        Assert.IsNotNull(captured);
    }
}
EOF

# ------------------------------------------------------------
# 3. Architecture tests sin StringAssert.DoesNotContain
# ------------------------------------------------------------

cat > tests/Miastro.Tests/Phase4ArchitectureTests.cs <<'EOF'
namespace Miastro.Tests;

[TestClass]
public sealed class Phase4ArchitectureTests
{
    private static readonly string Root =
        Path.GetFullPath(
            Path.Combine(
                AppContext.BaseDirectory,
                "../../../../../"));

    [TestMethod]
    public void Domain_DoesNotReferenceNodaTime()
    {
        var files = Directory.GetFiles(
            Path.Combine(Root, "src/Miastro.Domain"),
            "*",
            SearchOption.AllDirectories)
            .Where(x =>
                x.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) ||
                x.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            .Where(x =>
                !x.Contains("/bin/", StringComparison.Ordinal) &&
                !x.Contains("/obj/", StringComparison.Ordinal));

        foreach (var file in files)
        {
            var text = File.ReadAllText(file);

            Assert.IsFalse(
                text.Contains(
                    "NodaTime",
                    StringComparison.Ordinal),
                $"Domain references NodaTime: {file}");
        }
    }

    [TestMethod]
    public void Ui_DoesNotUseNodaTimeOrSqliteDirectly()
    {
        var files = Directory.GetFiles(
            Path.Combine(Root, "src/Miastro.UI.Avalonia"),
            "*",
            SearchOption.AllDirectories)
            .Where(x =>
                x.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) ||
                x.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            .Where(x =>
                !x.Contains("/bin/", StringComparison.Ordinal) &&
                !x.Contains("/obj/", StringComparison.Ordinal));

        foreach (var file in files)
        {
            var text = File.ReadAllText(file);

            Assert.IsFalse(
                text.Contains(
                    "NodaTime",
                    StringComparison.Ordinal),
                $"UI references NodaTime: {file}");

            Assert.IsFalse(
                text.Contains(
                    "SqliteConnection",
                    StringComparison.Ordinal),
                $"UI references SqliteConnection: {file}");

            Assert.IsFalse(
                text.Contains(
                    "Microsoft.Data.Sqlite",
                    StringComparison.Ordinal),
                $"UI references Microsoft.Data.Sqlite: {file}");
        }
    }
}
EOF

# ------------------------------------------------------------
# 4. Historical time typed-error test
# ------------------------------------------------------------

python3 - <<'PY'
from pathlib import Path
import re

p = Path(
    "tests/Miastro.Tests/Phase4HistoricalTimeTests.cs"
)

text = p.read_text()

pattern = re.compile(
r'''    \[TestMethod\]
    public void UnknownZone_IsTypedError\(\)
    \{
.*?
    \}
''',
re.S,
)

replacement = '''    [TestMethod]
    public void UnknownZone_IsTypedError()
    {
        HistoricalTimeException? captured = null;

        try
        {
            _resolver.Resolve(
                new LocalDateTime(2024, 1, 1, 12, 0),
                new IanaTimeZoneId("Etc/DefinitelyMissing"));
        }
        catch (HistoricalTimeException ex)
        {
            captured = ex;
        }

        Assert.IsNotNull(captured);

        Assert.AreEqual(
            HistoricalTimeErrorCode.UnknownTimeZone,
            captured.Code);
    }
'''

new_text, count = pattern.subn(
    replacement,
    text,
    count=1
)

if count != 1:
    raise SystemExit(
        "ERROR: no se pudo parchear UnknownZone_IsTypedError"
    )

p.write_text(new_text)
PY

# ------------------------------------------------------------
# 5. Geography async typed-error test
# ------------------------------------------------------------

python3 - <<'PY'
from pathlib import Path
import re

p = Path(
    "tests/Miastro.Tests/Phase4GeographySearchTests.cs"
)

text = p.read_text()

pattern = re.compile(
r'''    \[TestMethod\]
    public async Task MissingCatalog_IsTypedError\(\)
    \{
.*?
    \}
''',
re.S,
)

replacement = '''    [TestMethod]
    public async Task MissingCatalog_IsTypedError()
    {
        var service = new SqliteLocationSearchService(
            new GeoNamesCatalogOptions(
                Path.Combine(
                    Path.GetTempPath(),
                    Guid.NewGuid() + ".sqlite")));

        GeographyException? captured = null;

        try
        {
            await service.SearchAsync(
                new LocationSearchQuery("Madrid"));
        }
        catch (GeographyException ex)
        {
            captured = ex;
        }

        Assert.IsNotNull(captured);

        Assert.AreEqual(
            GeographyErrorCode.CatalogMissing,
            captured.Code);
    }
'''

new_text, count = pattern.subn(
    replacement,
    text,
    count=1
)

if count != 1:
    raise SystemExit(
        "ERROR: no se pudo parchear MissingCatalog_IsTypedError"
    )

p.write_text(new_text)
PY

# ------------------------------------------------------------
# 6. Verificar que no quedan APIs MSTest incompatibles
# ------------------------------------------------------------

if grep -RInE \
  'DataTestMethod|ThrowsException|ThrowsExceptionAsync|StringAssert\.DoesNotContain' \
  tests/Miastro.Tests/Phase4*.cs
then
    echo "ERROR: quedan APIs MSTest incompatibles."
    exit 820
fi

echo "MSTestCompatibilityPatch=PASS"

# ------------------------------------------------------------
# 7. Compilar y ejecutar tests
# ------------------------------------------------------------

dotnet build \
  Miastro.sln \
  --configuration Release \
  --no-restore

dotnet test \
  tests/Miastro.Tests/Miastro.Tests.csproj \
  --configuration Release \
  --no-build \
  --logger "console;verbosity=minimal"

# ------------------------------------------------------------
# 8. Revalidar fixture
# ------------------------------------------------------------

test -f \
  data/geography/generated/geonames.sqlite

test -f \
  data/geography/generated/manifest.json

python3 - <<'PY'
import sqlite3
from pathlib import Path

db = Path(
    "data/geography/generated/geonames.sqlite"
).resolve()

con = sqlite3.connect(
    f"file:{db}?mode=ro",
    uri=True
)

rows = con.execute(
    "SELECT COUNT(*) FROM locations"
).fetchone()[0]

schema = con.execute(
    "SELECT value FROM metadata "
    "WHERE key='schema_version'"
).fetchone()[0]

fts = con.execute(
    "SELECT COUNT(*) FROM sqlite_master "
    "WHERE type='table' "
    "AND name='location_fts'"
).fetchone()[0]

con.close()

assert rows == 8
assert schema == "1"
assert fts == 1

print("GeoFixtureRows=8")
print("GeoSchema=PASS")
print("GeoFts5=PASS")
print("GeoReadOnly=PASS")
PY

# ------------------------------------------------------------
# 9. Arquitectura
# ------------------------------------------------------------

if grep -RIl 'NodaTime' \
  src/Miastro.Domain \
  --include='*.cs' \
  --include='*.csproj' \
  | grep -q .
then
    echo "ERROR: Domain contiene NodaTime."
    exit 821
fi

if grep -RIlE \
  'NodaTime|Microsoft\.Data\.Sqlite|SqliteConnection' \
  src/Miastro.UI.Avalonia \
  --include='*.cs' \
  --include='*.csproj' \
  | grep -q .
then
    echo "ERROR: UI contiene dependencias prohibidas."
    exit 822
fi

echo "ArchitectureBoundaries=PASS"

git diff --check

echo
echo "=== FASE 4 / BLOQUE 1 — FIX COMPLETADO ==="
echo "Build=PASS"
echo "Tests=PASS"
echo "MSTestCompatibility=PASS"
echo "GeoFixture=PASS"
echo "GeoReadOnly=PASS"
echo "NodaTimeFoundation=PASS"
echo "ArchitectureBoundaries=PASS"
echo "Phase4Closed=NO"
echo "AcceptancePASS=0"
echo "AcceptanceFAIL=0"
echo "AcceptancePENDING=57"
echo "Phase5Started=NO"
echo
git status --short
