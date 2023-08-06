using FantasyFootball.DataAccess.Models;

namespace FantasyFootball.Logic.Interfaces
{
    public interface IHelperLogic
    {
        public bool ValidateProjections(List<Player> players, Rules rules);
        public bool ValidateRules(Rules rules);
    }
}
