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
            var r = new Dictionary<K, long>(a);
            foreach (var pair in b)
            {
                if (a.ContainsKey(pair.Key))
                    a[pair.Key] += pair.Value;
                else
                    a.Add(pair.Key, pair.Value);
            }
        }

        [HttpGet("videos-info")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> GetVideosInfo([FromQuery] DateTime since, [FromQuery] DateTime until)
        {
            var videos = await _service.GetVideos(since, until, "title", "length");
            var rankingByCountry = new Dictionary<string, long>();
            var rankingByRegion = new Dictionary<string, long>();
            var demographic = new Dictionary<string, long>();

            var videosInfo = await Task.WhenAll(videos.AsParallel().Select(async x =>
            {
                var id = x["id"] as string;
                var countries = await _service.GetViewsByCountry(id);
                var regions = await _service.GetViewsByRegion(id);
                var demo = await _service.GetViewsByGenderAge(id);

                AggregateDictionaries(rankingByCountry, countries);
                AggregateDictionaries(rankingByRegion, regions);
                AggregateDictionaries(demographic, demo);

                return new
                {
                    title = (x.ContainsKey("title") ? x["title"] : null),
                    date = x["created_time"],
                    reach = await _service.GetVideoTotalImpressions(id),
                    views = await _service.GetVideoTotalViews(id),
                    length = x["length"],
                    rankingByRegion = regions,
                    rankingByCountry = countries,
                    demographic = demo
                };
            }));
            
            return Ok(new
            {
                videos_count = videosInfo.Length,
                total_reach = videosInfo.Sum(x => x.reach),
                total_views = videosInfo.Sum(x => x.views),
                total_countries = rankingByCountry.Count,
                total_regions = rankingByRegion.Count,
                videos = videosInfo,
                rankingByRegion = rankingByRegion,
                rankingByCountry = rankingByCountry,
                demographic = demographic
            });
        }
    }
}