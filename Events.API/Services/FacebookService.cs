using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Events.API.Services
{
    public class FacebookService
    {
        private readonly HttpClient _client;
        private readonly string _accessToken;
        private readonly string _pageIdentifier;
        private const string ApiGraphVersion = "v11.0";
        private const string BaseUrl = "https://graph.facebook.com";

        private string _pageIDCache = null;

        public FacebookService(HttpClient client, IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            client.BaseAddress = new Uri(BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            _accessToken = configuration["Facebook:Token"];
            _pageIdentifier = configuration["Facebook:PageIdentifier"];

            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<List<JToken>> Request(string requestUri)
        {
            var r = JObject.Parse(await _client.GetStringAsync($"/{ApiGraphVersion}/{requestUri}"));
            var tokens = new List<JToken>();

            for (; ; )
            {
                if (!r.ContainsKey("data"))
                    throw new Exception("Invalid response from Facebook API Graph");

                if (!r["data"].HasValues)
                    break;

                tokens.AddRange(r["data"].AsEnumerable());

                // advance cursor
                if (!r.ContainsKey("paging") || r["paging"]["next"] == null)
                    break;

                var url = r["paging"]["next"].ToString();
                r = JObject.Parse(await _client.GetStringAsync(url));
            }

            return tokens;
        }

        public async Task<string> GetId() 
        {
            if (_pageIDCache == null)
                _pageIDCache = JObject.Parse(await _client.GetStringAsync($"/{ApiGraphVersion}/{_pageIdentifier}?access_token={_accessToken}"))["id"]
                                      .ToString();
            return _pageIDCache;
        }

        public async Task<List<Dictionary<string, object>>> GetVideos(DateTime start, DateTime end, params string[] fields)
        {
            if ((end - start).Days >= 90)
                throw new ArgumentOutOfRangeException("Max time diference is of 90 days");

            return (await Request($"{_pageIdentifier}/videos?access_token={_accessToken}&fields=created_time,{string.Join(",", fields)}&since={start.ToString("s")}&until={end.ToString("s")}"))
                    .Select(x => x.ToObject<Dictionary<string, object>>())
                    .ToList();
        }

        public async Task<long?> GetVideoCountComments(string videoId)
        {
            var r = JObject.Parse(await _client.GetStringAsync($"/{ApiGraphVersion}/{videoId}/comments?access_token={_accessToken}&limit=0&summary=1"));

            if (r["summary"] != null) 
                return r["summary"]["total_count"].Value<long>();
            return null;
        }

        public async Task<Dictionary<string, long>> GetVideoReactionsByType(string videoId)
        {
            var data = await Request($"{videoId}/video_insights/total_video_reactions_by_type_total?access_token={_accessToken}");

            if (data.Count == 0)
                return new Dictionary<string, long>();

            return data.First()["values"]
                       .First()["value"]
                       .ToObject<Dictionary<string, long>>();
        }

        public async Task<Dictionary<string, long>> GetVideoActionsCountByType(string videoId)
        {
            var data = await Request($"{videoId}/video_insights/total_video_stories_by_action_type?access_token={_accessToken}");

            if (data.Count == 0)
                return new Dictionary<string, long>();

            return data.First()["values"]
                       .First()["value"]
                       .ToObject<Dictionary<string, long>>();        
        }

        public async Task<long?> GetVideoTotalViews(string videoId)
        {
            var data = await Request($"{videoId}/video_insights/total_video_views?access_token={_accessToken}");

            if (data.Count == 0)
                return null;

            return data.First()["values"]
                       .First()["value"]
                       .Value<long>();
        }

        public async Task<long?> GetVideoTotalImpressions(string videoId)
        {
            var data = await Request($"{videoId}/video_insights/total_video_impressions?access_token={_accessToken}");

            if (data.Count == 0)
                return null;

            return data.First()["values"]
                       .First()["value"]
                       .Value<long?>();
        }

        public async Task<long?> GetCrosspostVideoCount(string videoId)
            => (await Request($"{videoId}/crosspost_shared_pages?access_token={_accessToken}&limit=1000")).Count;

        public async Task<Dictionary<string, long>> GetViewsByGenderAge(string videoId)
        {
            var data = await Request($"{videoId}/video_insights/total_video_view_time_by_age_bucket_and_gender?access_token={_accessToken}");

            if (data.Count == 0)
                return new Dictionary<string, long>();

            return data.First()["values"]
                       .First()["value"]
                       .ToObject<Dictionary<string, long>>()
                       .Where(x => !x.Key.StartsWith("U") && x.Value > 0)
                       .ToDictionary(k => k.Key, v => v.Value);
        }

        public async Task<Dictionary<string, long>> GetViewsByCountry(string videoId)
        {
            var data = await Request($"{videoId}/video_insights/total_video_view_time_by_country_id?access_token={_accessToken}");

            if (data.Count == 0)
                return new Dictionary<string, long>();

            return data.First()["values"]
                       .First()["value"]
                       .ToObject<Dictionary<string, long>>()
                       .Where(x => x.Value > 0)
                       .ToDictionary(k => k.Key, v => v.Value);
        }

        public async Task<Dictionary<string, long>> GetViewsByRegion(string videoId)
        {
            var data = await Request($"{videoId}/video_insights/total_video_view_time_by_region_id?access_token={_accessToken}");

            if (data.Count == 0)
                return new Dictionary<string, long>();

            return data.First()["values"]
                       .First()["value"]
                       .ToObject<Dictionary<string, long>>()
                       .Where(x => x.Value > 0)
                       .ToDictionary(k => k.Key, v => v.Value);
        }
    }
}