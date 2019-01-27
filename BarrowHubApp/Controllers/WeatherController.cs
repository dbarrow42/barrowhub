using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BarrowHubApp.Models;

namespace BarrowHubApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Weather")]
    public class WeatherController : Controller
    {
        private HttpClient http = new HttpClient();
        private string apiKey = "826cfcaf9a787e54d5717e445dbc74aa";
        private string apiUrl = "https://api.darksky.net/forecast/";
        private string longitude = "-83.154040";
        private string latitude = "42.562940";
        private string weatherUrl = "";
        private string excludes = "exclude=minutely";
        private string getWeatherUrl()
        {
            if(weatherUrl.Length == 0)
            {
                weatherUrl = String.Format("{0}{1}/{2},{3}?{4}", apiUrl, apiKey, latitude, longitude, excludes);
            }
            return weatherUrl;
        }
        public async Task<IList<WeatherReport>> getWeather()
        {
            HttpResponseMessage response = await http.GetAsync(getWeatherUrl());
            IList<WeatherReport> reports = new List<WeatherReport>();
            if (response.IsSuccessStatusCode)
            {
                WeatherReport report1 = new WeatherReport();
                var weather = await response.Content.ReadAsStringAsync();
                JToken data = JObject.Parse(weather)["currently"];
                report1.icon = data["icon"].ToString();
                report1.summary = data["summary"].ToString();
                report1.temperature = (int) Convert.ToDouble(data["temperature"].ToString()) + "#";
                DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                date = date.AddSeconds(Int32.Parse(data["time"].ToString())).ToLocalTime();
                report1.time = date;
                reports.Add(report1);
                ;
                for (int i = 1; i < 6; i++)
                {
                    WeatherReport report = new WeatherReport();
                    data = JObject.Parse(weather)["daily"]["data"][i];
                    report.icon = data["icon"].ToString();
                    report.summary = data["summary"].ToString();
                    report.temperature = (int)Convert.ToDouble(data["temperatureLow"].ToString()) + "# - ";
                    report.temperature += (int)Convert.ToDouble(data["temperatureHigh"].ToString()) + "#";
                    DateTime date2 = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                    date2 = date2.AddSeconds(Int32.Parse(data["time"].ToString())).ToLocalTime();
                    report.time = date2;
                    reports.Add(report);
                }

            }
            return reports;
        }
    }
}