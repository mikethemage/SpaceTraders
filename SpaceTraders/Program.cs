using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Quartz;
using SpaceTraders.Jobs;
using SpaceTraders.Models;
using SpaceTraders.Repositories;
using SpaceTraders.Repositories.DatabaseRepositories;
using SpaceTraders.Repositories.DatabaseRepositories.DbContexts;
using SpaceTraders.Repositories.MemoryOnlyRepositories;
using SpaceTraders.Services;

namespace SpaceTraders;

internal class Program
{
    static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        builder.Services.AddOptions<ClientConfig>()
                .Bind(builder.Configuration.GetSection(nameof(ClientConfig)))
                .ValidateDataAnnotations()
                .ValidateOnStart();

        // Add HttpClient
        builder.Services.AddHttpClient<ISpaceTradersApiService, SpaceTradersApiService>((serviceProvider, client) =>
            {
                client.BaseAddress = new Uri(serviceProvider.GetRequiredService<IOptions<ClientConfig>>().Value.ApiUrl!);
            })
            .AddHttpMessageHandler<AuthenticationHandler>();

        builder.Services.AddQuartzHostedService();

        // Add repositories as singletons
        builder.Services.AddSingleton<IAgentMemoryOnlyRepository, AgentMemoryOnlyRepository>();
        builder.Services.AddTransient<IAgentRepository, AgentDatabaseRepository>();
        builder.Services.AddSingleton<IShipRepository, ShipRepository>();
        builder.Services.AddSingleton<IContractMemoryOnlyRepository, ContractMemoryOnlyRepository>();
        builder.Services.AddTransient<IContractRepository, ContractDatabaseRepository>();
        builder.Services.AddSingleton<IWaypointMemoryOnlyRepository, WaypointMemoryOnlyRepository>();
        builder.Services.AddTransient<IWaypointRepository, WaypointDatabaseRepository>();
        builder.Services.AddSingleton<IFactionMemoryOnlyRepository, FactionMemoryOnlyRepository>();
        builder.Services.AddTransient<IFactionRepository, FactionDatabaseRepository>();
        builder.Services.AddSingleton<ITokenMemoryOnlyRepository, TokenMemoryOnlyRepository>();
        builder.Services.AddTransient<ITokenRepository, TokenDatabaseRepository>();
        builder.Services.AddSingleton<IMarketMemoryOnlyRepository, MarketMemoryOnlyRepository>();
        builder.Services.AddTransient<IMarketRepository, MarketDatabaseRepository>();
        builder.Services.AddSingleton<IShipInfoRepository, ShipInfoRepository>();

        //Add throttle service:
        builder.Services.AddSingleton<IThrottleService, ThrottleService>();

        //Add authentication handler:
        builder.Services.AddTransient<AuthenticationHandler>();

        builder.Services.AddTransient<IErrorDecoder, ErrorDecoder>();

        builder.Services.AddQuartz(q =>
        {
            q.ScheduleJob<SpaceTradersInitializationJob>(j =>
            {
                j.StartNow();
                j.WithDescription("Initialization");
            });

            q.AddJob<ProcessIdleShipJob>(j =>
            {
                j.WithIdentity("ProcessIdleShipJob");
                j.WithDescription("ProcessIdleShip");
                j.StoreDurably();
            });
        });

        //Add services:
        builder.Services.AddTransient<IShipService, ShipService>();
        builder.Services.AddTransient<IContractService, ContractService>();
        builder.Services.AddTransient<IWaypointService, WaypointService>();
        builder.Services.AddTransient<ILogInService, LogInService>();
        builder.Services.AddTransient<ITransactionService, TransactionService>();
        builder.Services.AddTransient<IMarketService, MarketService>();
        builder.Services.AddTransient<IOrdersService, OrdersService>();
        builder.Services.AddTransient<IAgentService, AgentService>();
        builder.Services.AddTransient<IFactionService, FactionService>();

        builder.Services.AddDbContext<RepositoryDbContext>(options =>
            options.UseSqlite("Data Source=SpaceTraders.db;")
            );

        using IHost host = builder.Build();

        await host.RunAsync();
    }
}