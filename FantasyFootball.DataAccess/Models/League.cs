namespace FantasyFootball.DataAccess.Models
{
    public class League
    {
        public int LeagueID { get; set; }
        public string LeagueName { get; set; } = string.Empty;
        public int ESPNID { get; set; }
        public string RulesCookie { get; set; } = string.Empty;
        public string ProjectionsCookie { get; set; } = string.Empty; 

    }
}
