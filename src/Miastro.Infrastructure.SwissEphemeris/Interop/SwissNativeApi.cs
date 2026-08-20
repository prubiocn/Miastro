using System.Runtime.InteropServices;

namespace Miastro.Infrastructure.SwissEphemeris.Interop;

internal sealed class SwissNativeApi
{
    private const int VersionBufferLength = 256;
    private const int ErrorBufferLength = 256;
    private const int PositionArrayLength = 6;
    private const int HouseCuspArrayLength = 13;
    private const int HouseAuxiliaryArrayLength = 20;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate IntPtr SweVersionDelegate(
        IntPtr buffer);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void SweSetEphePathDelegate(
        IntPtr path);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate double SweJuldayDelegate(
        int year,
        int month,
        int day,
        double hour,
        int gregorianFlag);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int SweCalcUtDelegate(
        double julianDayUt,
        int body,
        int flags,
        IntPtr result,
        IntPtr errorBuffer);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int SweHousesExDelegate(
        double julianDayUt,
        int flags,
        double latitudeDegrees,
        double longitudeDegrees,
        int houseSystem,
        IntPtr cusps,
        IntPtr auxiliary);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate void SweCloseDelegate();

    private readonly SweVersionDelegate _version;
    private readonly SweSetEphePathDelegate _setEphemerisPath;
    private readonly SweJuldayDelegate _julianDay;
    private readonly SweCalcUtDelegate _calculateUt;
    private readonly SweHousesExDelegate _housesEx;
    private readonly SweCloseDelegate _close;

    public SwissNativeApi(
        IntPtr libraryHandle)
    {
        if (libraryHandle == IntPtr.Zero)
        {
            throw new ArgumentException(
                "El handle nativo no puede ser cero.",
                nameof(libraryHandle));
        }

        _version =
            GetDelegate<SweVersionDelegate>(
                libraryHandle,
                "swe_version");

        _setEphemerisPath =
            GetDelegate<SweSetEphePathDelegate>(
                libraryHandle,
                "swe_set_ephe_path");

        _julianDay =
            GetDelegate<SweJuldayDelegate>(
                libraryHandle,
                "swe_julday");

        _calculateUt =
            GetDelegate<SweCalcUtDelegate>(
                libraryHandle,
                "swe_calc_ut");

        _housesEx =
            GetDelegate<SweHousesExDelegate>(
                libraryHandle,
                "swe_houses_ex");

        _close =
            GetDelegate<SweCloseDelegate>(
                libraryHandle,
                "swe_close");
    }

    public string GetVersion()
    {
        var buffer =
            Marshal.AllocHGlobal(
                VersionBufferLength);

        try
        {
            Clear(
                buffer,
                VersionBufferLength);

            var result =
                _version(buffer);

            if (result == IntPtr.Zero)
            {
                throw new InvalidOperationException(
                    "swe_version devolvió NULL.");
            }

            return Marshal.PtrToStringAnsi(buffer)
                ?? throw new InvalidOperationException(
                    "swe_version devolvió una cadena inválida.");
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    public void SetEphemerisPath(
        string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var pointer =
            Marshal.StringToHGlobalAnsi(path);

        try
        {
            _setEphemerisPath(pointer);
        }
        finally
        {
            Marshal.FreeHGlobal(pointer);
        }
    }

    public double JulianDay(
        int year,
        int month,
        int day,
        double hour,
        int gregorianFlag) =>
        _julianDay(
            year,
            month,
            day,
            hour,
            gregorianFlag);

    public NativePositionResult CalculateUt(
        double julianDayUt,
        int body,
        int flags)
    {
        var resultPointer =
            Marshal.AllocHGlobal(
                sizeof(double) *
                PositionArrayLength);

        var errorPointer =
            Marshal.AllocHGlobal(
                ErrorBufferLength);

        try
        {
            Clear(
                resultPointer,
                sizeof(double) *
                PositionArrayLength);

            Clear(
                errorPointer,
                ErrorBufferLength);

            var returnedFlags =
                _calculateUt(
                    julianDayUt,
                    body,
                    flags,
                    resultPointer,
                    errorPointer);

            var values =
                new double[
                    PositionArrayLength];

            Marshal.Copy(
                resultPointer,
                values,
                0,
                PositionArrayLength);

            var error =
                Marshal.PtrToStringAnsi(
                    errorPointer)
                ?? string.Empty;

            return new(
                returnedFlags,
                values,
                error);
        }
        finally
        {
            Marshal.FreeHGlobal(
                resultPointer);

            Marshal.FreeHGlobal(
                errorPointer);
        }
    }

    public NativeHouseResult CalculateHouses(
        double julianDayUt,
        double latitudeDegrees,
        double longitudeDegrees,
        int houseSystemCode)
    {
        var cuspsPointer =
            Marshal.AllocHGlobal(
                sizeof(double) *
                HouseCuspArrayLength);

        var auxiliaryPointer =
            Marshal.AllocHGlobal(
                sizeof(double) *
                HouseAuxiliaryArrayLength);

        try
        {
            Clear(
                cuspsPointer,
                sizeof(double) *
                HouseCuspArrayLength);

            Clear(
                auxiliaryPointer,
                sizeof(double) *
                HouseAuxiliaryArrayLength);

            // Flags = 0:
            // tropical, degrees, no sidereal mode.
            var returnCode =
                _housesEx(
                    julianDayUt,
                    0,
                    latitudeDegrees,
                    longitudeDegrees,
                    houseSystemCode,
                    cuspsPointer,
                    auxiliaryPointer);

            var cusps =
                new double[
                    HouseCuspArrayLength];

            var auxiliary =
                new double[
                    HouseAuxiliaryArrayLength];

            Marshal.Copy(
                cuspsPointer,
                cusps,
                0,
                HouseCuspArrayLength);

            Marshal.Copy(
                auxiliaryPointer,
                auxiliary,
                0,
                HouseAuxiliaryArrayLength);

            return new(
                returnCode,
                cusps,
                auxiliary);
        }
        finally
        {
            Marshal.FreeHGlobal(
                cuspsPointer);

            Marshal.FreeHGlobal(
                auxiliaryPointer);
        }
    }

    public void Close() =>
        _close();

    private static void Clear(
        IntPtr pointer,
        int length)
    {
        Marshal.Copy(
            new byte[length],
            0,
            pointer,
            length);
    }

    private static T GetDelegate<T>(
        IntPtr libraryHandle,
        string symbol)
        where T : Delegate
    {
        var address =
            NativeLibrary.GetExport(
                libraryHandle,
                symbol);

        return Marshal
            .GetDelegateForFunctionPointer<T>(
                address);
    }
}

internal sealed record NativePositionResult(
    int ReturnedFlags,
    double[] Values,
    string NativeError);

internal sealed record NativeHouseResult(
    int ReturnCode,
    double[] Cusps,
    double[] Auxiliary);
