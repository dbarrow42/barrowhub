using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Web;
using BarrowHubApp.Models;
using Microsoft.Extensions.Configuration;

namespace BarrowHubApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Spotify")]
    public class SpotifyController : Controller
    {
        private string client_id;
        private string client_secret;
        //private string tokenUrl = "https://accounts.spotify.com/api/token";
        private string authorizeUrl = "https://accounts.spotify.com/authorize";
        private string playerUrl = "https://api.spotify.com/v1/me/player";
        private HttpClient http = new HttpClient();

        IConfiguration _configuration;
        public SpotifyController(IConfiguration configuration)
        {
            _configuration = configuration;
            client_id = _configuration["client_id"];
            client_secret = _configuration["client_secret"];
        }

        public async Task<string> getAuthCodeUrl()
        {
            var url = String.Format("{0}?client_id={1}&response_type=token&redirect_uri={2}&scope=user-read-playback-state%20user-modify-playback-state", 
                authorizeUrl, client_id, HttpUtility.UrlEncode("http://localhost:4200/hub"));

            return url;
        }

        [HttpGet("player/{token}")]
        public async Task<Track> getSpotifyPlayerInfo(string token)
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", String.Format("Bearer {0}", token));
            HttpResponseMessage response = await http.GetAsync(String.Format("{0}/{1}", playerUrl, "currently-playing"));
            dynamic content;
            Track track = new Track();
            if (response.IsSuccessStatusCode)
            {
                content = await response.Content.ReadAsStringAsync();
                JToken jt = JObject.Parse(content);
                var a = jt["item"];
                track.title = jt["item"]["name"].ToString();
                track.artist = jt["item"]["artists"][0]["name"].ToString();
                track.imgUrl = jt["item"]["album"]["images"][1]["url"].ToString();
                track.isPlaying = Convert.ToBoolean(jt["is_playing"].ToString());
            }
            return track;
        }

        [HttpPut("player/pause/{token}")]
        public async Task<bool> pause(string token)
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", String.Format("Bearer {0}", token));
            HttpResponseMessage response = await http.PutAsync(String.Format("{0}/{1}", playerUrl, "pause"), null);
            return response.IsSuccessStatusCode;
        }

        [HttpPut("player/play/{token}")]
        public async Task<bool> play(string token)
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", String.Format("Bearer {0}", token));
            HttpResponseMessage response = await http.PutAsync(String.Format("{0}/{1}", playerUrl, "play"), null);
            return response.IsSuccessStatusCode;
        }

        [HttpPut("player/next/{token}")]
        public async Task<bool> next(string token)
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", String.Format("Bearer {0}", token));
            HttpResponseMessage response = await http.PostAsync(String.Format("{0}/{1}", playerUrl, "next"), null);
            return response.IsSuccessStatusCode;
        }

        [HttpPut("player/previous/{token}")]
        public async Task<bool> previous(string token)
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", String.Format("Bearer {0}", token));
            HttpResponseMessage response = await http.PostAsync(String.Format("{0}/{1}", playerUrl, "previous"), null);
            return response.IsSuccessStatusCode;
        }
    }

}