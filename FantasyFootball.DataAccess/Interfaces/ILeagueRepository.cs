
using FantasyFootball.DataAccess.Models;

namespace FantasyFootball.DataAccess.Interfaces
{
    public interface ILeagueRepository
    {

        public Task DeleteLeague(int leagueID);
        public Task InsertLeague(string leagueName, int ESPNID, string rulesCookie, string projectionsCookie);
        public Task<List<League>> FetchAllLeagues();
        public Task<League> FetchLeague(int ESPNID);
        public Task UpdateLeague(League league);
        public Task<bool> ValidateProjectionsCookie(int ESPNID, string projectionsCookie);
        public Task<bool> ValidateRulesCookie(int ESPNID, string rulesCookie);

    }
}
