using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BarrowHubApp.Models;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BarrowHubApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Pug")]
    public class PugController : Controller
    {
        private string pugApiUrl = "https://dog.ceo/api/breed/pug/images/random";
        private HttpClient http = new HttpClient();

        public async Task<Pug> getRandomPug()
        {
            Pug pug = new Pug();
            HttpResponseMessage response = await http.GetAsync(pugApiUrl);
            if (response.IsSuccessStatusCode)
            {
                var p = await response.Content.ReadAsStringAsync();
                pug.pugPic = JObject.Parse(p)["message"].ToString();
                Console.WriteLine(pug.pugPic);
            }
            return pug;
        }
    }
}