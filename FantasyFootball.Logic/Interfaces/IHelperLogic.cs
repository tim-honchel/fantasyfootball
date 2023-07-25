using FantasyFootball.DataAccess.Models;

namespace FantasyFootball.Logic.Interfaces
{
    public interface IHelperLogic
    {
        public Task<bool> ValidateProjections(List<Player> players, Rules rules);
        public bool ValidateRules(Rules rules);
    }
}
