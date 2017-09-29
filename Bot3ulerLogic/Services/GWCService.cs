using Bot3ulerLogic.Modules.Queue;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot3ulerLogic.Services
{
    public abstract class GWCService
    {
        protected ServerUpdater<string> Console;
        protected DiscordSocketClient Client;
        public string CommandName { get; set; }
        public GWCService(ServerUpdater<string> console, DiscordSocketClient client)
        {
            Console = console;
            Client = client;
        }
        public async Task UpdateConsole(string message)
        {
            await Console.UpdateObservers(message);
        }
    }
    public abstract class APIconnection : GWCService
    {
        public APIconnection(ServerUpdater<string> console, DiscordSocketClient client): base(console,client)
        {
        }
    }
    public abstract class APIConnectionScheduled : APIconnection
    {
        protected ScheduleMaker ScheduleMaker;
        public APIConnectionScheduled(ServerUpdater<string> console, DiscordSocketClient client, ScheduleMaker scheduleMaker) : base(console, client)
        {
            ScheduleMaker = scheduleMaker;
        }
        public async Task<string> StopSchedule(string query)
        {
            return await ScheduleMaker.StopSchedule(CommandName, query);
        }
        public async Task<string> GetAllSchedules()
        {
            return await ScheduleMaker.GetAllSchedules(CommandName);
        }
        public async Task<string> StopAllSchedules()
        {
            return await ScheduleMaker.StopAllSchedules();
        }
    }
}
