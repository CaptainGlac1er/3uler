using Bot3ulerLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3ulerBotConsole
{
    class Program: IServerObserver<string>
    {
        ServerUpdater<string> consoleUpdater;
        Bot3uler bot;
        public event EventHandler Ready;

        static void Main(string[] args)
        {
            new Program();
            Console.Read();
        }

        public Program()
        {
            consoleUpdater = new ServerUpdater<string>();
            consoleUpdater.AddObserver(this);
            bot = new Bot3uler(consoleUpdater);
            Ready += StartBot;
            Ready(this,EventArgs.Empty);
        }

        private async void StartBot(object sender, EventArgs e)
        {
            await bot.StartBot();
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
