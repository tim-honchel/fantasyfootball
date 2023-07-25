using FantasyFootball.DataAccess.Interfaces;
using FantasyFootball.DataAccess.Models;

namespace FantasyFootball.DataAccess.Implementations
{
    public class LeagueRepository : ILeagueRepository
    {
        public Task DeleteLeague(int leagueID)
        {
            throw new NotImplementedException();
        }

        public Task<List<League>> FetchAllLeagues()
        {
            throw new NotImplementedException();
        }

        public Task<League> FetchLeague(int ESPNID)
        {
            throw new NotImplementedException();
        }

        public Task InsertLeague(string leagueName, int ESPNID, string rulesCookie, string projectionsCookie)
        {
            throw new NotImplementedException();
        }

        public Task UpdateLeague(League league)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateProjectionsCookie(int ESPNID, string projectionsCookie)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateRulesCookie(int ESPNID, string rulesCookie)
        {
            throw new NotImplementedException();
        }
    }
}
