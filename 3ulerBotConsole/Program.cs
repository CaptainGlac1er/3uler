using Bot3ulerLogic;
using _3ulerBotShared.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _3ulerBotConsole
{
    class Program: IServerObserver<string>
    {
        static ManualResetEvent mre = new ManualResetEvent(false);
        public event EventHandler Ready, Shutdown;

        static void Main(string[] args)
        {
            new Program();
            mre.WaitOne();
        }

        public Program()
        {
            DiscordBot.bot.ListenForConsoleUpdate(this);
            Ready += StartBot;
            Shutdown += ShutdownBot;
            Ready(this,EventArgs.Empty);
        }

        private async void StartBot(object sender, EventArgs e)
        {
            await DiscordBot.bot.StartBot();
        }
        private void ShutdownBot(object sender, EventArgs e)
        {
            Program.mre.Set();
        }


        public async Task BotUpdate(string update)
        {
            await Task.Run(() =>
            {
                Console.Out.WriteLine(update);
            });
        }
    }
}
