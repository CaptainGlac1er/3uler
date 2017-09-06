using Bot3ulerLogic.Modules.Queue;
using Bot3ulerLogic.Preconditions;
using Bot3ulerLogic.Services;
using Discord.Commands;
using GWC.Imgur.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot3ulerLogic.Modules
{
    [Group("imgur"), CheckSource("imgur")]
    public class ImgurModule : ModuleBase<SocketCommandContext>
    {
        ImgurService Imgur;
        public ImgurModule(ImgurService service)
        {
            Imgur = service;
            Imgur.UpdateConsole("imgur module created");
        }
        [Command("get"), Summary("Get image from imgur")]
        public async Task Echo([Remainder, Summary("query")] string query)
        {
            ImgurPicture picture = await Imgur.GetImage(query);
            
            if (picture != null)
                await ReplyAsync($"{picture.title ?? ""}\n{picture.link}");
            else
                await ReplyAsync("Slow the meme train");
        }
        [Command("schedule"), Summary("Schedule imgur stream")]
        public async Task Schedule([Summary("delay")] int delay, [Remainder, Summary("query")] string query)
        {
            await ReplyAsync(await Imgur.StartSchedule(query, Context.Channel, delay));
        }
        [Command("stopschedule"), Summary("Stop schedule imgur stream")]
        public async Task ScheduleStop([Remainder, Summary("query")] string query)
        {
            await ReplyAsync(await Imgur.StopSchedule(query));
        }
        [Command("getschedules"), Summary("Get all Imgur schedules")]
        public async Task GetSchedules()
        {
            await ReplyAsync(await Imgur.GetAllSchedules());
        }

    }
}
