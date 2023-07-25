using FantasyFootball.DataAccess.Models;

namespace FantasyFootball.Logic.Interfaces
{
    public interface ILeagueLogic
    {
        public Task<League?> AddLeague(string leagueName, int leagueID, string rulesCookie, string projectionsCookie);
        public Task<League?> EditLeague(League league);
        public Task<bool> RemoveLeague(League league);
        public Task<League> SelectLeague(int ESPNID);
        public Task<List<League>> ViewAllLeagues();

        
    }
}
