using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Bot3ulerLogic.Modules;
using Bot3ulerLogic.Services;
using Bot3ulerLogic.Modules.Queue;
using Bot3ulerLogic.Config.Objects;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GWC.FileSystem;
using GWC.WebConnect;
using GWC.Imgur;
using GWC.WeatherUnderground;
using GWC.Cleverbot;
using Bot3ulerLogic.Preconditions;

namespace Bot3ulerLogic
{
    public class Bot3uler
    {
        DiscordSocketClient client;
        ServerUpdater<string> console;
        ServerUpdater<List<GuildObject>> GuildUpdate;
        GuildConfig GuildConfigInfo;
        public Bot3uler(ServerUpdater<string> console)
        {
            this.console = console;
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            client.Log += Log;
            //client.MessageReceived += MessageInbound;
            client.Ready += Client_Ready;
        }
        

        private async Task Client_Ready()
        {
            await client.SetGameAsync("scrubs like you");
            console.UpdateObservers("Bot connected");
            console.UpdateObservers("can connect to");
            if(GuildUpdate != null)
            {
                foreach(SocketGuild guild in client.Guilds)
                {
                    List<GuildObject> current = GuildUpdate.GetCurrentData();
                    current = current ?? new List<GuildObject>();
                    GuildObject guildObject = new GuildObject(guild.Name, guild.Id);
                    foreach(SocketTextChannel channel in guild.TextChannels)
                    {
                        guildObject.Channels.Add(new ChannelObject(channel.Name, channel.Id));
                    }
                    current.Add(guildObject);
                    GuildUpdate.UpdateObservers(current);
                }
            }
               // console.UpdateObservers($"{guild.Name} channel count {guild.Channels.Count} id {guild.Id}");
            
        }

        public async Task StartBot()
        {
            string token = (await (new FileData("Config/DiscordConfig.json")).GetObjectFromJson<DiscordInfo>()).Token;

            GuildConfigInfo = await GetSavedDiscordConfig();
            var services = await ConfigureServices();

            await services.GetRequiredService<CommandHandler>().ConfigureAsyc();

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            console.UpdateObservers(Thread.CurrentThread.Name);

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Debug.WriteLine(msg.ToString());
            console.UpdateObservers(string.Format("{0} {1} {2} {3}", msg.Source, msg.Exception, msg.Severity, msg.Message));
            console.UpdateObservers(Thread.CurrentThread.Name);
            return Task.CompletedTask;
        }
        
        public async Task<string> GetStatus()
        {
            StringBuilder output = new StringBuilder();
            IReadOnlyCollection<RestConnection> connections = await client.GetConnectionsAsync();
            foreach (RestConnection r in connections)
                output.Append(string.Format("{0} {1} {2}\n", r.Id, r.Name, r.Type));
            return output.ToString() + client.ConnectionState.ToString(); 
        }
        public async Task<GuildConfig> GetSavedDiscordConfig()
        {
            GuildConfig config = await (new FileData("Config/GuildConfig.json")).GetObjectFromJson<GuildConfig>();
            return config;
        }
        public async Task<IServiceProvider> ConfigureServices()
        {
            WebConnection WebConnect = new WebConnection();
            Imgur ImgurConnect = new Imgur(WebConnect);
            await ImgurConnect.LoadConfig(new FileData("Config/ImgurConfig.json"));
            await ImgurConnect.SetupConnection();

            WeatherUnderground WeatherUndergroundConnect = new WeatherUnderground(WebConnect);
            await WeatherUndergroundConnect.LoadConfig(new FileData("Config/WeatherUndergroundConfig.json"));

            Cleverbot CleverbotConnect = new Cleverbot(WebConnect);
            await CleverbotConnect.LoadConfig(new FileData("Config/CleverbotConfig.json"));


            var test = new ServiceCollection();
            test.AddSingleton(client);
            test.AddSingleton(console);
            test.AddSingleton(ImgurConnect);
            test.AddSingleton(WeatherUndergroundConnect);
            test.AddSingleton(CleverbotConnect);
            test.AddSingleton(GuildConfigInfo);
            test.AddSingleton<CommandHandler>();
            test.AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false, ThrowOnError = true }));
            test.AddSingleton(new TestService("testing: ", console));
            test.AddSingleton<WeatherUndergroundService>();
            test.AddSingleton<ImgurService>();
            test.AddSingleton<CleverbotService>();
            test.AddSingleton<ScheduleMaker>();
            test.AddSingleton<TvModeService>();

            var provider = test.BuildServiceProvider();
            provider.GetService<TestService>();
            provider.GetService<ImgurService>();
            provider.GetService<CleverbotService>();
            provider.GetService<TvModeService>();
            provider.GetService<WeatherUndergroundService>();
             return provider;
        }
        public void ListenForGuildChange(IServerObserver<List<GuildObject>> guildUpdate)
        {
            this.GuildUpdate = new ServerUpdater<List<GuildObject>>();
            GuildUpdate.AddObserver(guildUpdate);
        }
    }
    class DiscordInfo
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
    public class GuildObject : INotifyPropertyChanged
    {
        public GuildObject(string name, ulong id)
        {
            Name = name;
            Id = id;
            channels = new List<ChannelObject>();
        }
        private bool _ShowChannels = false;
        public string name;
        public ulong id;
        public List<ChannelObject> channels;

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                if (value != this.name)
                {
                    this.name = value;
                    NotifyPropertyChanged();
                }
            }

        }
        public ulong Id
        {
            get
            {
                return this.id;
            }
            set
            {
                if (value != this.id)
                {
                    this.id = value;
                    NotifyPropertyChanged();
                }
            }

        }
        [JsonIgnore]
        public bool ShowChannels
        {
            get
            {
                return _ShowChannels;
            }
            set
            {
                if (value != _ShowChannels)
                {
                    _ShowChannels = value;
                }
                NotifyPropertyChanged();
            }
        }
        public List<ChannelObject> Channels
        {
            get
            {
                return this.channels;
            }
            set
            {
                if (value != this.channels)
                {
                    this.channels = value;
                    NotifyPropertyChanged();
                }
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ChannelObject : INotifyPropertyChanged
    {
        public ChannelObject(string name, ulong id)
        {
            Name = name;
            Id = id;
        }
        private bool _ShowCommands = false;
        private string name;
        private ulong id;
        private bool vis;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != this.name)
                {
                    name = value;
                    NotifyPropertyChanged();
                }
            }

        }
        public ulong Id
        {
            get
            {
                return this.id;
            }
            set
            {
                if (value != this.id)
                {
                    this.id = value;
                    NotifyPropertyChanged();
                }
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
                if (value != _ShowCommands)
                {
                    _ShowCommands = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
