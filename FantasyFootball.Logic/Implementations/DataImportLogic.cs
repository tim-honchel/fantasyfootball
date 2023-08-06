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
            List<Player> qb = await _playersRepository.RequestProjections(ESPNLeagueID, projectionsCookie, "[0]",(int)(1.2* rules.Teams * (rules.QB + rules.Bench * 0.1)));
            List<Player> rb = await _playersRepository.RequestProjections(ESPNLeagueID, projectionsCookie, "[2]", (int)(1.2 * rules.Teams * (rules.RB +rules.FLEX * 0.6 + rules.Bench * 0.5)));
            List<Player> wr = await _playersRepository.RequestProjections(ESPNLeagueID, projectionsCookie, "[4]", (int)(1.2 * rules.Teams * (rules.WR + +rules.FLEX * 0.4 + rules.Bench * 0.3)));
            List<Player> te = await _playersRepository.RequestProjections(ESPNLeagueID, projectionsCookie, "[6]", (int)(1.2 * rules.Teams * (rules.TE + rules.Bench * 0.1)));
            List<Player> def = await _playersRepository.RequestProjections(ESPNLeagueID, projectionsCookie, "[16]", (int)(1.2 * rules.Teams * rules.DEF));
            List<Player> k = await _playersRepository.RequestProjections(ESPNLeagueID, projectionsCookie, "[17]", (int)(1.2 * rules.Teams * rules.K));
            List<Player> players = qb.Concat(rb).Concat(wr).Concat(te).Concat(def).Concat(k).ToList();
            if (players == null)
                return null;
            if (_helperLogic.ValidateProjections(players, rules) == false)
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
