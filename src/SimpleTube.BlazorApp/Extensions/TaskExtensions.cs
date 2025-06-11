namespace SimpleTube.BlazorApp.Extensions;

internal static class TaskExtensions
{
    public static async void FireAndForget(
        this ValueTask task,
        Action<Exception>? errorHandler = null
    )
    {
        try
        {
            await task;
        }
        catch (Exception exception)
        {
            errorHandler?.Invoke(exception);
        }
    }

    public static async void FireAndForget(this Task task, Action<Exception>? errorHandler = null)
    {
        try
        {
            await task;
        }
        catch (Exception exception)
        {
            errorHandler?.Invoke(exception);
        }
    }
}
