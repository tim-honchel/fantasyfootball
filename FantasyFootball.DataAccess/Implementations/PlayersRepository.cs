using FantasyFootball.DataAccess.Interfaces;
using FantasyFootball.DataAccess.Models;

namespace FantasyFootball.DataAccess.Implementations
{
    internal class PlayersRepository : IPlayersRepository
    {
        public Task<List<Player>> RequestProjections(int ESPNLeagueID, string projectionsCookie)
        {
            throw new NotImplementedException();
        }


    }
}
