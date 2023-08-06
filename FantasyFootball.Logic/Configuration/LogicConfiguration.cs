using FantasyFootball.Logic.Implementations;
using FantasyFootball.Logic.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FantasyFootball.Logic.Configuration
{
    public static class LogicConfiguration
    {
        public static IServiceCollection AddDataScope(this IServiceCollection services)
        {
            services.AddTransient<ILeagueLogic, LeagueLogic>();
            services.AddTransient<IDataImportLogic, DataImportLogic>();
            services.AddTransient<IDraftStrategyLogic, DraftStrategyLogic>();
            services.AddTransient<IHelperLogic, HelperLogic>();
            return services;
        }

    }
}
