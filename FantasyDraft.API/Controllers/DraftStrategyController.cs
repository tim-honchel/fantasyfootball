using FantasyFootball.DataAccess.Models;
using FantasyFootball.Logic.Implementations;
using FantasyFootball.Logic.Interfaces;
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

        [HttpPost("exportSpreadsheet")]
        [SwaggerOperation(Summary = "Convert player data to Excel")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> CanvertToSpreadsheet([FromBody] List<Player> players)
        {
            string csv = _draftStrategyLogic.GetSpreadsheet(players.Where(x=>x.FA >= 0).ToList());
            var fileBytes = Encoding.UTF8.GetBytes(csv);
            return File(fileBytes, "text/csv","reports.csv");
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
