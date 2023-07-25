using FantasyFootball.DataAccess.Interfaces;
using FantasyFootball.DataAccess.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http.Json;

namespace FantasyFootball.DataAccess.Implementations
{
    public class RulesRepository : IRulesRepository
    {
        public readonly IConfiguration _configuration;
        public RulesRepository()
        {

        }
        public RulesRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Rules> RequestRules(int ESPNLeagueID, string rulesCookie)
        {
            string url = _configuration["ESPNRulesURL"];
            if (url == null)
                return null;
            url = url.Replace("[year]", DateTime.Now.Year.ToString());
            url = url.Replace("[leagueID]", ESPNLeagueID.ToString());

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(url);
            httpClient.DefaultRequestHeaders.Add("Cookie", rulesCookie);

            var response = await httpClient.GetStringAsync(url);

            var settings = JObject.Parse(response).GetValue("settings").ToString();
            var draftSettings = JObject.Parse(settings).GetValue("draftSettings").ToString();
            var rosterSettings = JObject.Parse(settings).GetValue("rosterSettings").ToString();
            var lineupSlotCounts = JObject.Parse(rosterSettings).GetValue("lineupSlotCounts").ToString();
            var scheduleSettings = JObject.Parse(settings).GetValue("scheduleSettings").ToString();

            int teams = Convert.ToInt16(JObject.Parse(settings).GetValue("size").ToString()); ;
            int playoffTeams = Convert.ToInt16(JObject.Parse(scheduleSettings).GetValue("playoffTeamCount").ToString()); ;
            int salaryCap = Convert.ToInt16(JObject.Parse(draftSettings).GetValue("auctionBudget").ToString());
            int qb = Convert.ToInt16(JObject.Parse(lineupSlotCounts).GetValue("0").ToString());
            int rb = Convert.ToInt16(JObject.Parse(lineupSlotCounts).GetValue("2").ToString());
            int wr = Convert.ToInt16(JObject.Parse(lineupSlotCounts).GetValue("4").ToString());
            int te = Convert.ToInt16(JObject.Parse(lineupSlotCounts).GetValue("6").ToString());
            int flex = Convert.ToInt16(JObject.Parse(lineupSlotCounts).GetValue("21").ToString());
            int def = Convert.ToInt16(JObject.Parse(lineupSlotCounts).GetValue("16").ToString());
            int k = Convert.ToInt16(JObject.Parse(lineupSlotCounts).GetValue("17").ToString());
            int bench = Convert.ToInt16(JObject.Parse(lineupSlotCounts).GetValue("20").ToString());
            bool otherPositions = false;
            bool keepers = Convert.ToInt16(JObject.Parse(draftSettings).GetValue("keeperCount").ToString()) > 0;
            bool auctionDraft = JObject.Parse(draftSettings).GetValue("type").ToString() == "AUCTION";

            Rules rules = new Rules()
            {
                Teams = teams,
                PlayoffTeams = playoffTeams,
                SalaryCap = salaryCap,
                QB = qb,
                RB = rb,
                WR = wr,
                TE = te,
                FLEX = flex,
                DEF = def,
                K = k,
                Bench = bench,
                OtherPositions = otherPositions,
                Keepers = keepers,
                AuctionDraft = auctionDraft
            };

            return rules;
        }


    }
}
