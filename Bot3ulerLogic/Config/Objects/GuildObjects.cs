using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Bot3ulerLogic.Config.Objects
{
    public class UpdateableClasses : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
        protected void Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            storage = value;
            RaisePropertyChanged(propertyName);
        }
    }
    public class Channel : UpdateableClasses
    {
        private bool _ShowCommands = false;
        private List<string> _Commands = new List<string>();
        public Channel()
        {
            Commands = new List<string>();
        }
        public List<string> Commands
        {
            get
            {
                return _Commands;
            }
            set
            {
                Set(ref _Commands, value);
            }
        }

        [JsonIgnore]
        public bool ShowCommands
        {
            get
            {
                return _ShowCommands;
            }
            set
            {
                Debug.WriteLine($"{value} new value");
                Set(ref _ShowCommands, value);
            }
        }
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
            foreach(string value in Guilds[guildID].Channels[channelID].Commands)
            {
                Debug.WriteLine(value);
            }
            return (Guilds.ContainsKey(guildID) && Guilds[guildID].Channels.ContainsKey(channelID)) ? Guilds[guildID].Channels[channelID].Commands : null;
        }
    }
}
