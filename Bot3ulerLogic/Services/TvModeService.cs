using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Discord;

namespace Bot3ulerLogic.Services
{
    public class TvModeService : APIconnection
    {
        private class TVroom
        {
            public TVroom(SocketVoiceChannel TvChannel, List<SocketUser> Admins, ServerUpdater<string> console)
            {
                Console = console;
                _TvChannel = TvChannel;
                this.Admins = Admins;
                foreach (SocketUser user in Admins)
                    AddAdmins(user);
            }
            public  SocketVoiceChannel TvChannel
            {
                get
                {
                    return _TvChannel;
                }
            }
            private SocketVoiceChannel _TvChannel;
            private List<SocketUser> Allowed = new List<SocketUser>();
            private List<SocketUser> Admins = new List<SocketUser>();
            private List<SocketUser> Muted = new List<SocketUser>();
            private ServerUpdater<string> Console;

            private Dictionary<ulong, bool> SaveMuted = new Dictionary<ulong, bool>();

            private async void AddAdmins(SocketUser user)
            {
                SaveMuted.Add(user.Id, (user as SocketGuildUser).IsMuted);
                await (user as SocketGuildUser).ModifyAsync(x => x.Mute = false);
                Admins.Add(user);
            }
            private async Task AddGuest(SocketUser user)
            {
                SaveMuted.Add(user.Id, (user as SocketGuildUser).IsMuted);
                await (user as SocketGuildUser).ModifyAsync(x => x.Mute = true);
                Muted.Add(user);
            }
            private async void AddAllowed(SocketUser user)
            {
                SaveMuted.Add(user.Id, (user as SocketGuildUser).IsMuted);
                await (user as SocketGuildUser).ModifyAsync(x => x.Mute = false);
                Allowed.Add(user);
            }


            public async Task Add(SocketUser user)
            {
                if(!Allowed.Contains(user) && !Admins.Contains(user))
                {
                    Console.UpdateObservers("here");
                    await AddGuest(user);
                }
            }
            public async Task Remove(SocketUser user)
            {
                await (user as SocketGuildUser).ModifyAsync(x => x.Mute = SaveMuted[user.Id]);
                if (Muted.Contains(user))
                    Muted.Remove(user);
                SaveMuted.Remove(user.Id);
            }
            public async Task Modify(SocketUser user)
            {
                if (Muted.Contains(user) && !(user as SocketGuildUser).IsMuted)
                {
                    await Remove(user);
                    AddAllowed(user);
                    Console.UpdateObservers($"{user.Username} is now allowed");
                }
            }
            public async Task<int> GetRoomCount()
            {
                return await Task.Run<int>(() =>
                {
                    return Admins.Count + Allowed.Count + Muted.Count;
                });
            }

        }
        Dictionary<string, TVroom> RunningTVRooms = new Dictionary<string, TVroom>();
        public TvModeService(ServerUpdater<string> console, DiscordSocketClient client) : base(console, client)
        {
            client.UserVoiceStateUpdated += ProcessVoiceChannelChange;
            CommandName = "tvroom";
        }
        
        private async Task ProcessVoiceChannelChange(SocketUser user, SocketVoiceState oldChannel, SocketVoiceState newChannel)
        {
            Console.UpdateObservers($"{user == null} {oldChannel} {newChannel}");
            if (oldChannel.VoiceChannel.Id != newChannel.VoiceChannel.Id)
            {
                TVroom roomToUse = await GetRoom(newChannel.VoiceChannel);
                TVroom roomLeft = await GetRoom(oldChannel.VoiceChannel);
                if (roomToUse != null && roomLeft == roomToUse)
                {
                    await roomToUse.Modify(user);
                }
                else
                {
                    if (roomToUse != null)
                    {
                        await roomToUse.Add(user);
                    }
                    if (roomLeft != null)
                    {
                        await roomLeft.Remove(user);
                        if((await roomLeft.GetRoomCount()) == 0)
                        {
                            await RemoveRoom(roomLeft.TvChannel);
                        }
                    }
                }
            }
        }

        private async Task<TVroom> GetRoom(SocketVoiceChannel voicechannel)
        {
            return await Task.Run<TVroom>(() =>
            {
                if (voicechannel != null && RunningTVRooms.ContainsKey($"{voicechannel.Id}"))
                {
                    Console.UpdateObservers($"getting {voicechannel.Name}");
                    return RunningTVRooms[$"{voicechannel.Id}"];
                }
                return null;
            });
        }
        private async Task<bool> RemoveRoom(SocketVoiceChannel voicechannel)
        {
            return await Task.Run<bool>(() =>
            {
                if (voicechannel != null && RunningTVRooms.ContainsKey($"{voicechannel.Id}"))
                {
                    RunningTVRooms.Remove($"{voicechannel.Id}");
                    return true;
                }
                return false;
            });
        }

        public async void StartTvRoom(SocketVoiceChannel voicechannel, List<SocketUser> admins)
        {
            await Task.Run(() =>
            {
                TVroom room = new TVroom(voicechannel, admins, Console);
                RunningTVRooms.Add($"{voicechannel.Id}", room);
                Console.UpdateObservers(RunningTVRooms.Values.Count.ToString() + " rooms running");
            });
        }
        public async Task<List<string>> GetListOfRooms()
        {
            return await Task.Run<List<string>>(() =>
            {
                return RunningTVRooms.Keys.ToList();
            });
        }
    }
}
