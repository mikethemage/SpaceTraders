﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpaceTraders.Repositories;
using SpaceTraders.Services;

namespace SpaceTraders;

internal class Program
{
    static void Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder();

        builder.ConfigureServices(services =>
        {
            //services.AddLogging(logging => logging.AddConsole());

            // Add HttpClient
            services.AddHttpClient("SpaceTradersClient", client =>
            {
                client.BaseAddress = new Uri("https://api.spacetraders.io/v2/");
                // Add any additional HttpClient configuration here
            });

            // Add repositories as singletons
            services.AddSingleton<IAgentRepository, AgentRepository>();
            services.AddSingleton<IShipRepository, ShipRepository>();
            services.AddSingleton<IContractRepository, ContractRepository>();
            services.AddSingleton<IWaypointRepository, WaypointRepository>();
            services.AddSingleton<IFactionRepository, FactionRepository>();
            services.AddSingleton<ITokenRepository, TokenRepository>();
            services.AddSingleton<IMarketRepository, MarketRepository>();
            services.AddTransient<IErrorDecoder, ErrorDecoder>();
            services.AddSingleton<IShipInfoRepository, ShipInfoRepository>();

            // Add the SpaceTradersApp class itself
            services.AddSingleton<ISpaceTradersApiService, SpaceTradersApiService>();
            services.AddHostedService<SpaceTradersApp>();
        });

        var host = builder.Build();

        host.Run();
    }
}


