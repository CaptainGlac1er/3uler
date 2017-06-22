using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot3ulerLogic.Config.Objects
{
    public class Channel
    {
        public Channel()
        {
            Commands = new List<string>();
        }
        public List<string> Commands { get; set; }
    }

    public class Guild
    {
        public Guild()
        {
            Channels = new Dictionary<ulong, Channel>();
        }
        public Dictionary<ulong, Channel> Channels { get; set; }
    }

    public class GuildConfig
    {
        public GuildConfig()
        {
            Guilds = new Dictionary<ulong, Guild>();
        }
        public Dictionary<ulong, Guild> Guilds { get; set; }
        public List<string> GetCommandsOfGuildChannel(ulong guildID, ulong channelID)
        {
            return (Guilds.ContainsKey(guildID) && Guilds[guildID].Channels.ContainsKey(channelID)) ? Guilds[guildID].Channels[channelID].Commands : null;
        }
    }
}
