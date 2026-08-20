namespace Miastro.UI.Avalonia.Services;

public sealed class UserErrorService : IUserErrorService
{
    public string GetUserMessage(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return "No se pudo completar la operación. "
             + "Consulta el registro técnico si el problema continúa.";
    }
}
