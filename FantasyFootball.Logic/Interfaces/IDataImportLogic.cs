using FantasyFootball.DataAccess.Models;

namespace FantasyFootball.Logic.Interfaces
{
    public interface IDataImportLogic
    {
        public Task<List<Player>> FetchProjections(int ESPNLeagueID, string projectionsCookie, Rules rules);
        public Task<Rules> FetchRules(int ESPNLeagueID, string rulesCookie);
       
    }
}
