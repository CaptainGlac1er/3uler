using Bot3ulerLogic.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot3ulerLogic.Modules
{
    public class GWCModule : ModuleBase<SocketCommandContext>
    {
        protected GWCService Service;
        protected SocketVoiceChannel GetVoiceChannel()
        {
            return (Context.User as IVoiceState).VoiceChannel as SocketVoiceChannel;
        }
    }
}
