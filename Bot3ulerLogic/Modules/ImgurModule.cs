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
    public class ImgurModule : GWCModule
    {
        public ImgurModule(ImgurService service)
        {
            Service = service;
        }
        [Command("get"), Summary("Get image from imgur")]
        public async Task Echo([Remainder, Summary("query")] string query)
        {
            ImgurPicture picture = await (Service as ImgurService).GetImage(query);
            
            if (picture != null)
                await ReplyAsync($"{picture.title ?? ""}\n{picture.link}");
            else
                await ReplyAsync("Slow the meme train");
        }
        [Command("schedule"), Summary("Schedule imgur stream")]
        public async Task Schedule([Summary("delay")] int delay, [Remainder, Summary("query")] string query)
        {
            await ReplyAsync(await (Service as ImgurService).StartSchedule(query, Context.Channel, delay));
        }
        [Command("stopschedule"), Summary("Stop schedule imgur stream")]
        public async Task ScheduleStop([Remainder, Summary("query")] string query)
        {
            await ReplyAsync(await (Service as ImgurService).StopSchedule(query));
        }
        [Command("getschedules"), Summary("Get all Imgur schedules")]
        public async Task GetSchedules()
        {
            await ReplyAsync(await (Service as ImgurService).GetAllSchedules());
        }
        [Command("stopschedules"), Summary("Stop schedule imgur stream")]
        public async Task SchedulesStop()
        {
            await ReplyAsync(await (Service as ImgurService).StopAllSchedules());
        }

    }
}
