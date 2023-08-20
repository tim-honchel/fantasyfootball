namespace FantasyFootball.DataAccess.Models
{
    public class PercentRequest
    {
        public Dictionary<int, int> CountByID { get; set; }
        public List<Player> Players { get; set; }
    }
}
