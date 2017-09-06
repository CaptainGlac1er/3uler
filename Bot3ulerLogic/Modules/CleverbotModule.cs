using Bot3ulerLogic.Preconditions;
using Bot3ulerLogic.Services;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot3ulerLogic.Modules
{
    [Group("chat"), CheckSource("chat")]
    public class CleverbotModule : GWCModule
    {
        public CleverbotModule(CleverbotService service)
        {
            Service = service;
        }
        [Command(""),Summary("Ask cleverbot")]
        public async Task Chat([Remainder, Summary("query")] string query)
        {
            await ReplyAsync(await (Service as CleverbotService).Ask(query));
        }
    }
}
