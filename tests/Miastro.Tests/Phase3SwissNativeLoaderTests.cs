using Miastro.Astronomy.Abstractions.Diagnostics;
using Miastro.Astronomy.Abstractions.Errors;
using Miastro.Infrastructure.SwissEphemeris.Configuration;
using Miastro.Infrastructure.SwissEphemeris.Diagnostics;

namespace Miastro.Tests;

[TestClass]
public sealed class Phase3SwissNativeLoaderTests
{
    private const string ExpectedHash =
        "47e6fed985ccb5f067b7a0f6f746ec3567a7b54ce5f86140b2138616a8e6a653";

    [TestMethod]
    public void Controlled_native_library_loads_and_reports_version()
    {
        var root = FindRepositoryRoot();

        var library = Path.Combine(
            root,
            "src",
            "Miastro.Infrastructure.SwissEphemeris",
            "native",
            "linux-x64",
            "libswe.so");

        var ephemeris = Path.Combine(
            root,
            "data",
            "ephemeris");

        var options = new SwissEphemerisOptions(
            library,
            ephemeris,
            ExpectedHash,
            "2.10.03");

        var diagnostic =
            new SwissEphemerisDiagnostics(options)
                .Diagnose();

        Assert.IsTrue(diagnostic.LibraryAvailable);
        Assert.IsTrue(diagnostic.LibraryLoaded);
        Assert.IsTrue(diagnostic.AbiCompatible);
        Assert.AreEqual(
            "2.10.03",
            diagnostic.EngineVersion);
        Assert.AreEqual(
            Path.GetFullPath(library),
            diagnostic.LoadedLibraryPath);
    }

    [TestMethod]
    public void Missing_library_is_reported_without_native_crash()
    {
        var root = FindRepositoryRoot();

        var options = new SwissEphemerisOptions(
            Path.Combine(
                root,
                "does-not-exist",
                "libswe.so"),
            Path.Combine(root, "data", "ephemeris"),
            null,
            "2.10.03");

        var diagnostic =
            new SwissEphemerisDiagnostics(options)
                .Diagnose();

        Assert.IsFalse(diagnostic.LibraryAvailable);
        Assert.IsFalse(diagnostic.LibraryLoaded);
        Assert.AreEqual(
            "SWISS_LIBRARY_NOT_FOUND",
            diagnostic.TechnicalStatus);
    }

    [TestMethod]
    public void Wrong_hash_is_rejected()
    {
        var root = FindRepositoryRoot();

        var library = Path.Combine(
            root,
            "src",
            "Miastro.Infrastructure.SwissEphemeris",
            "native",
            "linux-x64",
            "libswe.so");

        var options = new SwissEphemerisOptions(
            library,
            Path.Combine(root, "data", "ephemeris"),
            new string('0', 64),
            "2.10.03");

        var diagnostic =
            new SwissEphemerisDiagnostics(options)
                .Diagnose();

        Assert.IsFalse(diagnostic.LibraryLoaded);
        Assert.IsFalse(diagnostic.AbiCompatible);
        Assert.AreEqual(
            "SWISS_LIBRARY_HASH_MISMATCH",
            diagnostic.TechnicalStatus);
    }

    private static string FindRepositoryRoot()
    {
        var directory =
            new DirectoryInfo(
                AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                    Path.Combine(
                        directory.FullName,
                        "Miastro.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "No se encontró la raíz del repositorio.");
    }
}
