using FantasyFootball.DataAccess.Models;
using FantasyFootball.Logic.Interfaces;

namespace FantasyFootball.Logic.Implementations
{
    public class HelperLogic : IHelperLogic
    {
        public bool ValidateProjections(List<Player> players, Rules rules)
        {
            Dictionary<string,double> minByPos = new Dictionary<string, double>();
            minByPos["QB"] = (rules.QB * rules.Bench *0.1) * rules.Teams;
            minByPos["RB"] = (rules.RB + rules.FLEX *0.75 +rules.Bench * 0.5) * rules.Teams;
            minByPos["WR"] = (rules.WR + rules.FLEX * 0.25 + rules.Bench *0.2) * rules.Teams;
            minByPos["TE"] = (rules.TE + rules.Bench *0.1) * rules.Teams;
            minByPos["DEF"] = rules.DEF * rules.Teams + 1;
            minByPos["K"] = rules.K * rules.Teams + 1;
            return true;
        }
        public bool ValidateRules(Rules rules)
        {
            return rules.QB > 0 && rules.RB > 0 && rules.WR > 0 && rules.TE > 0 && rules.DEF > 0 && rules.K > 0 && rules.OtherPositions == false && rules.Keepers == false && rules.AuctionDraft == true && rules.SalaryCap > 0 && rules.Teams >= 4;            
        }
    }
}
