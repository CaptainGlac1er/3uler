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
using System.Collections.Specialized;

namespace Bot3ulerLogic
{
    public class Bot3uler
    {
        DiscordSocketClient client;
        ServerUpdater<string> console;
        ServerUpdater<List<GuildObject>> GuildUpdate;
        GuildConfig GuildConfigInfo;
        WebConnection WebConnect;
        public Bot3uler(ServerUpdater<string> console)
        {
            this.console = console;
            WebConnect = new WebConnection();
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
            if (GuildUpdate != null)
            {
                foreach (SocketGuild guild in client.Guilds)
                {
                    try
                    {
                        List<GuildObject> current = GuildUpdate.GetCurrentData();
                        current = current ?? new List<GuildObject>();
                        GuildObject guildObject = new GuildObject(guild.Name, guild.Id);
                        foreach (SocketTextChannel channel in guild.TextChannels)
                        {
                            guildObject.Channels.Add(new ChannelObject(channel.Name, channel.Id));
                        }
                        current.Add(guildObject);
                        GuildUpdate.UpdateObservers(current);
                    }catch(Exception e)
                    {
                        Debug.WriteLine(e);
                    }
                }
            }
            // console.UpdateObservers($"{guild.Name} channel count {guild.Channels.Count} id {guild.Id}");

        }

        public async Task StartBot()
        {
            DiscordInfo discordInfo = await (new FileData("Config/DiscordConfig.json")).GetObjectFromJson<DiscordInfo>();
            GuildConfigInfo = await GetSavedDiscordConfig();
            var services = await ConfigureServices();

            await services.GetRequiredService<CommandHandler>().ConfigureAsyc();

            await client.LoginAsync(TokenType.Bot, discordInfo.Token);
            await client.StartAsync();
            console.UpdateObservers(Thread.CurrentThread.Name);
        }

        private async Task Log(LogMessage msg)
        {
            await Task.Run(() =>
            {
                Debug.WriteLine(msg.ToString());
                console.UpdateObservers(string.Format("{4} {0} {1} {2} {3}", msg.Source, msg.Exception, msg.Severity, msg.Message, Thread.CurrentThread.Name));
            });
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
            Imgur ImgurConnect = new Imgur(WebConnect);
            await ImgurConnect.LoadConfig(new FileData("Config/ImgurConfig.json"));
            await ImgurConnect.SetupConnection();

            WeatherUnderground WeatherUndergroundConnect = new WeatherUnderground(WebConnect);
            await WeatherUndergroundConnect.LoadConfig(new FileData("Config/WeatherUndergroundConfig.json"));

            Cleverbot CleverbotConnect = new Cleverbot(WebConnect);
            await CleverbotConnect.LoadConfig(new FileData("Config/CleverbotConfig.json"));


            var sc = new ServiceCollection();
            sc.AddSingleton(client);
            sc.AddSingleton(console);
            sc.AddSingleton(ImgurConnect);
            sc.AddSingleton(WeatherUndergroundConnect);
            sc.AddSingleton(CleverbotConnect);
            sc.AddSingleton(GuildConfigInfo);
            sc.AddSingleton<CommandHandler>();
            sc.AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false, ThrowOnError = true }));
            sc.AddSingleton(new TestService("testing: ", console));
            sc.AddSingleton<WeatherUndergroundService>();
            sc.AddSingleton<ImgurService>();
            sc.AddSingleton<CleverbotService>();
            sc.AddSingleton<ScheduleMaker>();
            sc.AddSingleton<TvModeService>();

            var sp = sc.BuildServiceProvider();
            sp.GetService<TestService>();
            sp.GetService<ImgurService>();
            sp.GetService<CleverbotService>();
            sp.GetService<TvModeService>();
            sp.GetService<WeatherUndergroundService>();
            return sp;
        }
        public void ListenForGuildChange(IServerObserver<List<GuildObject>> guildUpdate)
        {
            GuildUpdate = new ServerUpdater<List<GuildObject>>();
            GuildUpdate.AddObserver(guildUpdate);
        }
    }
    class DiscordInfo
    {
        [JsonProperty("token"),JsonRequired()]
        public string Token { get; set; }
           //todo
        [JsonProperty("client_id")]
        public string ClientID { get; set; }
            //todo
        [JsonProperty("secret")]
        public string Secret { get; set; }

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
        private string _Name;
        private ulong _Id;

        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (value != _Name)
                {
                    _Name = value;
                    NotifyPropertyChanged();
                }
            }

        }
        public ulong Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (value != _Id)
                {
                    this._Id = value;
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
