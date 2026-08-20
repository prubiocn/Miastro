namespace Miastro.UI.Avalonia.States;

public sealed class OperationState
{
    public bool IsLoading { get; private set; }

    public string? UserMessage { get; private set; }

    public void Begin()
    {
        IsLoading = true;
        UserMessage = null;
    }

    public void Complete()
    {
        IsLoading = false;
        UserMessage = null;
    }

    public void Fail(string userMessage)
    {
        IsLoading = false;
        UserMessage = userMessage;
    }
}
