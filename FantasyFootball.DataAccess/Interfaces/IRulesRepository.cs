using FantasyFootball.DataAccess.Models;

namespace FantasyFootball.DataAccess.Interfaces
{
    public interface IRulesRepository
    {

        public Task<Rules> RequestRules(int ESPNLeagueID, string rulesCookie);

    }
}
