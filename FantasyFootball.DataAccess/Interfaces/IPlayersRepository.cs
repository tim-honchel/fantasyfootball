using FantasyFootball.DataAccess.Models;

namespace FantasyFootball.DataAccess.Interfaces
{
    public interface IPlayersRepository
    {
        public Task<List<Player>> RequestProjections(int ESPNLeagueID, string projectionsCookie);
    }
}
