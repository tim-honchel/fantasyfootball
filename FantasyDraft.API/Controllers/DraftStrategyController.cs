using FantasyFootball.DataAccess.Models;
using FantasyFootball.Logic.Implementations;
using FantasyFootball.Logic.Interfaces;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Http.Headers;
using System.Text;

namespace FantasyDraft.API.Controllers
{
    [Controller]
    [Route("draftstrategy")]
    public class DraftStrategyController : Controller
    {
        public IDraftStrategyLogic _draftStrategyLogic;

        public DraftStrategyController(IDraftStrategyLogic draftStrategyLogic)
        {
            _draftStrategyLogic = draftStrategyLogic;
        }

        [HttpPost("addOtherTags")]
        public async Task<IActionResult> AddNominationPlayerTags([FromBody] List<Player> players)
        {
            foreach (Player player in players)
            {
                if (player.Tags.Count == 0 && player.PercentOfTopRosters > 10)
                {
                    player.Tags.Add("Solid");
                }
                else if (player.PercentOfTopRosters < 1 && player.Cost > 5)
                {
                    player.Tags.Add("Nomination");
                }
            }
            return new OkObjectResult(players);
        }

        [HttpPost("addTopTags")]
        public async Task<IActionResult> AddTopPlayerTags([FromBody] List<Player> players, [FromQuery] int qb, [FromQuery] int rb1, [FromQuery] int rb2, [FromQuery] int wr1, [FromQuery] int wr2, [FromQuery] int te, [FromQuery] int flex, [FromQuery] int def, [FromQuery] int k)
        {
            players = _draftStrategyLogic.AddTopTags(players, qb, rb1, rb2, wr1, wr2, te, flex, def, k);
            return new OkObjectResult(players);
        }

        [HttpPost("addTopRosterPercent")]
        public async Task<IActionResult> AddTopRosterPercent([FromBody] PercentRequest request)
        {
            List<Player> players = request.Players;
            Dictionary<int,int> count = request.CountByID;
            int total = count[0];
            int x = 0;
            foreach (Player player in players)
            {
                if (count.ContainsKey(player.PlayerID))
                {
                    x = count[player.PlayerID];
                    player.PercentOfTopRosters = Math.Round(100* (double)x / (double)total,1);
                }
            }
            return new OkObjectResult(players.OrderByDescending(x => x.PercentOfTopRosters).ToList()) ;
        }
        

