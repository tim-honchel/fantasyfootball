using FantasyFootball.DataAccess.Models;
using FantasyFootball.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace FantasyDraft.API.Controllers
{
    [Controller]
    [Route("league")]
    public class LeagueController : Controller
    {
        public ILeagueLogic _leagueLogic;
        public LeagueController(ILeagueLogic leagueLogic)
        {
            _leagueLogic = leagueLogic;
        }


        [HttpGet("all")]
        [SwaggerOperation(Summary = "View all saved fantasy leagues")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<League>), Description = "OK - found and returned all leagues")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Bad Request - error processing the request")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Not Found - no leagues could be found")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal Server Error - server did not respond")]
        public async Task<IActionResult> GetAllLeagues()
        {
            try
            {
                List<League> leagues = await _leagueLogic.ViewAllLeagues();
                if (leagues == null)
                    return new NotFoundResult();
                return new OkObjectResult(leagues);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            
        }

        [HttpGet()]
        [SwaggerOperation(Summary = "Get authorization info for specific league")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(League), Description = "OK - found and returned league information")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Bad Request - error processing the request")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Description = "Not Found - no league could be found")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal Server Error - server did not respond")]
        public async Task<IActionResult> GetLeague([FromQuery] int ESPNLeagueID)
        {
            try
            {
                League league = await _leagueLogic.SelectLeague(ESPNLeagueID);
                if (league == null)
                    return new NotFoundResult();
                return new OkObjectResult(league);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
            
        }


        [HttpPost("new")]
        [SwaggerOperation(Summary = "Add a fantasy league, including authorization cookies")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(League), Description = "OK - created and saved league")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Bad Request - error processing the request")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "Unauthorized - cookies were not valid")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal Server Error - server did not respond")]
        public async Task<IActionResult> PostLeagueInfo([FromBody] League league)
        {
            try
            {
                League? newLeague = await _leagueLogic.AddLeague(league.LeagueName, league.ESPNID, league.RulesCookie, league.ProjectionsCookie);
                if (newLeague == null)
                    return new UnauthorizedObjectResult("Cookies were invalid. League was not saved.");
                return new OkObjectResult(newLeague);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        [HttpPut("edit")]
        [SwaggerOperation(Summary = "Edit authorization cookies for a league")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(League), Description = "OK - updated league")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Bad Request - error processing the request")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "Unauthorized - cookies were not valid")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal Server Error - server did not respond")]
        public async Task<IActionResult> PutLeagueInfo([FromBody] League league)
        {
            try
            {
                League? updatedLeague = await _leagueLogic.EditLeague(league);
                if (updatedLeague == null)
                    return new UnauthorizedObjectResult("Cookies were invalid. League was not updated.");
                return new OkObjectResult(updatedLeague);
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }

        [HttpDelete("delete")]
        [SwaggerOperation(Summary = "Delete reference to a fantasy league")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(League), Description = "OK - deleted league")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Bad Request - error processing the request")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "Unauthorized - provided information did not match any saved league records")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal Server Error - server did not respond")]
        public async Task<IActionResult> DeleteLeague([FromBody] League league)
        {
            try
            {
                if (await _leagueLogic.RemoveLeague(league) == false)
                    return new UnauthorizedObjectResult("Provided information did not match any saved league records.");
                return new OkResult();
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(e.Message);
            }
        }




    }
}
