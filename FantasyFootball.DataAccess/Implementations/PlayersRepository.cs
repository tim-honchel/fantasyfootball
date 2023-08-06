using FantasyFootball.DataAccess.Interfaces;
using FantasyFootball.DataAccess.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http.Json;

namespace FantasyFootball.DataAccess.Implementations
{
    public class PlayersRepository : IPlayersRepository
    {
        public readonly IConfiguration _configuration;
        public PlayersRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<List<Player>> RequestProjections(int ESPNLeagueID, string projectionsCookie, string positions, int limit)
        {
            string filter = _configuration["ESPNFantasyFilter"];
            filter = filter.Replace("[limit]", limit.ToString());
            filter = filter.Replace("[positions]", positions);

            string url = _configuration["ESPNProjectionsURL"];
            if (url == null)
                return null;
            url = url.Replace("[year]", DateTime.Now.Year.ToString());
            url = url.Replace("[leagueID]", ESPNLeagueID.ToString());
           
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(url);
            httpClient.DefaultRequestHeaders.Add("Cookie", projectionsCookie);
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            httpClient.DefaultRequestHeaders.Add("X-Fantasy-Filter", filter);

            string response = await httpClient.GetStringAsync(url);

            List<Player> allPlayers = new List<Player>();

            var players = JObject.Parse(response).GetValue("players");

            foreach (JObject player in players)
            {
                var playerDetail = JObject.Parse(player.ToString()).GetValue("player").ToString();
                var stats = JObject.Parse(playerDetail).GetValue("stats")[0].ToString();
                int positionID = Convert.ToInt16(JObject.Parse(playerDetail).GetValue("defaultPositionId").ToString());
                var position = GetPosition(positionID);
                Player newPlayer = new Player()
                {
                    PlayerID = Convert.ToInt32(JObject.Parse(player.ToString()).GetValue("id").ToString()),
                    Cost = Convert.ToInt16(JObject.Parse(player.ToString()).GetValue("draftAuctionValue").ToString()),
                    LastName = JObject.Parse(playerDetail).GetValue("lastName").ToString(),
                    FirstInitial = JObject.Parse(playerDetail).GetValue("firstName").ToString()[0].ToString(),
                    Position = position,
                    Team = JObject.Parse(playerDetail).GetValue("proTeamId").ToString(),
                    WeeklyPoints = Math.Round(Convert.ToDouble(JObject.Parse(stats).GetValue("appliedAverage").ToString()),2)
                };
                if ( newPlayer.Position == "DEF")
                {
                    newPlayer.FirstInitial = "D";
                    newPlayer.LastName = JObject.Parse(playerDetail).GetValue("firstName").ToString();
                };
                allPlayers.Add(newPlayer);

            }

            return allPlayers;

        }

        public string GetPosition(int positionID)
        {
            var positionFinder = new Dictionary<int, string>()
            {
                {0, "QB" },
                {1, "QB" },
                {2, "RB" },
                {3, "WR" },
                {4, "TE" },
                {5, "K" },
                {16, "DEF" },

            };
            return positionFinder.ContainsKey(positionID) ? positionFinder[positionID] : "";
        }


    }
}
