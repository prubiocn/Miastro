using Miastro.Astronomy.Abstractions.Diagnostics;

namespace Miastro.Astronomy.Abstractions.Contracts;

public interface IAstronomyEngineDiagnostics
{
    AstronomyEngineDiagnostic Diagnose();
}
