
using TechTask.Persistence;
using TechTask.Services;

namespace TechTask.BackgroundTask;

public class GroupItemsTask : IHostedService, IDisposable
{
    private readonly IServiceProvider _serviceProvider;

    private Timer? _timer;

    public GroupItemsTask(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(GroupItems, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

        return Task.CompletedTask;
    }

    private void GroupItems(object? state)
    {
        using var scope = _serviceProvider.CreateScope();
        var groupedItemService = scope.ServiceProvider.GetRequiredService<GroupedItemService>();
        groupedItemService.GroupItems(200f);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
