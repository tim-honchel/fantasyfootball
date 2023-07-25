using FantasyFootball.DataAccess.Interfaces;
using FantasyFootball.DataAccess.Models;
using FantasyFootball.Logic.Interfaces;

namespace FantasyFootball.Logic.Implementations
{
    public class DataImportLogic : IDataImportLogic
    {
        public IPlayersRepository _playersRepository;
        public IRulesRepository _rulesRepository;
        public IHelperLogic _helperLogic;
        public DataImportLogic(IPlayersRepository playersRepository, IRulesRepository rulesRepository, IHelperLogic helperLogic)
        {
            _playersRepository = playersRepository;
            _rulesRepository = rulesRepository;
            _helperLogic = helperLogic;
        }
        public async Task<List<Player>> FetchProjections(int ESPNLeagueID, string projectionsCookie, Rules rules)
        {
            List<Player> players = await _playersRepository.RequestProjections(ESPNLeagueID, projectionsCookie);
            if (players == null)
                return null;
            if (await _helperLogic.ValidateProjections(players, rules) == false)
                return null;
            return players;
        }

        public async Task<Rules> FetchRules(int ESPNLeagueID, string rulesCookie)
        {
            Rules rules = await _rulesRepository.RequestRules(ESPNLeagueID, rulesCookie);
            if (rules == null)
                return null;
            if (_helperLogic.ValidateRules(rules) == false)
                return null;
            return rules;
        }
    }
}
