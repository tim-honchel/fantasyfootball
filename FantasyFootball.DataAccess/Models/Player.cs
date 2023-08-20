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

        public double QB1 { get; set; } = 0;
        public double RB1 { get; set; } = 0;
        public double RB2 { get; set; } = 0;
        public double WR1 { get; set; } = 0;
        public double WR2 { get; set; } = 0;
        public double TE1 { get; set; } = 0;
        public double  FLEX { get; set; } = 0;
        public double DEF { get; set; } = 0;
        public double K { get; set; } = 0;
        public double FA { get; set; } = 0;
        public double ExpectedValueLow { get; set; } = 0;
        public double ExpectedValue { get; set; } = 0;
        public double ExpectedValueHigh { get; set; } = 0;
        public double PercentOfTopRosters { get; set; } = 0;
        public List<string> Tags { get; set; } = new List<string>();



    }
}
