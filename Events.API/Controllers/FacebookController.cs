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

        // private void AggregateDictionaries<K>(Dictionary<K, long> a, Dictionary<K, long> b)
        // {
        //     if (b != null) 
        //     {
        //         foreach (var pair in b)
        //         {
        //             if (a.ContainsKey(pair.Key))
        //                 a[pair.Key] += pair.Value;
        //             else
        //                 a.Add(pair.Key, pair.Value);
        //         }
        //     }
        // }

        [HttpGet("videos-info")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> GetVideosInfo([FromQuery] string pageIdentifier,  [FromQuery] DateTime since, [FromQuery] DateTime until)
        {
            var videos = await _service.GetVideos(pageIdentifier, since, until, "title", "length");

            var videosInfo = await Task.WhenAll(videos.AsParallel().Select(async x =>
            {
                var id = x["id"] as string;
                var actionsByType = await _service.GetVideoActionsCountByType(pageIdentifier, id);                

                return new
                {
                    title = (x.ContainsKey("title") ? x["title"] : null),
                    date = x["created_time"],
                    reach = await _service.GetVideoTotalImpressions(pageIdentifier, id),
                    views = await _service.GetVideoTotalViews(pageIdentifier, id),
                    comments = actionsByType.ContainsKey("comment") ? actionsByType["comment"] : 0,
                    shares = actionsByType.ContainsKey("share") ? actionsByType["share"] : 0,
                    crosspost_count = await _service.GetCrosspostVideoCount(pageIdentifier, id),
                    total_view_time = await _service.GetVideoTotalViewTime(pageIdentifier, id),
                    reactions = await _service.GetVideoReactionsByType(pageIdentifier, id),
                    length = x["length"],
                    ranking_by_region = await _service.GetViewsByRegion(pageIdentifier, id),
                    ranking_by_country = await _service.GetViewsByCountry(pageIdentifier, id),
                };
            }));
            
            return Ok(new
            {
                videos_count = videosInfo.Length,
                videos = videosInfo,
            });
        }
    }
}
