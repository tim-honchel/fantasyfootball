namespace FantasyFootball.DataAccess.Models
{
    public class Player
    {
        public int PlayerID { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FirstInitial { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Team { get; set; } = string.Empty;
        public int Cost { get; set; }
        public double WeeklyPoints { get; set; }
        public int LeagueID { get; set; }

    }
}
