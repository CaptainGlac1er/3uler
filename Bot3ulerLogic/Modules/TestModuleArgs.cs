using Bot3ulerLogic.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot3ulerLogic.Modules
{
    public class TestModuleArgs : ModuleBase<SocketCommandContext>
    {
        string premessage;
        public TestModuleArgs(TestService stuff)
        {
            this.premessage = stuff.GetPrefix();
            stuff.UpdateConsole("module created");
        }
        [Command("echo"), Summary("echos a message")]
        public async Task Echo([Remainder, Summary("text to echo")] string echo)
        {
            await ReplyAsync(premessage + " " + echo);
        }
    }
}