        [HttpPost("expectedValue")]
        [SwaggerOperation(Summary = "Calculate expected cost range based on projected point")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> CalculateExpectedValue([FromBody] List<Player> players, [FromQuery] double qbslope, [FromQuery] double qbintercept, [FromQuery] double qbresidual, [FromQuery] double rbslope, [FromQuery] double rbintercept, [FromQuery] double rbresidual, [FromQuery] double wrslope, [FromQuery] double wrintercept, [FromQuery] double wrresidual, [FromQuery] double teslope, [FromQuery] double teintercept, [FromQuery] double teresidual, [FromQuery] double rb1effectup, [FromQuery] double rb1effectdown)
        {
            CostAnalysis analysis = new CostAnalysis()
            {
                QBSlope = qbslope,
                QBIntercept = qbintercept,
                QBResidual = qbresidual,
                RBSlope = rbslope,
                RBIntercept = rbintercept,
                RBResidual = rbresidual,
                WRSlope = wrslope,
                WRIntercept = wrintercept,
                WRResidual = wrresidual,
                TESlope = teslope,
                TEIntercept = teintercept,
                TEResidual = teresidual,
                RB1EffectUp = rb1effectup,
                RB1EffectDown = rb1effectdown
            };
            List<Player> playersWithValues = _draftStrategyLogic.GetExpectedValue(players, analysis);
           return new OkObjectResult(playersWithValues);
        }

        [HttpPost("exampleRosters")]
        public async Task<IActionResult> GenerateExampleRosters([FromBody] List<Rosters> rosters, [FromHeader] List<int> PlayerIDs)
        { 
            List<Rosters> topRosters = new List<Rosters>();
            foreach(int id in PlayerIDs)
            {
                Rosters roster = rosters.Where(x=>x.QB == id || x.RB1 == id || x.WR1 == id || x.TE == id).FirstOrDefault();
                if (roster != null)
                {
                    topRosters.Add(roster);
                    rosters.Remove(roster);
                }
                
            }
            return new OkObjectResult(topRosters);
        }

        [HttpPost("exportSpreadsheet")]
        [SwaggerOperation(Summary = "Convert player data to Excel")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> ConvertToSpreadsheet([FromBody] List<Player> players)
        {
            string csv = _draftStrategyLogic.GetSpreadsheet(players.Where(x=>x.FA >= 0).ToList());
            var fileBytes = Encoding.UTF8.GetBytes(csv);
            return File(fileBytes, "text/csv","reports.csv");
        }

        [HttpPost("findRosters")]
        [SwaggerOperation(Summary = "Find best combinations")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> BuildRosters([FromBody] List<Player> players, [FromQuery] double targetPoints, [FromQuery] int teams, [FromQuery] int cap, [FromQuery] int qb, [FromQuery] int rb, [FromQuery] int wr, [FromQuery] int te, [FromQuery] int flex, [FromQuery] int k, [FromQuery] int def, [FromQuery] int bench)
        {
            Rules rules = new Rules()
            {
                Teams = teams,
                SalaryCap = cap,
                QB = qb,
                RB = rb,
                WR = wr,
                TE = te,
                FLEX = flex,
                DEF = def,
                K = k,
                Bench = bench
            };
            List<Rosters> rosters = _draftStrategyLogic.BuildBestRosters(players, rules, targetPoints);
            return new OkObjectResult(rosters);
        }

        [HttpPost("isolateTopPlayers")]
        public async Task<IActionResult> IsolateTopPlayers([FromBody] List<Player> players)
        {
            List<int> playerIDs = new List<int>();
            List<Player> topPlayers = players.Where(x => x.PercentOfTopRosters >= 10 && (x.Position == "QB" || x.Position == "RB" || x.Position == "WR" || x.Position == "TE")).ToList();
            foreach (Player player in topPlayers)
            {
                playerIDs.Add(player.PlayerID);
            }
            return new OkObjectResult(playerIDs);
        }

        [HttpPost("playerPlayoffPercent")]
        public async Task<IActionResult> CalculatePlayoffPercent([FromBody] List<Rosters> rosters)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            int count = 0;
            foreach (Rosters roster in rosters)
            {
                count += 1;
               if (result.ContainsKey(roster.QB))
                {
                    result[roster.QB] += 1;
                }
               else
                {
                    result[roster.QB] = 1;
                }

                if (result.ContainsKey(roster.RB1))
                {
                    result[roster.RB1] += 1;
                }
                else
                {
                    result[roster.RB1] = 1;
                }

                if (result.ContainsKey(roster.RB2))
                {
                    result[roster.RB2] += 1;
                }
                else
                {
                    result[roster.RB2] = 1;
                }

                if (result.ContainsKey(roster.WR1))
                {
                    result[roster.WR1] += 1;
                }
                else
                {
                    result[roster.WR1] = 1;
                }

                if (result.ContainsKey(roster.WR2))
                {
                    result[roster.WR2] += 1;
                }
                else
                {
                    result[roster.WR2] = 1;
                }

                if (result.ContainsKey(roster.TE))
                {
                    result[roster.TE] += 1;
                }
                else
                {
                    result[roster.TE] = 1;
                }

                if (result.ContainsKey(roster.FLEX))
                {
                    result[roster.FLEX] += 1;
                }
                else
                {
                    result[roster.FLEX] = 1;
                }

                if (result.ContainsKey(roster.DEF))
                {
                    result[roster.DEF] += 1;
                }
                else
                {
                    result[roster.DEF] = 1;
                }

                if (result.ContainsKey(roster.K))
                {
                    result[roster.K] += 1;
                }
                else
                {
                    result[roster.K] = 1;
                }

            }
            result[0] = count;
            return new OkObjectResult(result);
        }

        [HttpPost("pointAverages")]
        [SwaggerOperation(Summary = "Determine average points per position")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> CalculatePointAverages([FromBody] List<Player> players, [FromQuery] int teams, [FromQuery] int qb, [FromQuery] int rb, [FromQuery] int wr, [FromQuery] int te, [FromQuery] int flex, [FromQuery] int k, [FromQuery] int def, [FromQuery] int bench)
        {
            Rules rules = new Rules()
            {
                Teams = teams,
                QB = qb,
                RB = rb,
                WR = wr,
                TE = te,
                FLEX = flex,
                DEF = def,
                K = k,
                Bench = bench
            };
            Averages averages = _draftStrategyLogic.GetAverages(players, rules);
            return new OkObjectResult(averages);
        }

        [HttpPost("regressionAnalysis")]
        [SwaggerOperation(Summary = "Calculate effect of projected points on price, by positon")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> CalculateLineOfBestFit([FromBody] List<Player> players)
        {
            CostAnalysis analysis = _draftStrategyLogic.GetCostAnalysis(players.Where(x=>x.Cost > 0).ToList());
            return new OkObjectResult(analysis);
        }


        [HttpPost("relativePoints")]
        [SwaggerOperation(Summary = "Calculate relative value by position")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> CalculateRelativePoints([FromBody] List<Player> players, [FromQuery] double QB, [FromQuery] double RB1, [FromQuery] double RB2, [FromQuery] double WR1, [FromQuery] double WR2, [FromQuery] double TE, [FromQuery] double DEF, [FromQuery] double K, [FromQuery] double FLEX, [FromQuery] double FAQB, [FromQuery] double FARB, [FromQuery] double FAWR, [FromQuery] double FATE, [FromQuery] double FADEF, [FromQuery] double FAK)
        {
            Averages averages = new()
            {
                QB = QB,
                RB1 = RB1,
                RB2 = RB2,
                WR1 = WR1,
                WR2 = WR2,
                TE = TE,
                FLEX = FLEX,
                DEF = DEF,
                K = K,
                FAQB = FAQB,
                FARB = FARB,
                FAWR = FAWR,
                FATE = FATE,
                FADEF = FADEF,
                FAK = FAK
            };
            List<Player> playersWithRelativePoints = _draftStrategyLogic.GetRelativePoints(players, averages);
            return new OkObjectResult(playersWithRelativePoints);
        }

        [HttpPost("salaryChart")]
        [SwaggerOperation(Summary = "Rank best player available by salary and position")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> CalculateSalaryChart([FromBody] List<Player> players, [FromQuery] double QB, [FromQuery] double RB1, [FromQuery] double RB2, [FromQuery] double WR1, [FromQuery] double WR2, [FromQuery] double TE, [FromQuery] double DEF, [FromQuery] double K, [FromQuery] double FLEX)
        {
            Averages averages = new()
            {
                QB = QB,
                RB1 = RB1,
                RB2 = RB2,
                WR1 = WR1,
                WR2 = WR2,
                TE = TE,
                FLEX = FLEX,
                DEF = DEF,
                K = K
            };
            DraftStrategyLogic logic = new DraftStrategyLogic();
            List<Player> playerChart = logic.GetSalaryRank(players, averages);
            return new OkObjectResult(playerChart);
        }

        [HttpPost("strongRoster")]
        [SwaggerOperation(Summary = "Return a very above average draft roster")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> CalculateStrongRoster([FromBody] List<Player> players, [FromQuery] int cap, [FromQuery] int qb, [FromQuery] int rb, [FromQuery] int wr, [FromQuery] int te, [FromQuery] int flex, [FromQuery] int k, [FromQuery] int def, [FromQuery] int bench)
        {
            Rules rules = new Rules()
            {
                SalaryCap = cap,
                QB = qb,
                RB = rb,
                WR = wr,
                TE = te,
                FLEX = flex,
                DEF = def,
                K = k,
                Bench = bench
            };
            List<Player> topPlayers = _draftStrategyLogic.GetStrongRoster(players, rules);
            return new OkObjectResult(topPlayers);
        }

        [HttpPost("strongerRoster")]
        [SwaggerOperation(Summary = "Return a very above average draft roster")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> CalculateStrongerRoster([FromBody] List<Player> players, [FromQuery] int qb, [FromQuery] int rb1, [FromQuery] int rb2, [FromQuery] int wr1, [FromQuery] int wr2, [FromQuery] int te, [FromQuery] int flex, [FromQuery] int def, [FromQuery] int k, [FromQuery] int cap, [FromQuery] int bench)
        {
            List<Player> currentPlayers = _draftStrategyLogic.GetCurrentRoster(players, qb, rb1, rb2, wr1, wr2, te, flex, def, k);
            List<Player> topPlayers = _draftStrategyLogic.GetStrongerRoster(currentPlayers, players, cap, bench); 
            return new OkObjectResult(topPlayers);
        }
    }
}
