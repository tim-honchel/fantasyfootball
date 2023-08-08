

using FantasyFootball.DataAccess.Implementations;
using FantasyFootball.DataAccess.Models;

namespace FantasyFootball.Logic.Interfaces
{
    public interface IDraftStrategyLogic
    {
        Averages GetAverages(List<Player> players, Rules rules);
        List<Player> GetRelativePoints(List<Player> players, Averages averages);
        List<Player> GetStrongRoster(List<Player> players, Rules rules);
        List<Player> GetCurrentRoster(List<Player> players, int qb, int rb1, int rb2, int wr1, int wr2, int te, int flex, int def, int k);
        List<Player> GetStrongerRoster(List<Player> currentPlayers, List<Player> allPlayers, int cap, int bench);
        CostAnalysis GetCostAnalysis(List<Player> players);
        List<Player> GetExpectedValue(List<Player> players, CostAnalysis analysis);
        string GetSpreadsheet(List<Player> players);


    }
}
