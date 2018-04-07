using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Conventions;
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

    public class Command : UpdateableClasses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public long CommandId { get; set; }
        public string CommandString { get; set; }
        public virtual ICollection<Channel> CommandsChannel { get; set; }
    }

    /*public class ChannelCommand
    {
        public int Id { get; set; }

        public long CommandId { get; set; }
        public virtual Command Command { get; set; }

        public long ChannelId { get; set; }
        public virtual Channel Channel { get; set; }
    }*/

    public class Channel : UpdateableClasses
    {
        [Key]
        public long ChannelId { get; set; }
        //public virtual ICollection<ChannelCommand> Commands { get; set; }
        //public ICollection<ChannelCommand> ChannelCommands { get; set; }
        public virtual ICollection<Command> ChannelCommands { get; set; }
        public virtual Guild ChannelGuild { get; set; }
        [NotMapped]
        private bool _ShowCommands = false;
        [NotMapped]
        private List<string> _Commands = new List<string>();
        public Channel()
        {
            Commands = new List<string>();
        }
        [NotMapped]
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
        [NotMapped]
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
        [Key]
        public long GuildId { get; set; }
        public string GuildName { get; set; }
        public virtual ICollection<Channel> GuildChannels { get; set; }
        [NotMapped]
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
