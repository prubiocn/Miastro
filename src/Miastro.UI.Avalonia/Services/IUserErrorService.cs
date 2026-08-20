namespace Miastro.UI.Avalonia.Services;

public interface IUserErrorService
{
    string GetUserMessage(Exception exception);
}
