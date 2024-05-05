using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SpaceTraders.Models;
using SpaceTraders.Repositories;
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

        // Add repositories as singletons
        builder.Services.AddSingleton<IAgentRepository, AgentRepository>();
        builder.Services.AddSingleton<IShipRepository, ShipRepository>();
        builder.Services.AddSingleton<IContractRepository, ContractRepository>();
        builder.Services.AddSingleton<IWaypointRepository, WaypointRepository>();
        builder.Services.AddSingleton<IFactionRepository, FactionRepository>();
        builder.Services.AddSingleton<ITokenRepository, TokenRepository>();
        builder.Services.AddSingleton<IMarketRepository, MarketRepository>();
        builder.Services.AddSingleton<IShipInfoRepository, ShipInfoRepository>();

        //Add throttle service:
        builder.Services.AddSingleton<IThrottleService, ThrottleService>();

        //Add authentication handler:
        builder.Services.AddTransient<AuthenticationHandler>();

        builder.Services.AddTransient<IErrorDecoder, ErrorDecoder>();

        // Add the SpaceTradersApp class itself
        builder.Services.AddHostedService<SpaceTradersApp>();

        //Add services:
        builder.Services.AddTransient<IShipService, ShipService>();
        builder.Services.AddTransient<IContractService, ContractService>();
        builder.Services.AddTransient<IWaypointService, WaypointService>();
        builder.Services.AddTransient<ILogInService, LogInService>();
        builder.Services.AddTransient<ITransactionService, TransactionService>();
        builder.Services.AddTransient<IMarketService, MarketService>();
        builder.Services.AddTransient<IOrdersService, OrdersService>();

        using IHost host = builder.Build();

        await host.RunAsync();
    }
}