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
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using Bot3ulerLogic.Migrations;
using MySql.Data.Entity;

namespace Bot3ulerLogic
{ 
    public class BotDbContext : DbContext
    {
        public BotDbContext (): base("name=testdatabaseConnection")
        {
            Console.WriteLine("here");
        }
        public DbSet<Guild> Guilds { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Command> Commands { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Channel>()
                .HasMany<Command>(s => s.ChannelCommands)
                .WithMany(c => c.CommandsChannel)
                .Map(cs =>
                {
                    cs.MapLeftKey("ChannelId");
                    cs.MapRightKey("CommandId");
                    cs.ToTable("ChannelCommands");
                });
            /*modelBuilder.Entity<Guild>()
                .HasMany<Channel>(g => g.GuildChannels)
                .WithRequired(c => c.ChannelGuild)
                .Map(cs =>
                {
                    cs.MapKey("GuildId");
                    cs.ToTable("GuildChannels");
                });*/
        }
    }

    public class Bot3uler
    {

        DiscordSocketClient Client;
        ServerUpdater<string> Console;
        ServerUpdater<List<Guild>> GuildUpdate;
        WebConnection WebConnect;
        public Bot3uler()
        {
            WebConnect = new WebConnection();
            Console = new ServerUpdater<string>();
            GuildUpdate = new ServerUpdater<List<Guild>>();
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            Client.Log += Log;
            //client.MessageReceived += MessageInbound;
            Client.Ready += Client_Ready;
        }


        private async Task Client_Ready()
        {
            await Client.SetGameAsync("scrubs like you");
            await Console.UpdateObservers("Bot connected");
            await Console.UpdateObservers("can connect to");
            using (var db = new BotDbContext())
            {
                if (GuildUpdate != null)
                {
                    foreach (var guild in Client.Guilds)
                    {
                        try
                        {
                            var dbguild = db.Guilds.SingleOrDefault(g => g.GuildId == (long) guild.Id);
                            if (dbguild == null)
                            {
                                dbguild = new Guild()
                                {
                                    GuildId = (long) guild.Id,
                                    GuildName = guild.Name
                                };
                                dbguild = db.Guilds.Add(dbguild);
                                await Console.UpdateObservers($"Added guild: {dbguild.GuildName} to db");
                            }
                            else
                            {
                                dbguild.GuildName = guild.Name;
                            };
                            foreach (SocketTextChannel channel in guild.TextChannels)
                            {
                                var dbchannel = dbguild.GuildChannels.SingleOrDefault(c => c.ChannelId == (long) channel.Id);
                                if (dbchannel == null)
                                {
                                    dbchannel = new Channel
                                    {
                                        ChannelId = (long) channel.Id,
                                        Name = channel.Name
                                    };
                                    dbguild.GuildChannels.Add(dbchannel);
                                    await Console.UpdateObservers($"Added channel: {dbchannel.Name} to db");
                                }
                                else
                                {
                                    dbchannel.Name = channel.Name;
                                }
                                await Console.UpdateObservers($"Added {dbguild.GuildName} > {dbchannel.Name} has {dbchannel.ChannelCommands.Count} commands");
                            }
                            await db.SaveChangesAsync();
                            await GuildUpdate.UpdateObservers(db.Guilds.ToList());
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e);
                        }
                    }
                }
            }
            //var data = await (new FileData("Config/GuildConfig.json")).GetObjectFromJson<BotConfig>();
                // console.UpdateObservers($"{guild.Name} channel count {guild.Channels.Count} id {guild.Id}");

            }

        public async Task StartBot()
        {
            DiscordInfo discordInfo = await (new FileData("Config/DiscordConfig.json")).GetObjectFromJson<DiscordInfo>();
            var services = await ConfigureServices();

            await services.GetRequiredService<CommandHandler>().ConfigureAsyc();

            await Client.LoginAsync(TokenType.Bot, discordInfo.Token);
            await Client.StartAsync();
            await Console.UpdateObservers(Thread.CurrentThread.Name);
        }

        private async Task Log(LogMessage msg)
        {
            Debug.WriteLine(msg.ToString());
            await Console.UpdateObservers(string.Format("{4} {0} {1} {2} {3}", msg.Source, msg.Exception, msg.Severity, msg.Message, Thread.CurrentThread.Name));
        }

        public async Task<string> GetStatus()
        {
            StringBuilder output = new StringBuilder();
            IReadOnlyCollection<RestConnection> connections = await Client.GetConnectionsAsync();
            foreach (RestConnection r in connections)
                output.Append(string.Format("{0} {1} {2}\n", r.Id, r.Name, r.Type));
            return output.ToString() + Client.ConnectionState.ToString();
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
            sc.AddSingleton(Client);
            sc.AddSingleton(Console);
            sc.AddSingleton(ImgurConnect);
            sc.AddSingleton(WeatherUndergroundConnect);
            sc.AddSingleton(CleverbotConnect);
            sc.AddSingleton<CommandHandler>();
            sc.AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false, ThrowOnError = true }));
            sc.AddSingleton(new TestService("testing: ", Console));
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
        public void ListenForGuildChange(IServerObserver<List<Guild>> guildUpdate)
        {
            GuildUpdate.AddObserver(guildUpdate);
        }
        public void ListenForConsoleUpdate(IServerObserver<string> console)
        {
            Console.AddObserver(console);
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
    
}
