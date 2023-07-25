using FantasyFootball.DataAccess.Interfaces;
using FantasyFootball.DataAccess.Models;
using FantasyFootball.Logic.Interfaces;

namespace FantasyFootball.Logic.Implementations
{
    public class LeagueLogic : ILeagueLogic
    {
        public ILeagueRepository _leagueRepository;
        public LeagueLogic(ILeagueRepository leagueRepository)
        {
            _leagueRepository = leagueRepository;
        }

        public async Task<League?> AddLeague(string leagueName, int ESPNID, string rulesCookie, string projectionsCookie)
        {
            if (await _leagueRepository.ValidateRulesCookie(ESPNID, rulesCookie) == false || await _leagueRepository.ValidateProjectionsCookie(ESPNID, projectionsCookie) == false)
                return null;
            await _leagueRepository.InsertLeague(leagueName, ESPNID, rulesCookie, projectionsCookie);
            return await _leagueRepository.FetchLeague(ESPNID);
        }

        public async Task<League?> EditLeague(League league)
        {
            if (await _leagueRepository.ValidateRulesCookie(league.ESPNID, league.RulesCookie) == false || await _leagueRepository.ValidateProjectionsCookie(league.ESPNID, league.ProjectionsCookie) == false)
                return null;
            await _leagueRepository.UpdateLeague(league);
            return await _leagueRepository.FetchLeague(league.ESPNID);
        }

        public async Task<bool> RemoveLeague(League league)
        {
            var leagueToRemove = await _leagueRepository.FetchLeague(league.ESPNID);
            if (league.LeagueName == leagueToRemove.LeagueName && league.LeagueID == leagueToRemove.LeagueID && league.ESPNID == leagueToRemove.ESPNID && league.RulesCookie == leagueToRemove.RulesCookie && league.ProjectionsCookie == leagueToRemove.ProjectionsCookie)
            {
                await _leagueRepository.DeleteLeague(league.LeagueID);
                return true;
            }
            return false;
        }

        public async Task<League> SelectLeague(int ESPNID)
        {
            return await _leagueRepository.FetchLeague(ESPNID);
        }

        public async Task<List<League>> ViewAllLeagues()
        {
            return await _leagueRepository.FetchAllLeagues();
        }
    }
}
