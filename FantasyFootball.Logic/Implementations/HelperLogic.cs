using FantasyFootball.DataAccess.Models;
using FantasyFootball.Logic.Interfaces;

namespace FantasyFootball.Logic.Implementations
{
    public class HelperLogic : IHelperLogic
    {
        public Task<bool> ValidateProjections(List<Player> players, Rules rules)
        {
            throw new NotImplementedException();
        }
        public bool ValidateRules(Rules rules)
        {
            return rules.QB > 0 && rules.RB > 0 && rules.WR > 0 && rules.TE > 0 && rules.DEF > 0 && rules.K > 0 && rules.OtherPositions == false && rules.Keepers == false && rules.AuctionDraft == true && rules.SalaryCap > 0 && rules.Teams >= 4;            
        }
    }
}
