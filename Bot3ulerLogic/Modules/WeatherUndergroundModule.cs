using Bot3ulerLogic.Preconditions;
using Bot3ulerLogic.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bot3ulerLogic.Modules
{
    [Group("wu"), CheckSource("wu")]
    public class WeatherUndergroundModule : GWCModule
    {
        //private WeatherUndergroundService WeatherUnderground;
        private CommandService _services;
        public WeatherUndergroundModule(WeatherUndergroundService service, CommandService services)
        {
            Service = service;
            (Service as WeatherUndergroundService).UpdateConsole("wu module created");
            _services = services;
        }
        [Command("current"), Summary("Get current weather temp")]
        public async Task CurrentWeatherQuery([Remainder, Summary("query")] string query)
        {
            await ReplyAsync(await (Service as WeatherUndergroundService).GetCurrentTemp(query));
        }
        [Command("schedulecurrent"), Summary("current temp schedule")]
        public async Task CurrentWeatherQuery([Summary("delay")] int delay, [Remainder, Summary("query")] string query)
        {
            await ReplyAsync(await (Service as WeatherUndergroundService).StartCurrentTempSchedule(query, Context.Channel, delay));
        }
        [Command("link"), Summary("Get weather link")]
        public async Task LinkWeatherQuery([Remainder, Summary("query")] string query)
        {
            await ReplyAsync(await (Service as WeatherUndergroundService).GetWeatherUndergroudLink(query));
        }
        [Command("stopschedule"), Summary("Stop schedule weather stream")]
        public async Task ScheduleStop([Remainder, Summary("query")] string query)
        {
            await ReplyAsync(await (Service as WeatherUndergroundService).StopSchedule(query));
        }
        [Command("getschedules"), Summary("Get all weather schedules")]
        public async Task GetSchedules()
        {
            await ReplyAsync(await (Service as WeatherUndergroundService).GetAllSchedules());
        }
        /*[Command("help"), Summary("Get help")]
        public async Task Help()
        {
            StringBuilder output = new StringBuilder();
            foreach (CommandInfo info in (typeof(WeatherUndergroundModule)).GetCustomAttribute<Command>())
            {
            }

                foreach (ModuleInfo module in _services.Modules)
            {
                if(module.Name == "wu")
                {
                    foreach(CommandInfo comamnd in module.Commands)
                    {
                        output.Append($"{comamnd.Summary}");
                    }
                    break;
                }
            }
        }*/
    }
}
