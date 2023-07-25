using FantasyFootball.DataAccess.Implementations;
using FantasyFootball.DataAccess.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FantasyFootball.DataAccess.Configuration
{
    public static class DataAccessConfiguration
    {
        public static IServiceCollection AddDataScope(this IServiceCollection services)
        {
            services.AddTransient<ILeagueRepository, LeagueRepository>();
            services.AddTransient<IPlayersRepository, PlayersRepository>();
            services.AddTransient<IRulesRepository, RulesRepository>();
            return services;
        }
    }
}
