using Bot3ulerLogic.Modules.Queue;
using Discord.WebSocket;
using GWC.Imgur;
using GWC.Imgur.DataTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot3ulerLogic.Services
{
    public class ImgurService : APIConnectionScheduled
    {
        Imgur ImgurConnection;
        public ImgurService(Imgur imgurConnection, ServerUpdater<string> console, DiscordSocketClient client, ScheduleMaker scheduleMaker) : base(console, client, scheduleMaker)
        {
            ImgurConnection = imgurConnection;
            CommandName = "imgur";
        }
        public async Task<ImgurPicture> GetImage(string query)
        {
            
            ImgurAlbum album = await ImgurConnection.QuerySearch(query);
            if(album == null)
            {
                return null;
            }
            else
            {
                return album.data[(new Random()).Next(album.data.Count)];
            }
        }
        public async Task<string> StartSchedule(string query, ISocketMessageChannel channelRequested, int delay)
        {
            return  await ScheduleMaker.AddSchedule(new ImgurSchedule(CommandName, query, channelRequested, delay, GetImage)) ? "Schedule Made Successful" : "Schedule Failed";
        }
    }
}
