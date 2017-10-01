using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Bot3ulerLogic.Modules.Queue;
using Bot3ulerLogic.Services.Queue;
using GWC.WeatherUnderground;
using GWC.WeatherUnderground.DataTypes;
using Discord;

namespace Bot3ulerLogic.Services
{
    public class WeatherUndergroundService : APIConnectionScheduled
    { 
        WeatherUnderground WeatherUndergroundConnection;
        public WeatherUndergroundService(WeatherUnderground weatherUndergroundConnection, ServerUpdater<string> console, DiscordSocketClient client, ScheduleMaker scheduleMaker) : base(console, client, scheduleMaker)
        {
            WeatherUndergroundConnection = weatherUndergroundConnection;
            CommandName = "weather";
        }
        public async Task<WeatherUndergroundResponse> GetWeatherUndergroundConditionsResponse(string query)
        {
            return await WeatherUndergroundConnection.QuerySearch(WeatherUnderground.QueryType.conditions, query);
        }
        public async Task<WeatherUndergroundResponse> GetWeatherUndergroundSatelliteResponse(string query)
        {
            return await WeatherUndergroundConnection.QuerySearch(WeatherUnderground.QueryType.satellite, query);
        }
        public async Task<Embed> GetWeatherMap(string query)
        {
            EmbedBuilder builder = new EmbedBuilder();
            WeatherUndergroundResponse wuresponse = await GetWeatherUndergroundSatelliteResponse(query);
            if (wuresponse != null && wuresponse.satellite != null)
            {
                builder.WithTitle($"Weather Map for {query}");
                builder.ImageUrl = wuresponse.satellite.image_url;
                EmbedFooterBuilder test = new EmbedFooterBuilder();
                test.WithIconUrl("https://icons.wxug.com/logos/PNG/wundergroundLogo_4c.png");
                test.WithText($"WeatherUnderground");
                builder.WithFooter(test);
                builder.WithColor(Color.DarkGrey);
            }
            else
            {
                builder.Title = $"No result for search \"{query}\"";
            }
            return builder.Build();
        }
        public async Task<Embed> GetCurrent(string query)
        {
            EmbedBuilder builder = new EmbedBuilder();
            WeatherUndergroundResponse wuresponse = await GetWeatherUndergroundConditionsResponse(query);
            if (wuresponse != null && wuresponse.current_observation != null)
            {
                var co = wuresponse.current_observation;
                builder.WithThumbnailUrl(co.icon_url);
                builder.WithTitle($"{wuresponse.current_observation.display_location.city}, {wuresponse.current_observation.display_location.state_name}");
                builder.AddField("Current Temperature", $"{wuresponse.current_observation.temp_f}");
                builder.AddField("Humidity", $"{wuresponse.current_observation.relative_humidity}");
                builder.AddField("Wind", $"{wuresponse.current_observation.wind_string}");
                builder.AddField("Forecast", $"{wuresponse.current_observation.forecast_url}");
                EmbedFooterBuilder test = new EmbedFooterBuilder();
                test.WithIconUrl("https://icons.wxug.com/logos/PNG/wundergroundLogo_4c.png");
                test.WithText($"WeatherUnderground");
                builder.WithFooter(test);
                builder.WithColor(Color.DarkGrey);
            }
            else
            {
                builder.Title = $"No result for search \"{query}\"";
            }
            return builder.Build();

        }
        public async Task<string> GetCurrentTemp(string query)
        {
            StringBuilder response = new StringBuilder();
            WeatherUndergroundResponse wuresponse = await GetWeatherUndergroundConditionsResponse(query);
            if (wuresponse != null)
            {
                if (wuresponse.current_observation != null)
                    return $"{wuresponse.current_observation.display_location.city} {wuresponse.current_observation.display_location.state} is currently {wuresponse.current_observation.temp_f}";
                else if (wuresponse.response.results != null)
                {
                    response.Append("Which one?```");
                    foreach (Result res in wuresponse.response.results)
                        response.Append($"{res.city} {res.state}\n");
                    response.Append("```");
                }
                else
                    return "error";

            }
            else
            {
                return "error";
            }
            return response.ToString().Trim();
        }
        public async Task<string> GetWeatherUndergroudLink(string query)
        {
            StringBuilder response = new StringBuilder();
            WeatherUndergroundResponse wuresponse = await GetWeatherUndergroundConditionsResponse(query);
            if (wuresponse != null)
            {
                if (wuresponse.current_observation != null)
                    return $"Check out the forecast at {wuresponse.current_observation.forecast_url}";
                else if (wuresponse.response.results != null)
                {
                    response.Append("Which one?```");
                    foreach (Result res in wuresponse.response.results)
                        response.Append($"{res.city} {res.state}\n");
                    response.Append("```");
                }
                else
                    return "error";

            }
            else
            {
                return "error";
            }
            return response.ToString().Trim();
        }
        public async Task<string> StartCurrentTempSchedule(string query, ISocketMessageChannel channelRequested, int delay)
        {
            StringBuilder response = new StringBuilder();
            WeatherUndergroundResponse wuresponse = await GetWeatherUndergroundConditionsResponse(query);
            if (wuresponse != null)
            {
                if (wuresponse.current_observation != null)
                {
                    return await ScheduleMaker.AddSchedule(new WeatherUndergroundSchedule(CommandName, query, channelRequested, delay, GetCurrentTemp)) ? "Schedule Made Succesfully" : "Schedule Failed";
                }
                else if (wuresponse.response.results != null)
                {
                    response.Append("Which one?```");
                    foreach (Result res in wuresponse.response.results)
                        response.Append($"{res.city} {res.state}\n");
                    response.Append("```");
                }
                else
                    return "error";

            }
            else
            {
                return "error";
            }
            return response.ToString().Trim();
        }
    }
}
