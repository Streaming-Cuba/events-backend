using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Events.API.Data;
using Events.API.DTO;
using Events.API.Helpers;
using Events.API.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Events.API.Services;

namespace Events.API.Controllers
{
    [Route("/api/v1/facebook")]
    [ApiController]
    public class FacebookController : ControllerBase
    {
        private readonly FacebookService _service;

        public FacebookController(FacebookService service)
        {
            _service = service;
        }

        private void AggregateDictionaries<K>(Dictionary<K, long> a, Dictionary<K, long> b)
        {
            if (b != null) 
            {
                foreach (var pair in b)
                {
                    if (a.ContainsKey(pair.Key))
                        a[pair.Key] += pair.Value;
                    else
                        a.Add(pair.Key, pair.Value);
                }
            }
        }

        [HttpGet("videos-info")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> GetVideosInfo([FromQuery] DateTime since, [FromQuery] DateTime until)
        {
            var videos = await _service.GetVideos(since, until, "title", "length");
            var rankingByCountryTotal = new Dictionary<string, long>();
            var rankingByRegionTotal = new Dictionary<string, long>();
            var demographicTotal = new Dictionary<string, long>();
            var reactionsTotal = new Dictionary<string, long>();

            var videosInfo = await Task.WhenAll(videos.AsParallel().Select(async x =>
            {
                var id = x["id"] as string;
                var countries = await _service.GetViewsByCountry(id);
                var regions = await _service.GetViewsByRegion(id);
                var demographic = await _service.GetViewsByGenderAge(id);
                var reactions = await _service.GetVideoReactionsByType(id);

                AggregateDictionaries(rankingByCountryTotal, countries);
                AggregateDictionaries(rankingByRegionTotal, regions);
                AggregateDictionaries(demographicTotal, demographic);
                AggregateDictionaries(reactionsTotal, reactions);

                return new
                {
                    title = (x.ContainsKey("title") ? x["title"] : null),
                    date = x["created_time"],
                    reach = await _service.GetVideoTotalImpressions(id),
                    views = await _service.GetVideoTotalViews(id),
                    comments = await _service.GetVideoCountComments(id),
                    shares = await _service.GetVideoSharesCount(id),
                    crosspost_count = await _service.GetCrosspostVideoCount(id),
                    reactions = reactions,
                    length = x["length"],
                    ranking_by_region = regions,
                    ranking_by_country = countries,
                    demographic = demographic
                };
            }));
            
            return Ok(new
            {
                videos_count = videosInfo.Length,
                total_reach = videosInfo.Sum(x => x.reach),
                total_views = videosInfo.Sum(x => x.views),
                total_countries = rankingByCountryTotal.Count,
                total_regions = rankingByRegionTotal.Count,
                total_comments = videosInfo.Sum(x => x.comments),
                total_shares = videosInfo.Sum(x => x.shares),
                total_crossposts = videosInfo.Sum(x => x.crosspost_count),
                total_reactions = reactionsTotal,
                videos = videosInfo,
                ranking_by_region = rankingByRegionTotal,
                ranking_by_country = rankingByCountryTotal,
                demographic = demographicTotal
            });
        }
    }
}
