using _3ulerBotServer.Models;
using Bot3ulerLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3ulerBotServer.ViewModel
{
    class MainWindowViewModel
    {
        public ServerUpdater<string> ConsoleUpdater;
        public MainWindowViewModel()
        {
            ConsoleUpdater = new ServerUpdater<string>();
        }
        public void AddConsole(IServerObserver<string> console)
        {
            ConsoleUpdater.AddObserver(console);
        }
        public void AddBot()
        {
            DiscordBot.bot = new Bot3uler(ConsoleUpdater);
        }
        public void AddGuildList(IServerObserver<List<GuildObject>> guilds)
        {
            DiscordBot.bot.ListenForGuildChange(guilds);
        }
        public Bot3uler GetBot()
        {
            return DiscordBot.bot;
        }
    }
}
