using Discord.Commands;
using Discord.WebSocket;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using Bot3ulerLogic.Modules;

namespace Bot3ulerLogic
{

    class CommandHandler
    {
        private readonly IServiceProvider _provider;

        private readonly CommandService _commands;

        private readonly DiscordSocketClient _client;
        private readonly ServerUpdater<string> _updater;
        public CommandHandler(IServiceProvider provider)
        {
            _provider = provider;
            _client = _provider.GetService<DiscordSocketClient>();
            _client.MessageReceived += ProcessCommandAsync;
            _commands = _provider.GetService<CommandService>();
            _updater = _provider.GetService<ServerUpdater<string>>();
            _updater.UpdateObservers("commands setup");
        }
        public async Task ConfigureAsyc()
        {
            Assembly a = Assembly.GetExecutingAssembly();
            await _commands.AddModulesAsync(a);
            _updater.UpdateObservers("ConfigureAsyc run");
        }
        private async Task ProcessCommandAsync(SocketMessage pMsg)
        {
            var message = pMsg as SocketUserMessage;
            _updater.UpdateObservers(message.Content);

            if (message == null) return;
            if (message.Content.Length > 0 && !message.Content.StartsWith("!")) return;
            
            int argPos = 1;
            
            var context = new SocketCommandContext(_client, message);
            _updater.UpdateObservers((await _commands.ExecuteAsync(context, argPos, _provider)).ToString());
        }
        /*public async Task<string> GetHelp(ICommandContext context, string command)
        {
            SearchResult result = _commands.Search(context, command);
            if (result.IsSuccess) {
                foreach (CommandMatch match in result.Commands)
                {

                }
            }
        }*/
    }
}
