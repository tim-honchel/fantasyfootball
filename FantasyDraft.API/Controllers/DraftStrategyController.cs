using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace FantasyDraft.API.Controllers
{
    [Controller]
    [Route("draftstrategy")]
    public class DraftStrategyController : Controller
    {
        [HttpGet("bestRoster")]
        [SwaggerOperation(Summary = "Return the best possible draft roster")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> CalculateBestRoster()
        {
            return new OkResult();
        }

        [HttpGet("pointAverages")]
        [SwaggerOperation(Summary = "Determine average points per position")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> CalculatePointAverages()
        {
            return new OkResult();
        }

        [HttpGet("relativePoints")]
        [SwaggerOperation(Summary = "Determine player value relative to position")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> CalculateRelativePoints()
        {
            return new OkResult();
        }

        [HttpGet("salaryChart")]
        [SwaggerOperation(Summary = "Rank best player available by salary and position")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> CalculateSalaryChart()
        {
            return new OkResult();
        }

        [HttpGet("sortedPlayers")]
        [SwaggerOperation(Summary = "Sort players by points and position")]
        [ProducesResponseType(typeof(void), 200)]
        public async Task<IActionResult> SortPlayers()
        {
            return new OkResult();
        }
    }
}
