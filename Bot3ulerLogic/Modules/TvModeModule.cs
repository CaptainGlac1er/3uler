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
    [Group("tvroom")]
    public class TvModeModule : GWCModule
    {
        public TvModeModule(TvModeService service)
        {
            Service = service;
        }
        [Command("start")]
        public async Task StartTvRoom()
        {
            
            SocketVoiceChannel voiceChannel = (Context.User as IVoiceState).VoiceChannel as SocketVoiceChannel;
            if (voiceChannel != null)
            {
                (Service as TvModeService).StartTvRoom(voiceChannel, new List<SocketUser>() { Context.User });
            }
            else
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} You're not in a voice channel");
            }
        }
        [Command("list")]
        public async Task List()
        {
            StringBuilder sb = new StringBuilder();
            foreach( string s in await (Service as TvModeService).GetListOfRooms())
            {
                sb.Append(s);
            }
            await ReplyAsync(sb.ToString());
        }
        [Command("getusers")]
        public async Task GetUsers()
        {
            SocketVoiceChannel voiceChannel = (Context.User as IVoiceState).VoiceChannel as SocketVoiceChannel;
            await ReplyAsync(await (Service as TvModeService).GetUserList(voiceChannel));
        }
    }
}
