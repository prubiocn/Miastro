using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Miastro.Astronomy.Abstractions.Errors;
using Miastro.Infrastructure.SwissEphemeris.Configuration;
using Miastro.Infrastructure.SwissEphemeris.Interop;

namespace Miastro.Infrastructure.SwissEphemeris.Loading;

internal sealed class SwissNativeLibrary : IDisposable
{
    private IntPtr _handle;
    private bool _disposed;

    public string LibraryPath { get; }

    public SwissNativeApi Api { get; }

    private SwissNativeLibrary(
        IntPtr handle,
        string libraryPath)
    {
        _handle = handle;
        LibraryPath = libraryPath;
        Api = new SwissNativeApi(handle);
    }

    public static SwissNativeLibrary Load(
        SwissEphemerisOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        EnsureLinuxX64();

        var path =
            Path.GetFullPath(
                options.NativeLibraryPath);

        if (!File.Exists(path))
        {
            throw CreateException(
                AstronomyErrorCode.LibraryNotFound,
                "SWISS_LIBRARY_NOT_FOUND",
                "El motor astronómico no está disponible.",
                $"No existe: {path}");
        }

        if (options.ExpectedSha256 is not null)
        {
            ValidateSha256(
                path,
                options.ExpectedSha256);
        }

        IntPtr handle;

        try
        {
            handle = NativeLibrary.Load(path);
        }
        catch (Exception ex)
            when (ex is DllNotFoundException
                or BadImageFormatException)
        {
            throw CreateException(
                AstronomyErrorCode.LibraryNotLoadable,
                "SWISS_LIBRARY_LOAD_FAILED",
                "El motor astronómico no puede iniciarse.",
                ex.Message,
                ex);
        }

        try
        {
            return new SwissNativeLibrary(
                handle,
                path);
        }
        catch
        {
            NativeLibrary.Free(handle);
            throw;
        }
    }

    private static void EnsureLinuxX64()
    {
        if (!OperatingSystem.IsLinux() ||
            RuntimeInformation.ProcessArchitecture !=
            Architecture.X64)
        {
            throw CreateException(
                AstronomyErrorCode.AbiIncompatible,
                "SWISS_ABI_UNSUPPORTED",
                "El motor astronómico no es compatible con esta plataforma.",
                $"OS={RuntimeInformation.OSDescription}; " +
                $"Arch={RuntimeInformation.ProcessArchitecture}");
        }
    }

    private static void ValidateSha256(
        string path,
        string expected)
    {
        using var stream =
            File.OpenRead(path);

        var actual =
            Convert.ToHexString(
                SHA256.HashData(stream))
            .ToLowerInvariant();

        if (!string.Equals(
                actual,
                expected,
                StringComparison.OrdinalIgnoreCase))
        {
            throw CreateException(
                AstronomyErrorCode.AbiIncompatible,
                "SWISS_LIBRARY_HASH_MISMATCH",
                "El motor astronómico no supera la validación de integridad.",
                $"Expected={expected}; Actual={actual}");
        }
    }

    private static AstronomyEngineException CreateException(
        AstronomyErrorCode code,
        string technicalCode,
        string safeMessage,
        string technicalDetail,
        Exception? inner = null) =>
        new(
            new AstronomyError(
                code,
                technicalCode,
                safeMessage),
            technicalDetail,
            inner);

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            Api.Close();
        }
        finally
        {
            if (_handle != IntPtr.Zero)
            {
                NativeLibrary.Free(_handle);
                _handle = IntPtr.Zero;
            }

            _disposed = true;
        }
    }
}
