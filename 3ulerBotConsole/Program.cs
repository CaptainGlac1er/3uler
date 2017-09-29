using Bot3ulerLogic;
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
        Bot3uler bot;
        static ManualResetEvent mre = new ManualResetEvent(false);
        public event EventHandler Ready, Shutdown;

        static void Main(string[] args)
        {
            new Program();
            mre.WaitOne();
        }

        public Program()
        {
            bot = new Bot3uler();
            bot.ListenForConsoleUpdate(this);
            Ready += StartBot;
            Shutdown += ShutdownBot;
            Ready(this,EventArgs.Empty);
        }

        private async void StartBot(object sender, EventArgs e)
        {
            await bot.StartBot();
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
