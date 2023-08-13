using FantasyFootball.DataAccess.Models;
using FantasyFootball.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace FantasyDraft.API.Controllers
{
    [Controller]
    [Route("dataimport")]
    public class DataImportController : Controller
    {
        public IDataImportLogic _dataImportLogic;
        public DataImportController(IDataImportLogic dataImportLogic)
        {
            _dataImportLogic = dataImportLogic;
        }

        [HttpPost("leagueRules")]
        [SwaggerOperation(Summary = "Fetch league rules from ESPN. Use espn_s2 cookie", Description = "{'ESPNLeagueID' = int, 'rulesCookie' = string}")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<League>), Description = "OK - found and returned rules")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Bad Request - error processing the request")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Not Found - no rules could be found")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal Server Error - server did not respond")]
        public async Task<IActionResult> GetRules([FromQuery] int ESPNLeagueID, [FromHeader] string rulesCookie)
        {
            try
            {
                Rules rules = await _dataImportLogic.FetchRules(ESPNLeagueID, rulesCookie);
                if (rules == null)
                    return new NotFoundResult();
                return new OkObjectResult(rules);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e);
            }
        }

        [HttpPost("playerProjections")]
        [SwaggerOperation(Summary = "Fetch player fantasy projections from ESPN. Use espn_s2 cookie", Description = "{'ESPNLeagueID' = int, 'projectionsCookie' = string, 'rules' = {} }")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<League>), Description = "OK - found and returned all players")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Bad Request - error processing the request")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Not Found - no players could be found")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal Server Error - server did not respond")]
        public async Task<IActionResult> GetPlayerProjections([FromQuery] int ESPNLeagueID, [FromHeader] string projectionsCookie, [FromBody] Rules rules)
        {
            try
            {
                List<Player> players = await _dataImportLogic.FetchProjections(ESPNLeagueID, projectionsCookie, rules);
                if (players == null)
                    return new NotFoundResult();
                return new OkObjectResult(players);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e);
            }
        }


    }
}
