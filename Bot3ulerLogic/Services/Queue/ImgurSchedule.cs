using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using GWC.Imgur.DataTypes;

namespace Bot3ulerLogic.Modules.Queue
{
    class ImgurSchedule : Schedule
    {
        private Func<string, Task<ImgurPicture>> FunctionToUse;
        public ImgurSchedule(string command, string query, ISocketMessageChannel channelRequested, int delay, Func<string, Task<ImgurPicture>> functionToUse) : base (command, query, channelRequested, delay)
        {
            FunctionToUse = functionToUse;
        }

        public override async void Action(object stateInfo)
        {
            ImgurPicture picture = await FunctionToUse(Query);
            if (picture != null)
                await ChannelRequested.SendMessageAsync($"{picture.title ?? ""}\n{picture.link}");
            else
                await ChannelRequested.SendMessageAsync("Slow the meme train");
        }
        
    }
}
