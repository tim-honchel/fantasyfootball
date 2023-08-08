

using FantasyFootball.DataAccess.Implementations;
using FantasyFootball.DataAccess.Models;
using FantasyFootball.Logic.Interfaces;
using System.Reflection.Emit;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace FantasyFootball.Logic.Implementations
{
    public class DraftStrategyLogic : IDraftStrategyLogic
    {
        public Averages GetAverages(List<Player> players, Rules rules)
        {
            Averages averages = new Averages();
            averages.QB = CalculateAverage(players.Where(x => x.Position == "QB").OrderByDescending(x=>x.WeeklyPoints).ToList(), rules.Teams, 0);
            averages.RB1 = CalculateAverage(players.Where(x => x.Position == "RB").OrderByDescending(x => x.WeeklyPoints).ToList(), rules.Teams, 0);
            averages.RB2 = CalculateAverage(players.Where(x => x.Position == "RB").OrderByDescending(x => x.WeeklyPoints).ToList(), rules.Teams, rules.Teams);
            averages.WR1 = CalculateAverage(players.Where(x => x.Position == "WR").OrderByDescending(x => x.WeeklyPoints).ToList(), rules.Teams, 0);
            averages.WR2 = CalculateAverage(players.Where(x => x.Position == "WR").OrderByDescending(x => x.WeeklyPoints).ToList(), rules.Teams, rules.Teams);
            averages.TE = CalculateAverage(players.Where(x => x.Position == "TE").OrderByDescending(x => x.WeeklyPoints).ToList(), rules.Teams, 0);
            averages.DEF = CalculateAverage(players.Where(x => x.Position == "DEF").OrderByDescending(x => x.WeeklyPoints).ToList(), rules.Teams, 0);
            averages.K = CalculateAverage(players.Where(x => x.Position == "K").OrderByDescending(x => x.WeeklyPoints).ToList(), rules.Teams, 0);

            double flexRB = players.Where(x => x.Position == "RB").OrderByDescending(x => x.WeeklyPoints).ToList()[rules.Teams * 2].WeeklyPoints;
            double flexWR = players.Where(x => x.Position == "WR").OrderByDescending(x => x.WeeklyPoints).ToList()[rules.Teams * 2].WeeklyPoints;

            List<Player> flex = players.Where(x => (x.Position == "RB" && x.WeeklyPoints <= flexRB) || (x.Position == "WR" && x.WeeklyPoints <= flexWR)).OrderByDescending(x => x.WeeklyPoints).ToList();
            averages.FLEX = CalculateAverage(flex, rules.Teams, 0);

            averages.total = Math.Round(averages.QB + averages.RB1 + averages.RB2 + averages.WR1 + averages.WR2 + averages.TE + averages.FLEX + averages.K + averages.DEF);

            averages.FAQB = players.Where(x=>x.Position == "QB").OrderByDescending(x => x.WeeklyPoints).ToList()[(int)(rules.Teams * (rules.QB + rules.Bench * 0.1))].WeeklyPoints;
            averages.FARB = players.Where(x => x.Position == "RB").OrderByDescending(x => x.WeeklyPoints).ToList()[(int)(rules.Teams * (rules.RB + rules.FLEX * 0.6 + rules.Bench * 0.5))].WeeklyPoints;
            averages.FAWR = players.Where(x => x.Position == "WR").OrderByDescending(x => x.WeeklyPoints).ToList()[(int)(rules.Teams * (rules.WR + +rules.FLEX * 0.4 + rules.Bench * 0.3))].WeeklyPoints;
            averages.FATE = players.Where(x => x.Position == "TE").OrderByDescending(x => x.WeeklyPoints).ToList()[(int)(rules.Teams * (rules.TE + rules.Bench * 0.1))].WeeklyPoints;
            averages.FADEF = players.Where(x => x.Position == "DEF").OrderByDescending(x => x.WeeklyPoints).ToList()[rules.Teams].WeeklyPoints;
            averages.FAK = players.Where(x => x.Position == "K").OrderByDescending(x => x.WeeklyPoints).ToList()[rules.Teams].WeeklyPoints;

            return averages;
        }

        public double CalculateAverage(List<Player> players, int teams, int exclude)
        {
            double average;
            if (teams % 2 == 0)
            {
                average = (players[teams / 2 - 1 + exclude].WeeklyPoints + players[teams / 2 + exclude].WeeklyPoints) / 2;
            }
            else
            {
                average = players[(int)(teams / 2 - 1 + exclude)].WeeklyPoints;
            }
            return Math.Round(average, 2);
        }


        public List<Player> GetRelativePoints(List<Player> players, Averages averages)
        {
            foreach (Player player in players)
            {
                if (player.Position == "QB")
                {
                    player.QB1 = Math.Round(player.WeeklyPoints - averages.QB,2);
                    player.FA = Math.Round(player.WeeklyPoints - averages.FAQB, 2);
                }
                else if (player.Position == "RB")
                {
                    player.RB1 = Math.Round(player.WeeklyPoints - averages.RB1,2);
                    player.RB2 = Math.Round(player.WeeklyPoints - averages.RB2,2);
                    player.FLEX = Math.Round(player.WeeklyPoints - averages.FLEX,2);
                    player.FA = Math.Round(player.WeeklyPoints - averages.FARB, 2);
                }
                else if (player.Position == "WR")
                {
                    player.WR1 = Math.Round(player.WeeklyPoints - averages.WR1,2);
                    player.WR2 = Math.Round(player.WeeklyPoints - averages.WR2,2);
                    player.FLEX = Math.Round(player.WeeklyPoints - averages.FLEX,2);
                    player.FA = Math.Round(player.WeeklyPoints - averages.FAWR, 2);
                }
                else if (player.Position == "TE")
                {
                    player.TE1 = Math.Round(player.WeeklyPoints - averages.TE,2);
                    player.FA = Math.Round(player.WeeklyPoints - averages.FATE, 2);
                }
                else if (player.Position == "DEF")
                {
                    player.DEF = Math.Round(player.WeeklyPoints - averages.DEF,2);
                    player.FA = Math.Round(player.WeeklyPoints - averages.FADEF, 2);
                }
                else if (player.Position == "K")
                {
                    player.K = Math.Round(player.WeeklyPoints - averages.K,2);
                    player.FA = Math.Round(player.WeeklyPoints - averages.FAK, 2);
                }
                
            }
            return players;
        }
        public List<Player> GetSalaryRank(List<Player> players, Averages averages)
        {
            List<Player> qb = RankByPosition(players.Where(x => x.Position == "QB").OrderByDescending(x => x.WeeklyPoints).ToList(), "QB", averages.QB, false);
            List<Player> rb1 = RankByPosition(players.Where(x => x.Position == "RB").OrderByDescending(x => x.WeeklyPoints).ToList(), "RB1", averages.RB1, false);
            List<Player> rb2 = RankByPosition(players.Where(x => x.Position == "RB").OrderByDescending(x => x.WeeklyPoints).ToList(), "RB2", averages.RB2, true);
            List<Player> wr1 = RankByPosition(players.Where(x => x.Position == "WR").OrderByDescending(x => x.WeeklyPoints).ToList(), "WR1", averages.WR1, false);
            List<Player> wr2 = RankByPosition(players.Where(x => x.Position == "WR").OrderByDescending(x => x.WeeklyPoints).ToList(), "WR2", averages.WR2, true);
            List<Player> te = RankByPosition(players.Where(x => x.Position == "TE").OrderByDescending(x => x.WeeklyPoints).ToList(), "TE", averages.TE, false);
            List<Player> flex = RankByPosition(players.Where(x => x.Position == "RB" || x.Position == "WR").OrderByDescending(x => x.WeeklyPoints).ToList(), "FLEX", averages.FLEX, true);
            List<Player> def = RankByPosition(players.Where(x => x.Position == "DEF").OrderByDescending(x => x.WeeklyPoints).ToList(), "DEF", averages.DEF, false);
            List<Player> k = RankByPosition(players.Where(x => x.Position == "K").OrderByDescending(x => x.WeeklyPoints).ToList(), "K", averages.K, false);
            List<Player> sortedPlayers = qb.Concat(rb1).Concat(rb2).Concat(wr1).Concat(wr2).Concat(te).Concat(flex).Concat(def).Concat(k).ToList();
            return sortedPlayers;
        }

        public List<Player> RankByPosition(List<Player> players, string position, double average, bool inclusive)
        {
            List<Player> playerChart = new List<Player>();
            int salary = players[0].Cost + 1;
            foreach (Player player in players)
            {
                if ((inclusive == true && player.Cost <= salary && player.WeeklyPoints - average > -4) || (inclusive == false && player.Cost < salary && player.WeeklyPoints - average > -4))
                {
                    Player playerToAdd = new()
                    {
                        PlayerID = player.PlayerID,
                        LastName = player.LastName,
                        FirstInitial = player.FirstInitial,
                        Team = player.Team,
                        Position = position,
                        WeeklyPoints = Math.Round(player.WeeklyPoints - average, 2),
                        Cost = player.Cost
                    };
                    playerChart.Add(playerToAdd);
                    salary = player.Cost;
                }
            }    

            return playerChart;
        }

        public List<Player> GetStrongRoster(List<Player> players, Rules rules)
        {
            List<Player> qb = players.Where(x => x.Position == "QB").OrderByDescending(x => x.WeeklyPoints).ToList();
            List<Player> rb1 = players.Where(x => x.Position == "RB1").OrderByDescending(x => x.WeeklyPoints).ToList();
            List<Player> rb2 = players.Where(x => x.Position == "RB2").OrderByDescending(x => x.WeeklyPoints).ToList();
            List<Player> wr1 = players.Where(x => x.Position == "WR1").OrderByDescending(x => x.WeeklyPoints).ToList();
            List<Player> wr2 = players.Where(x => x.Position == "WR2").OrderByDescending(x => x.WeeklyPoints).ToList();
            List<Player> te = players.Where(x => x.Position == "TE").OrderByDescending(x => x.WeeklyPoints).ToList();
            List<Player> flex = players.Where(x => x.Position == "FLEX").OrderByDescending(x => x.WeeklyPoints).ToList();
            List<Player> def = players.Where(x => x.Position == "DEF").OrderByDescending(x => x.WeeklyPoints).ToList();
            List<Player> k = players.Where(x => x.Position == "K").OrderByDescending(x => x.WeeklyPoints).ToList();

            int salary = qb[0].Cost + rb1[0].Cost + rb2[0].Cost + wr1[0].Cost + wr1[0].Cost + te[0].Cost + flex[0].Cost + def[0].Cost + k[0].Cost;
            Dictionary<string, double> slope = new()
            {
                {"QB", 0 },
                {"RB1", 0 },
                {"RB2", 0 },
                {"WR1", 0 },
                {"WR2", 0 },
                {"TE", 0 },
                {"FLEX", 0 },
                {"DEF", 0 },
                {"K", 0 }
            };
            string maxSlope = "";

            while ((salary > rules.SalaryCap - rules.Bench && qb.Count > 1 && rb1.Count > 1 && rb2.Count > 1 && wr1.Count > 1 && wr2.Count > 1 && te.Count > 1 && flex.Count > 1 && def.Count > 1 && k.Count > 1) || (rb1[0].PlayerID == rb2[0].PlayerID || wr1[0].PlayerID == wr2[0].PlayerID || flex[0].PlayerID == rb1[0].PlayerID || flex[0].PlayerID == rb2[0].PlayerID || flex[0].PlayerID == wr1[0].PlayerID || flex[0].PlayerID == wr2[0].PlayerID))
            {
                    var remove = flex.SingleOrDefault(x => x.PlayerID == rb1[0].PlayerID);
                    if (remove != null && flex.Count>1) { flex.Remove(remove); }
                    remove = flex.SingleOrDefault(x => x.PlayerID == rb2[0].PlayerID);
                    if (remove != null && flex.Count > 1) { flex.Remove(remove); }
                    remove = flex.SingleOrDefault(x => x.PlayerID == wr1[0].PlayerID);
                    if (remove != null && flex.Count > 1) { flex.Remove(remove); }
                    remove = flex.SingleOrDefault(x => x.PlayerID == wr2[0].PlayerID);
                    if (remove != null && flex.Count > 1) { flex.Remove(remove); }
                    remove = rb2.SingleOrDefault(x => x.PlayerID == rb1[0].PlayerID);
                    if (remove != null && rb2.Count > 1) { rb2.Remove(remove); }
                    remove = wr2.SingleOrDefault(x => x.PlayerID == wr1[0].PlayerID);
                    if (remove != null && wr2.Count > 1) { wr2.Remove(remove); }
                if (salary > rules.SalaryCap - rules.Bench)
                {
                    slope["QB"] = qb.Count > 1 ? (qb[0].Cost - qb[1].Cost) / (qb[0].WeeklyPoints - qb[1].WeeklyPoints) : 0;
                    slope["RB1"] = rb1.Count > 1 ? (rb1[0].Cost - rb1[1].Cost) / (rb1[0].WeeklyPoints - rb1[1].WeeklyPoints) : 0;
                    slope["RB2"] = rb2.Count > 1 ? (rb2[0].Cost - rb2[1].Cost) / (rb2[0].WeeklyPoints - rb2[1].WeeklyPoints) : 0;
                    slope["WR1"] = wr1.Count > 1 ? (wr1[0].Cost - wr1[1].Cost) / (wr1[0].WeeklyPoints - wr1[1].WeeklyPoints) : 0;
                    slope["WR2"] = wr2.Count > 1 ? (wr2[0].Cost - wr2[1].Cost) / (wr2[0].WeeklyPoints - wr2[1].WeeklyPoints) : 0;
                    slope["TE"] = te.Count > 1 ? (te[0].Cost - te[1].Cost) / (te[0].WeeklyPoints - te[1].WeeklyPoints) : 0;
                    slope["FLEX"] = flex.Count > 1 ? (flex[0].Cost - flex[1].Cost) / (flex[0].WeeklyPoints - flex[1].WeeklyPoints) : 0;
                    slope["DEF"] = def.Count > 1 ? (def[0].Cost - def[1].Cost) / (def[0].WeeklyPoints - def[1].WeeklyPoints) : 0;
                    slope["K"] = k.Count > 1 ? (k[0].Cost - k[1].Cost) / (k[0].WeeklyPoints - k[1].WeeklyPoints) : 0;
                    maxSlope = slope.MaxBy(x => x.Value).Key;

                    if (maxSlope == "QB")
                    {
                        qb.RemoveAt(0);
                    }
                    else if (maxSlope == "RB1")
                    {
                        rb1.RemoveAt(0);
                    }
                    else if (maxSlope == "RB2")
                    {
                        rb2.RemoveAt(0);
                    }
                    else if (maxSlope == "WR1")
                    {
                        wr1.RemoveAt(0);
                    }
                    else if (maxSlope == "WR2")
                    {
                        wr2.RemoveAt(0);
                    }
                    else if (maxSlope == "TE")
                    {
                        te.RemoveAt(0);
                    }
                    else if (maxSlope == "FLEX")
                    {
                        flex.RemoveAt(0);
                    }
                    else if (maxSlope == "DEF")
                    {
                        def.RemoveAt(0);
                    }
                    else if (maxSlope == "K")
                    {
                        k.RemoveAt(0);
                    }
                }
                salary = qb[0].Cost + rb1[0].Cost + rb2[0].Cost + wr1[0].Cost + wr1[0].Cost + te[0].Cost + flex[0].Cost + def[0].Cost + k[0].Cost;
            }
            List<Player> topPlayers = new()
            {
                qb[0], rb1[0], rb2[0], wr1[0], wr2[0], te[0], flex[0], def[0], k[0]
            };

            return topPlayers;
        }

        public List<Player> GetCurrentRoster(List<Player> players, int qb, int rb1, int rb2, int wr1, int wr2, int te, int flex, int def, int k)
        {
            List<Player> currentPlayers = new();

            Player? QB = players.SingleOrDefault(x => x.PlayerID == qb);
            if (QB != null)
            {
                currentPlayers.Add(QB);
                players.Remove(QB);
            }

            Player? RB1 = players.SingleOrDefault(x => x.PlayerID == rb1);
            if (RB1 != null)
            {
                RB1.Position = "RB1";
                currentPlayers.Add(RB1);
                players.Remove(RB1);
            }

            Player? RB2 = players.SingleOrDefault(x => x.PlayerID == rb2);
            if (RB2 != null)
            {
                RB2.Position = "RB2";
                currentPlayers.Add(RB2);
                players.Remove(RB2);
            }

            Player? WR1 = players.SingleOrDefault(x => x.PlayerID == wr1);
            if (WR1 != null)
            {
                WR1.Position = "WR1";
                currentPlayers.Add(WR1);
                players.Remove(WR1);
            }

            Player? WR2 = players.SingleOrDefault(x => x.PlayerID == wr2);
            if (WR2 != null)
            {
                WR2.Position = "WR2";
                currentPlayers.Add(WR2);
                players.Remove(WR2);
            }

            Player? TE = players.SingleOrDefault(x => x.PlayerID == te);
            if (TE != null)
            {
                currentPlayers.Add(TE);
                players.Remove(TE);
            }

            Player? FLEX = players.SingleOrDefault(x => x.PlayerID == flex);
            if (FLEX != null)
            {
                FLEX.Position = "FLEX";
                currentPlayers.Add(FLEX);
                players.Remove(FLEX);
            }

            Player? DEF = players.SingleOrDefault(x => x.PlayerID == def);
            if (DEF != null)
            {
                currentPlayers.Add(DEF);
                players.Remove(DEF);
            }

            Player? K = players.SingleOrDefault(x => x.PlayerID == k);
            if (K != null)
            {
                currentPlayers.Add(K);
                players.Remove(K);
            }

            return currentPlayers;
        }
        public List<Player> GetStrongerRoster(List<Player> currentPlayers, List<Player> allPlayers, int cap, int bench)
        {
            int capSpace = cap - bench - currentPlayers.Sum(x => x.Cost);
            if (capSpace == 0) { return currentPlayers; }

            List<Player> qbs = allPlayers.Where(x => x.Position == "QB" && x.WeeklyPoints > currentPlayers[0].WeeklyPoints && x.Cost - currentPlayers[0].Cost <= capSpace).OrderByDescending(x=>x.WeeklyPoints).ToList();
            List<Player> rb1s = allPlayers.Where(x => x.Position == "RB" && x.WeeklyPoints > currentPlayers[1].WeeklyPoints && x.Cost - currentPlayers[1].Cost <= capSpace).OrderByDescending(x => x.WeeklyPoints).ToList();
            List<Player> rb2s = allPlayers.Where(x => x.Position == "RB" && x.WeeklyPoints > currentPlayers[2].WeeklyPoints && x.Cost - currentPlayers[2].Cost <= capSpace).OrderByDescending(x => x.WeeklyPoints).ToList();
            List<Player> wr1s = allPlayers.Where(x => x.Position == "WR" && x.WeeklyPoints > currentPlayers[3].WeeklyPoints && x.Cost - currentPlayers[3].Cost <= capSpace).OrderByDescending(x => x.WeeklyPoints).ToList();
            List<Player> wr2s = allPlayers.Where(x => x.Position == "WR" && x.WeeklyPoints > currentPlayers[4].WeeklyPoints && x.Cost - currentPlayers[4].Cost <= capSpace).OrderByDescending(x => x.WeeklyPoints).ToList();
            List<Player> tes = allPlayers.Where(x => x.Position == "TE" && x.WeeklyPoints > currentPlayers[5].WeeklyPoints && x.Cost - currentPlayers[5].Cost <= capSpace).OrderByDescending(x => x.WeeklyPoints).ToList();
            List<Player> flexs = allPlayers.Where(x => (x.Position == "RB" || x.Position == "WR") && x.WeeklyPoints > currentPlayers[6].WeeklyPoints && x.Cost - currentPlayers[6].Cost <= capSpace).OrderByDescending(x => x.WeeklyPoints).ToList();
            List<Player> defs = allPlayers.Where(x => x.Position == "DEF" && x.WeeklyPoints > currentPlayers[7].WeeklyPoints && x.Cost - currentPlayers[7].Cost <= capSpace).OrderByDescending(x => x.WeeklyPoints).ToList();
            List<Player> ks = allPlayers.Where(x => x.Position == "K" && x.WeeklyPoints > currentPlayers[8].WeeklyPoints && x.Cost - currentPlayers[8].Cost <= capSpace).OrderByDescending(x => x.WeeklyPoints).ToList();

            Dictionary<string, double> slope = new()
            {
                {"QB", 0 },
                {"RB1", 0 },
                {"RB2", 0 },
                {"WR1", 0 },
                {"WR2", 0 },
                {"TE", 0 },
                {"FLEX", 0 },
                {"DEF", 0 },
                {"K", 0 }
            };
            
            slope["QB"] = qbs.Count > 0 ? (qbs[0].WeeklyPoints - currentPlayers[0].WeeklyPoints) : 0;
            slope["RB1"] = rb1s.Count > 0 ? (rb1s[0].WeeklyPoints - currentPlayers[1].WeeklyPoints) : 0;
            slope["RB2"] = rb2s.Count > 0 ? (rb2s[0].WeeklyPoints - currentPlayers[2].WeeklyPoints) : 0;
            slope["WR1"] = wr1s.Count > 0 ? (wr1s[0].WeeklyPoints - currentPlayers[3].WeeklyPoints) : 0;
            slope["WR2"] = wr2s.Count > 0 ? (wr2s[0].WeeklyPoints - currentPlayers[4].WeeklyPoints) : 0;
            slope["TE"] = tes.Count > 0 ? (tes[0].WeeklyPoints - currentPlayers[5].WeeklyPoints) : 0;
            slope["FLEX"] = flexs.Count > 0 ? (flexs[0].WeeklyPoints - currentPlayers[6].WeeklyPoints) : 0;
            slope["DEF"] = defs.Count > 0 ? (defs[0].WeeklyPoints - currentPlayers[7].WeeklyPoints) : 0;
            slope["K"] = ks.Count > 0 ? (ks[0].WeeklyPoints - currentPlayers[8].WeeklyPoints) : 0;
            string maxSlope = slope.MaxBy(x => x.Value).Key;

            if (maxSlope == "QB")
            {
                currentPlayers.RemoveAt(0);
                currentPlayers.Add(qbs[0]);
            }
            else if (maxSlope == "RB1")
            {
                currentPlayers.RemoveAt(1);
                rb1s[0].Position = "RB1";
                currentPlayers.Add(rb1s[0]);
            }
            else if (maxSlope == "RB2")
            {
                currentPlayers.RemoveAt(2);
                rb2s[0].Position = "RB2";
                currentPlayers.Add(rb2s[0]);
            }
            else if (maxSlope == "WR1")
            {
                currentPlayers.RemoveAt(3);
                wr1s[0].Position = "WR1"; ;
                currentPlayers.Add(wr1s[0]);
            }
            else if (maxSlope == "WR2")
            {
                currentPlayers.RemoveAt(4);
                wr2s[0].Position = "WR2";
                currentPlayers.Add(wr2s[0]);
            }
            else if (maxSlope == "TE")
            {
                currentPlayers.RemoveAt(5);
                currentPlayers.Add(tes[0]);
            }
            else if (maxSlope == "FLEX")
            {
                currentPlayers.RemoveAt(6);
                if (flexs[0].Position == "RB" && flexs[0].WeeklyPoints > currentPlayers[1].WeeklyPoints)
                {
                    flexs[0].Position = "RB1";
                    currentPlayers[1].Position = "RB2";
                    currentPlayers[2].Position = "FLEX";
                }
                else if (flexs[0].Position == "RB" && flexs[0].WeeklyPoints > currentPlayers[2].WeeklyPoints)
                {
                    flexs[0].Position = "RB2";
                    currentPlayers[2].Position = "FLEX";
                }
                else if (flexs[0].Position == "WR" && flexs[0].WeeklyPoints > currentPlayers[3].WeeklyPoints)
                {
                    flexs[0].Position = "WR1";
                    currentPlayers[3].Position = "WR2";
                    currentPlayers[4].Position = "FLEX";
                }
                else if (flexs[0].Position == "WR" && flexs[0].WeeklyPoints > currentPlayers[4].WeeklyPoints)
                {
                    flexs[0].Position = "WR2";
                    currentPlayers[4].Position = "FLEX";
                }
                else
                {
                    flexs[0].Position = "FLEX";
                }
                currentPlayers.Add(flexs[0]);
            }
            else if (maxSlope == "DEF")
            {
                currentPlayers.RemoveAt(7);
                currentPlayers.Add(defs[0]);
            }
            else if (maxSlope == "K")
            {
                currentPlayers.RemoveAt(8);
                currentPlayers.Add(ks[0]);
            }

            return currentPlayers;
        }

        public CostAnalysis GetCostAnalysis(List<Player> players)
        {
            CostAnalysis analysis = new CostAnalysis();

            List<Player> qbs = players.Where(x => x.Position == "QB").ToList();
            List<Player> rbs = players.Where(x => x.Position == "RB").ToList();
            List<Player> wrs = players.Where(x => x.Position == "WR").ToList();
            List<Player> tes = players.Where(x => x.Position == "TE").ToList();

            analysis = CalculateSlopeAndIntercept(analysis, "QB", qbs);
            analysis = CalculateSlopeAndIntercept(analysis, "RB", rbs);
            analysis = CalculateSlopeAndIntercept(analysis, "WR", wrs);
            analysis = CalculateSlopeAndIntercept(analysis, "TE", tes);

            return analysis;
        }

        public CostAnalysis CalculateSlopeAndIntercept(CostAnalysis analysis, string position, List<Player> players)
        {
            double sumPoints = players.Sum(x => x.FA);
            double sumPrice = players.Sum(x => x.Cost);

            double sumXSquared = players.Sum(x => x.FA * x.FA);
            double sumXY = players.Sum(x => x.FA * x.Cost);

            double slope = Math.Round((players.Count() * sumXY - sumPoints * sumPrice) / (players.Count() * sumXSquared - sumPoints * sumPoints),2);
            double intercept = Math.Round((sumPrice - slope * sumPoints)/players.Count(),2);
            double residual = Math.Round(players.Sum(x => Math.Abs(x.Cost - slope * x.FA + intercept))/players.Count(),2);

            Player topPlayer = players.OrderByDescending(x => x.Cost).FirstOrDefault();
            Player bottomPlayer = players.Where(x => x.Cost > 0).OrderByDescending(x => x.Cost).LastOrDefault();
            double margin = residual / topPlayer.Cost;
            double topDifference = topPlayer.Cost - (topPlayer.FA * slope + intercept);
            double bottomDifference = bottomPlayer.Cost - (bottomPlayer.FA * slope + intercept);

            residual = residual * .5;
            
            if (position == "QB")
            {
                analysis.QBSlope = slope;
                analysis.QBIntercept = intercept;
                analysis.QBResidual = residual;
            }
            else if (position == "RB")
            {
                analysis.RBSlope = slope;
                analysis.RBIntercept = intercept;
                analysis.RBResidual = residual;
            }
            else if (position == "WR")
            {
                analysis.WRSlope = slope;
                analysis.WRIntercept = intercept;
                analysis.WRResidual = residual;
            }
            else if (position == "TE")
            {
                analysis.TESlope = slope;
                analysis.TEIntercept = intercept;
                analysis.TEResidual = residual;
            }
            if (margin >= 0.25 && topDifference > 0 && bottomDifference < 0 && position == "RB")
            {
                analysis.RB1EffectUp = Math.Round(topDifference / ((topPlayer.RB1 + topPlayer.RB2)/2),2);
                analysis.RB1EffectDown = Math.Round(bottomDifference / ((bottomPlayer.RB1+bottomPlayer.RB2)/2),2);
            }


            return analysis;
        }

        public List<Player> GetExpectedValue(List<Player> players, CostAnalysis analysis)
        {
            foreach (Player player in players)
            {
                if (player.Position == "QB")
                {
                    player.ExpectedValue = player.FA * analysis.QBSlope + analysis.QBIntercept;
                    player.ExpectedValueHigh = player.ExpectedValue + analysis.QBResidual;
                    player.ExpectedValueLow = player.ExpectedValue - analysis.QBResidual;
                }
                else if (player.Position == "RB")
                {
                    player.ExpectedValue = player.FA * analysis.RBSlope + analysis.RBIntercept;
                    player.ExpectedValue = player.RB1 + player.RB2 > 0 ? player.ExpectedValue + (player.RB1+player.RB2)/2 * analysis.RB1EffectUp : player.ExpectedValue + (player.RB1+player.RB2)/2 * analysis.RB1EffectDown;
                    player.ExpectedValueHigh = player.ExpectedValue + analysis.RBResidual;
                    player.ExpectedValueLow = player.ExpectedValue - analysis.RBResidual;
                }
                else if (player.Position == "WR")
                {
                    player.ExpectedValue = player.FA * analysis.WRSlope + analysis.WRIntercept;
                    player.ExpectedValueHigh = player.ExpectedValue + analysis.WRResidual;
                    player.ExpectedValueLow = player.ExpectedValue - analysis.WRResidual;
                }
                if (player.Position == "TE")
                {
                    player.ExpectedValue = player.FA * analysis.TESlope + analysis.TEIntercept;
                    player.ExpectedValueHigh = player.ExpectedValue + analysis.TEResidual;
                    player.ExpectedValueLow = player.ExpectedValue - analysis.TEResidual;
                }
                else if (player.Position == "K" || player.Position == "DEF")
                {
                    player.ExpectedValue = player.Cost;
                    player.ExpectedValueHigh = 1;
                    player.ExpectedValueLow = 0;
                }
                player.ExpectedValue = Math.Round(player.ExpectedValue, 0);
                player.ExpectedValueHigh = Math.Round(player.ExpectedValueHigh, 0);
                player.ExpectedValueLow = Math.Round(player.ExpectedValueLow, 0);
                if (player.ExpectedValue < 0) { player.ExpectedValue = 0; }
                if (player.ExpectedValueHigh < 0) { player.ExpectedValueHigh = 0; }
                if (player.ExpectedValueLow < 0) { player.ExpectedValueLow = 0; }

            }
            return players;
        }

        public string GetSpreadsheet(List<Player> players)
        {
            StringBuilder csv = new StringBuilder();

            List<Player> qb = players.Where(x => x.Position == "QB").OrderByDescending(x => x.WeeklyPoints).ToList();
            csv.AppendLine("QB, Team, Avg Cost, Lo|Med|Hi, Proj PPW, +QB1, +FA");
            foreach (Player player in qb)
            {
                csv.AppendLine($"{player.FirstInitial} {player.LastName},{player.Team},{player.Cost},{player.ExpectedValueLow}|{player.ExpectedValue}|{player.ExpectedValueHigh},{player.WeeklyPoints},{player.QB1},{player.FA}");
            }
            csv.AppendLine();
            
            List<Player> rb = players.Where(x => x.Position == "RB").OrderByDescending(x => x.WeeklyPoints).ToList();
            csv.AppendLine("RB, Team, Avg Cost, Lo|Med|Hi, Proj PPW, +RB1, +RB2, +FLEX, +FA");
            foreach (Player player in rb)
            {
                csv.AppendLine($"{player.FirstInitial} {player.LastName},{player.Team},{player.Cost},{player.ExpectedValueLow}|{player.ExpectedValue}|{player.ExpectedValueHigh},{player.WeeklyPoints},{player.RB1}, {player.RB2}, {player.FLEX}, {player.FA}");
            }
            csv.AppendLine();

            List<Player> wr = players.Where(x => x.Position == "WR").OrderByDescending(x => x.WeeklyPoints).ToList();
            csv.AppendLine("WR, Team, Avg Cost, Lo|Med|Hi, Proj PPW, +WR1, +WR2, +FLEX, +FA");
            foreach (Player player in wr)
            {
                csv.AppendLine($"{player.FirstInitial} {player.LastName},{player.Team},{player.Cost},{player.ExpectedValueLow}|{player.ExpectedValue}|{player.ExpectedValueHigh},{player.WeeklyPoints},{player.WR1}, {player.WR2}, {player.FLEX}, {player.FA}");
            }
            csv.AppendLine();

            List<Player> te = players.Where(x => x.Position == "TE").OrderByDescending(x => x.WeeklyPoints).ToList();
            csv.AppendLine("TE, Team, Avg Cost, Lo|Med|Hi, Proj PPW, +TE1, +FA");
            foreach (Player player in te)
            {
                csv.AppendLine($"{player.FirstInitial} {player.LastName},{player.Team},{player.Cost},{player.ExpectedValueLow}|{player.ExpectedValue}|{player.ExpectedValueHigh},{player.WeeklyPoints},{player.TE1},{player.FA}");
            }
            csv.AppendLine();

            List<Player> def = players.Where(x => x.Position == "DEF").OrderByDescending(x => x.WeeklyPoints).ToList();
            csv.AppendLine("DEF, Team, Avg Cost, Lo|Med|Hi, Proj PPW, +DEF1, +FA");
            foreach (Player player in def)
            {
                csv.AppendLine($"{player.FirstInitial} {player.LastName},{player.Team},{player.Cost},{player.ExpectedValueLow}|{player.ExpectedValue}|{player.ExpectedValueHigh},{player.WeeklyPoints},{player.DEF},{player.FA}");
            }
            csv.AppendLine();

            List<Player> k = players.Where(x => x.Position == "K").OrderByDescending(x => x.WeeklyPoints).ToList();
            csv.AppendLine("K, Team, Avg Cost, Lo|Med|Hi, Proj PPW, +K1, +FA");
            foreach (Player player in k)
            {
                csv.AppendLine($"{player.FirstInitial} {player.LastName},{player.Team},{player.Cost},{player.ExpectedValueLow}|{player.ExpectedValue}|{player.ExpectedValueHigh},{player.WeeklyPoints},{player.K},{player.FA}");
            }

            return csv.ToString();
        }


    }
}
