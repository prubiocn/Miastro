using System.Diagnostics;
using System.Text.Json;
using Miastro.Astronomy.Abstractions.Models;
using Miastro.Domain.Calculation;
using Miastro.Domain.Houses;
using Miastro.Domain.Objects;
using Miastro.Infrastructure.SwissEphemeris.Calculation;
using Miastro.Infrastructure.SwissEphemeris.Configuration;
using Miastro.Infrastructure.SwissEphemeris.Diagnostics;
using Miastro.Infrastructure.SwissEphemeris.Houses;

if (args.Length != 1)
{
    Console.Error.WriteLine(
        "Usage: benchmark <repository-root>");

    return 2;
}

var root =
    Path.GetFullPath(args[0]);

var nativeManifest =
    Path.Combine(
        root,
        "third_party",
        "swisseph",
        "native-manifest.json");

using var json =
    JsonDocument.Parse(
        File.ReadAllText(nativeManifest));

var hash =
    json.RootElement
        .GetProperty("sha256")
        .GetString()
    ?? throw new InvalidOperationException(
        "No existe hash de libswe.so.");

var options =
    new SwissEphemerisOptions(
        Path.Combine(
            root,
            "src",
            "Miastro.Infrastructure.SwissEphemeris",
            "native",
            "linux-x64",
            "libswe.so"),
        Path.Combine(
            root,
            "data",
            "ephemeris"),
        hash,
        "2.10.03");

var instant =
    AstronomicalInstant.FromUtc(
        new DateTimeOffset(
            2024, 1, 1,
            12, 0, 0,
            TimeSpan.Zero));

var location =
    new GeographicLocation(
        40.4168,
        -3.7038);

var diagnostics =
    new SwissEphemerisDiagnostics(
        options);

var positionCalculator =
    new SwissEphemerisPositionCalculator(
        options);

var houseCalculator =
    new SwissEphemerisHouseCalculator(
        options);

var stopwatch =
    Stopwatch.StartNew();

var diagnostic =
    diagnostics.Diagnose();

stopwatch.Stop();

if (!diagnostic.LibraryLoaded ||
    !diagnostic.AbiCompatible)
{
    throw new InvalidOperationException(
        "El diagnóstico nativo no pasó.");
}

var initializationMs =
    stopwatch.Elapsed.TotalMilliseconds;

stopwatch.Restart();

_ = positionCalculator.Calculate(
    AstrologicalObjectId.Sun,
    instant,
    CalculationProfile.MiastroV1);

stopwatch.Stop();

var firstCalculationMs =
    stopwatch.Elapsed.TotalMilliseconds;

var planets =
    new[]
    {
        AstrologicalObjectId.Sun,
        AstrologicalObjectId.Moon,
        AstrologicalObjectId.Mercury,
        AstrologicalObjectId.Venus,
        AstrologicalObjectId.Mars,
        AstrologicalObjectId.Jupiter,
        AstrologicalObjectId.Saturn,
        AstrologicalObjectId.Uranus,
        AstrologicalObjectId.Neptune,
        AstrologicalObjectId.Pluto
    };

stopwatch.Restart();

foreach (var planet in planets)
{
    _ = positionCalculator.Calculate(
        planet,
        instant,
        CalculationProfile.MiastroV1);
}

stopwatch.Stop();

var tenPlanetsMs =
    stopwatch.Elapsed.TotalMilliseconds;

stopwatch.Restart();

var placidus =
    houseCalculator.Calculate(
        instant,
        location,
        HouseSystem.Placidus);

stopwatch.Stop();

if (!placidus.Success)
{
    throw new InvalidOperationException(
        "Placidus benchmark failed.");
}

var housesMs =
    stopwatch.Elapsed.TotalMilliseconds;

Console.WriteLine(
    $"InitializationMs={initializationMs:F3}");

Console.WriteLine(
    $"FirstCalculationIncludingEphemerisValidationMs={firstCalculationMs:F3}");

Console.WriteLine(
    $"TenPlanetsMs={tenPlanetsMs:F3}");

Console.WriteLine(
    $"PlacidusHousesMs={housesMs:F3}");

Console.WriteLine(
    $"EngineVersion={diagnostic.EngineVersion}");

Console.WriteLine(
    $"Architecture={diagnostic.Architecture}");

return 0;
