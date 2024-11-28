using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TechTask.Persistence;
using TechTask.Services;

namespace TechTask.ConsoleApp;

public class Program
{
    private readonly ILogger<Program> _logger;
    private readonly GroupedItemService _groupedItemService;

    
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        host.Services.GetRequiredService<Program>().Run();
    }
    public Program(GroupedItemService groupedItemService, ILogger<Program> logger)
    {
        _logger= logger;
        _groupedItemService = groupedItemService;
    }
    
    public async Task Run()
    {
        _logger.LogInformation("Task is starting..");

       await _groupedItemService.GroupItems(200f);

        _logger.LogInformation("Task finised succesfully..");
    }
    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddTransient<GroupedItemService>();
                services.AddDbContext<AppDbContext>(options => options.UseNpgsql("Host=localhost;Username=postgres;Password=gudron;Database=techTask"));
            });
    }


}
