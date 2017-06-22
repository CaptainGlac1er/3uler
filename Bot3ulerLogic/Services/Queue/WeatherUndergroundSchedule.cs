using Bot3ulerLogic.Modules.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Bot3ulerLogic.Services.Queue
{
    class WeatherUndergroundSchedule : Schedule
    {
        Func<string, Task<string>> FunctionToUse;
        public WeatherUndergroundSchedule(string command, string query, ISocketMessageChannel channelRequested, int delay, Func<string, Task<string>> functionToUse) : base(command, query, channelRequested, delay)
        {
            FunctionToUse = functionToUse;
        }

        public override async void Action(object stateInfo)
        {
            await ChannelRequested.SendMessageAsync(await FunctionToUse(Query));
        }
    }
}
