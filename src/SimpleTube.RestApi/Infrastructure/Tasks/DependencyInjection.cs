namespace SimpleTube.RestApi.Infrastructure.Tasks;

internal static class DependencyInjection
{
    public static IServiceCollection AddTasks(this IServiceCollection services) =>
        services.AddSingleton(typeof(ITaskQueue<>), typeof(Internal.TaskQueue<>));
}
