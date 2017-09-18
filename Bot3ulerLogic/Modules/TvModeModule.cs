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
            SocketVoiceChannel voiceChannel = GetVoiceChannel();
            if (voiceChannel != null)
            {
                await (Service as TvModeService).StartTvRoom(voiceChannel, new List<SocketUser>() { Context.User });
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
            SocketVoiceChannel voiceChannel = GetVoiceChannel();
            if (voiceChannel != null)
            {
                await ReplyAsync(await (Service as TvModeService).GetUserList(voiceChannel));
            }
            else
            {
                await ReplyAsync($"{Context.User.Mention} you are not in a voice channel");
            }
        }
        [Command("stop")]
        public async Task StopRoom()
        {
            SocketVoiceChannel voiceChannel = GetVoiceChannel();
            if (voiceChannel != null)
            {
                if(await (Service as TvModeService).RemoveRoom(voiceChannel, Context.User))
                {
                    await Context.Channel.SendMessageAsync($"stopped successfully");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"stop failed");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"{Context.User.Mention} You're not in a voice channel");
            }
        }
    }
}
