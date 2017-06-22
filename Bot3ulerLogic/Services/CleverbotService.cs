using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using GWC.Cleverbot;

namespace Bot3ulerLogic.Services
{
    public class CleverbotService : APIconnection
    {
        Cleverbot CleverbotConnect;
        public CleverbotService(Cleverbot cleverbot, ServerUpdater<string> console, DiscordSocketClient client) : base(console, client)
        {
            CleverbotConnect = cleverbot;
            CommandName = "chat";
        }
        public async Task<string> Ask(string query)
        {
            return await CleverbotConnect.GetReply(query);
        }
    }
}
