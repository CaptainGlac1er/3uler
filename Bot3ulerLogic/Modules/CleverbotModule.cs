using Bot3ulerLogic.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot3ulerLogic.Modules
{
    [Group("chat")]
    public class CleverbotModule : ModuleBase<SocketCommandContext>
    {
        public CleverbotService Service;
        public CleverbotModule(CleverbotService service)
        {
            Service = service;
        }
        [Command(""),Summary("Ask cleverbot")]
        public async Task Chat([Remainder, Summary("query")] string query)
        {
            await ReplyAsync(await Service.Ask(query));
        }
    }
}
