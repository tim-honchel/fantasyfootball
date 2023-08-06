using FantasyFootball.DataAccess.Models;
using FantasyFootball.Logic.Implementations;
using FantasyFootball.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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



        [HttpPost("pointAverages")]
        [SwaggerOperation(Summary = "Determine average points per position")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> CalculatePointAverages([FromBody] List<Player> players, [FromQuery] int teams, [FromQuery] int qb, [FromQuery] int rb, [FromQuery] int wr, [FromQuery] int te, [FromQuery] int flex, [FromQuery] int k, [FromQuery] int def)
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
                K = k
            };
            Averages averages = _draftStrategyLogic.GetAverages(players, rules);
            return new OkObjectResult(averages);
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

        [HttpPostAttribute("strongerRoster")]
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
