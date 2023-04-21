using System.Text.Json.Nodes;

namespace Common;

public static class TaskExtensions
{
    public static async Task<JsonObject?> TimeoutAfter(this Task<JsonObject> task, TimeSpan timeout)
    {
        using var timeoutCancellationTokenSource = new CancellationTokenSource();
        var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
        if (completedTask != task)
        {
            return null;
        }

        timeoutCancellationTokenSource.Cancel();
        return await task;
    }
}