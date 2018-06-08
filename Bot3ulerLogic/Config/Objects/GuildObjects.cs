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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ChannelId { get; set; }
        //public virtual ICollection<ChannelCommand> Commands { get; set; }
        //public ICollection<ChannelCommand> ChannelCommands { get; set; }
        public virtual ICollection<Command> ChannelCommands
        {
            get;
            set;
        }

        [NotMapped]
        public ICollection<Command> GetChannelCommands
        {
            get
            {
                using (var db = new BotDbContext())
                {
                    var commands = db.Channels.Find(ChannelId).ChannelCommands;
                    return commands ?? new List<Command>();
                }
            }
        }
        public virtual Guild ChannelGuild { get; set; }
        [NotMapped]
        private bool _ShowCommands = false;
        [NotMapped]
        private List<string> _Commands = new List<string>();

        [NotMapped] private string _Name;
        [NotMapped]
        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }
        public Channel()
        {
            Commands = new List<string>();
        }
        [NotMapped]
        public List<string> Commands
        {
            get => _Commands;
            set => Set(ref _Commands, value);
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

        public async void RemoveCommand(long commandId)
        {
            using (var db = new BotDbContext())
            {
                db.Channels.Find(ChannelId)?.ChannelCommands.Remove(db.Commands.Find(commandId));
                await db.SaveChangesAsync();
                RaisePropertyChanged("GetChannelCommands");
                RaisePropertyChanged("AvailableCommands");
            }
        }
        public async void AddCommand(long commandId)
        {
            using (var db = new BotDbContext())
            {
                var channel = await db.Channels.FindAsync(ChannelId);
                channel?.ChannelCommands.Add(db.Commands.Find(commandId));
                await db.SaveChangesAsync();
                RaisePropertyChanged("GetChannelCommands");
                RaisePropertyChanged("AvailableCommands");
            }
        }
        [NotMapped]
        public List<Command> AvailableCommands
        {
            get
            {
                using (var db = new BotDbContext())
                {
                    var commands = db.Channels.Find(ChannelId)?.GetChannelCommands.ToList();
                    var leftover = db.Commands.ToList();
                    leftover.RemoveAll(
                        x => commands != null && commands.Exists(y => y.CommandString == x.CommandString));
                    return leftover.ToList();
                }
            }
        }
    }

    public class Guild : UpdateableClasses
    {
        public Guild()
        {
            Channels = new Dictionary<ulong, Channel>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long GuildId { get; set; }
        public string GuildName { get; set; }
        public virtual ICollection<Channel> GuildChannels { get; set; }
        [NotMapped]
        public Dictionary<ulong, Channel> Channels { get; set; }
        
        [NotMapped]
        private bool _ShowChannels = false;
        [NotMapped]
        public bool ShowChannels
        {
            get
            {
                return _ShowChannels;
            }
            set
            {
                Debug.WriteLine($"{value} new value");
                Set(ref _ShowChannels, value);
            }
        }
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
