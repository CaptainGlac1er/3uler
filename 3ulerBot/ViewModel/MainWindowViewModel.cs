using _3ulerBotShared.Models;
using Bot3ulerLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot3ulerLogic.Config.Objects;

namespace _3ulerBotServer.ViewModel
{
    public class MainWindowViewModel
    {
        public void AddConsole(IServerObserver<string> console)
        {
            DiscordBot.bot.ListenForConsoleUpdate(console);
        }
        public void AddGuildList(IServerObserver<List<Guild>> guilds)
        {
            DiscordBot.bot.ListenForGuildChange(guilds);
        }
        public Bot3uler GetBot()
        {
            return DiscordBot.bot;
        }
    }
}
