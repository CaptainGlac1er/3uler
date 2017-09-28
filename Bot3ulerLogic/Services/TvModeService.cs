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
    public class TvModeService : GWCService
    {
        private class TVroom
        {
            private DateTime started;
            public TVroom(SocketVoiceChannel TvChannel, ServerUpdater<string> console)
            {
                Console = console;
                _TvChannel = TvChannel;
                started = DateTime.Now;
            }
            public  SocketVoiceChannel TvChannel
            {
                get
                {
                    return _TvChannel;
                }
            }
            public List<SocketUser> Admins
            {
                get
                {
                    return _Admins;
                }
            }
            public List<SocketUser> Allowed
            {
                get
                {
                    return _Allowed;
                }
            }
            public List<SocketUser> Muted
            {
                get
                {
                    return _Muted;
                }
            }
            private SocketVoiceChannel _TvChannel;
            private List<SocketUser> _Allowed = new List<SocketUser>();
            private List<SocketUser> _Admins = new List<SocketUser>();
            private List<SocketUser> _Muted = new List<SocketUser>();
            private ServerUpdater<string> Console;

            private Dictionary<ulong, bool> SaveMuted = new Dictionary<ulong, bool>();

            public async Task AddAdmins(SocketUser user)
            {
                SaveMuted.Add(user.Id, (user as SocketGuildUser).IsMuted);
                await (user as SocketGuildUser).ModifyAsync(x => x.Mute = false);
                _Admins.Add(user);
            }
            private async Task AddGuest(SocketUser user)
            {
                SaveMuted.Add(user.Id, (user as SocketGuildUser).IsMuted);
                await (user as SocketGuildUser).ModifyAsync(x => x.Mute = true);
                _Muted.Add(user);
            }
            private async Task AddAllowed(SocketUser user)
            {
                SaveMuted.Add(user.Id, (user as SocketGuildUser).IsMuted);
                await (user as SocketGuildUser).ModifyAsync(x => x.Mute = false);
                _Allowed.Add(user);
            }


            public async Task Add(SocketUser user, bool bypass = false)
            {
                
                if(started.Subtract(DateTime.Now).TotalMinutes < 1 || bypass)
                {
                    await Console.UpdateObservers($"{user.Username} was added to tvroom as Allowed during allowed time");
                    await AddAllowed(user);

                }
                if(!_Allowed.Contains(user) && !_Admins.Contains(user))
                {
                    await Console.UpdateObservers($"{user.Username} was added to tvroom as Guest");
                    await AddGuest(user);
                }
            }
            public async Task Remove(SocketUser user)
            {
                try
                {
                    await (user as SocketGuildUser).ModifyAsync(x => x.Mute = SaveMuted[user.Id]);
                }catch(Exception e)
                {
                    await Console.UpdateObservers(e.Message);
                    foreach(SocketUser su in Admins)
                    {
                        IDMChannel dm = await su.GetOrCreateDMChannelAsync();
                        await dm.SendMessageAsync($"{user.Username} couldnt be unmuted");
                    }

                }
                if (_Muted.Contains(user))
                    _Muted.Remove(user);
                SaveMuted.Remove(user.Id);
            }
            public async Task Modify(SocketUser user)
            {
                if (_Muted.Contains(user) && !(user as SocketGuildUser).IsMuted)
                {
                    await Remove(user);
                    await AddAllowed(user);
                    await Console.UpdateObservers($"{user.Username} is now allowed");
                }
                if (Allowed.Contains(user) && (user as SocketGuildUser).IsMuted)
                {
                    await Remove(user);
                    await AddGuest(user);
                    await Console.UpdateObservers($"{user.Username} is now muted");
                }
            }
            public async Task<int> GetRoomCount()
            {
                return await Task.Run<int>(() =>
                {
                    return _Admins.Count + _Allowed.Count + _Muted.Count;
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
            await UpdateConsole($"{user == null} {oldChannel} {newChannel}");
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
            if (voicechannel != null && RunningTVRooms.ContainsKey($"{voicechannel.Id}"))
            {
                await UpdateConsole($"getting {voicechannel.Name}");
                return RunningTVRooms[$"{voicechannel.Id}"];
            }
            return null;
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
        public async Task<bool> RemoveRoom(SocketVoiceChannel voicechannel, SocketUser user)
        {
            TVroom room = await GetRoom(voicechannel);
            if(room != null && room.Admins.Contains(user))
            {
                await RemoveRoom(voicechannel);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task StartTvRoom(SocketVoiceChannel voicechannel, List<SocketUser> admins)
        {
            TVroom room = new TVroom(voicechannel, Console);
            foreach(SocketUser user in admins)
            {
                await room.AddAdmins(user);
            }
                
            foreach (SocketUser user in voicechannel.Users)
            {
                if (!admins.Contains(user))
                {
                    await room.Add(user, true);
                }
            }
            RunningTVRooms.Add($"{voicechannel.Id}", room);
            await Console.UpdateObservers(RunningTVRooms.Values.Count.ToString() + " rooms running");
        }
        public async Task<List<string>> GetListOfRooms()
        {
            return await Task.Run<List<string>>(() =>
            {
                return RunningTVRooms.Keys.ToList();
            });
        }
        public async Task<string> GetUserList(SocketVoiceChannel room)
        {
            StringBuilder output = new StringBuilder();
            TVroom currentRoom = await GetRoom(room);
            if (currentRoom != null)
            {
                output.AppendLine($"{currentRoom.TvChannel.Name}");
                output.AppendLine("Admins:");
                foreach(SocketUser user in currentRoom.Admins)
                {
                    output.AppendLine($"{user.Username}");
                }

                output.AppendLine("Allowed:");
                foreach (SocketUser user in currentRoom.Allowed)
                {
                    output.AppendLine($"{user.Username}");
                }

                output.AppendLine("Muted:");
                foreach (SocketUser user in currentRoom.Muted)
                {
                    output.AppendLine($"{user.Username}");
                }

            }
            else
            {
                output.AppendLine("No tv rooms in this channel");
            }
            return output.ToString();
        }
    }
}
