namespace FantasyFootball.DataAccess.Models
{
    public class Rules
    {
        public int RulesID { get; set; }
        public int Teams { get; set; }
        public int PlayoffTeams { get; set; }
        public int SalaryCap { get; set;}
        public int QB { get; set; }
        public int RB { get; set; }
        public int WR { get; set; }
        public int TE { get; set; }
        public int FLEX { get; set; }
        public int DEF { get; set; }
        public int K { get; set; }
        public int Bench { get; set; }
        public bool OtherPositions { get; set; } = false;
        public bool Keepers { get; set; } = false;
        public bool AuctionDraft { get; set; } = false;
        public int LeagueID { get; set; }

    }
}
